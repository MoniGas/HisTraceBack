using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common;
using Dal;
using LinqModel;

namespace GenCodeServiceAllInOne
{
    /// <summary>
    /// 藏红包/藏优惠券
    /// </summary>
    public static class OperateActive
    {
        public static void Start()
        {
            ThreadStart start = ConcealRed;
            Thread th = new Thread(start) { IsBackground = true };
            th.Start();
        }
        public static void ConcealRed()
        {
            ActivityDAL dal = new ActivityDAL();
            string sql = string.Format(" select * from GenCondeInfoSetting where state={0}", 1);
            //服务运行时间间隔
            int timeSpan = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimeSpan"]);
            while (true)
            {
                //生成码的配置信息表
                try
                {
                    List<GenCondeInfoSetting> setCode = dal.GetGencodeInfo();
                    foreach (GenCondeInfoSetting set in setCode)
                    {
                        #region 循环处理每一个数据库的业务
                        //数据库连接串
                        string conStr = string.Format("Data Source={0};database={1};UID={2};pwd={3};",
                            set.DataBaseIP.ToString(), set.DataBaseName.ToString(),
                            set.DatabaseUserID.ToString(), set.DatabasePWD.ToString());
                        #region  查找还未藏红包的活动
                        List<YX_ActivitySub> activitySub = dal.GetActivitySub(conStr, (int)EnumText.ActivityMethod.Packet);
                        foreach (YX_ActivitySub sub in activitySub)
                        {
                            List<long> li = new List<long>();
                            //本次红包数量
                            int redSum = 0;
                            List<YX_AcivityDetail> detailList = dal.GetActivityDetail(conStr, sub.ActivityID);
                            foreach (var detail in detailList)
                            {
                                if (detail.RedCount > 0)
                                {
                                    for (int i = 0; i < detail.RedCount; i++)
                                    {
                                        li.Add(detail.AcivityRedDetailID);
                                    }
                                }
                                redSum += (int)detail.RedCount ;
                            }
                            //打乱红包顺序
                            shuffle(ref li); 
                            ActivityConceal concealModel = dal.GetActivityRelation(conStr, sub.ActivityID);
                            //数量相等或者小于优惠券数量，所有的码都更新
                            if (concealModel.codeNum == 0)
                            {
                                continue;
                            }
                            if (concealModel.codeNum == redSum || concealModel.codeNum < redSum)
                            {
                                concealModel.concealNum = concealModel.codeNum;
                                List<long> redList = li.Take(concealModel.concealNum).ToList();
                                li = li.Skip(concealModel.concealNum).ToList(); //移除已使用的红包
                                concealModel.redList = redList;
                                concealModel.ConcealCodeIds = string.Join(",", concealModel.fwCode);
                            }
                            else
                            {
                                List<long> num = GetTowLongNumRandom(redSum, concealModel.startId, concealModel.endId,concealModel.fwCode);
                                concealModel.concealNum = redSum;
                                concealModel.ConcealCodeIds = string.Join(",", num);
                                List<long> redList = li.Take(concealModel.concealNum).ToList();
                                li = li.Skip(concealModel.concealNum).ToList(); //移除已使用的红包
                                concealModel.redList = redList;
                            }
                            bool doSetCounpon = dal.UpdateRedCode(concealModel);
                            //if (doSetCounpon)
                            //{
                            //   GenCode.WriteLogTest("==========藏红包成功==========", "Conceal");
                            //}
                            //else
                            //{
                            //    GenCode.WriteLogTest("==========藏红包失败==========", "Conceal");
                            //}
                        }
                        #endregion

                        #region  查找还未藏优惠券的活动
                         activitySub = dal.GetActivitySub(conStr, (int)EnumText.ActivityMethod.Coupon);
                        foreach (YX_ActivitySub sub in activitySub)
                        {
                            List<long> li = new List<long>();
                            //本次红包数量
                            int redSum = 0;
                            YX_ActivityCoupon detailCounpon = dal.GetCouponDetail(conStr, sub.ActivityID);
                            for(int i=0;i<detailCounpon.CouponCount;i++)
                            {
                                li.Add(detailCounpon.CouponID);
                            }
                            redSum =(int) detailCounpon.CouponCount;
                            ActivityConceal concealModel = dal.GetActivityRelation(conStr, sub.ActivityID);
                            //数量相等或者小于优惠券数量，所有的码都更新
                            if (concealModel.codeNum == 0)
                            {
                                continue;
                            }
                            if (concealModel.codeNum == redSum || concealModel.codeNum < redSum)
                            {
                                concealModel.concealNum = concealModel.codeNum;
                                List<long> redList = li.Take(concealModel.concealNum).ToList();
                                li = li.Skip(concealModel.concealNum).ToList(); //移除已使用的红包
                                concealModel.redList = redList;
                                concealModel.ConcealCodeIds = string.Join(",", concealModel.fwCode);
                            }
                            else
                            {
                                List<long> num = GetTowLongNumRandom(redSum, concealModel.startId, concealModel.endId,concealModel.fwCode);
                                concealModel.concealNum = redSum;
                                concealModel.ConcealCodeIds = string.Join(",", num);
                                List<long> redList = li.Take(concealModel.concealNum).ToList();
                                li = li.Skip(concealModel.concealNum).ToList(); //移除已使用的红包
                                concealModel.redList = redList;
                            }
                            bool doSetCounpon = dal.UpdateCoupon(concealModel);
                            if (doSetCounpon)
                            {
                                GenCode.WriteLogTest("==========藏优惠券成功==========", "Conceal");
                            }
                            else
                            {
                                GenCode.WriteLogTest("==========藏优惠券失败==========", "Conceal");
                            }
                        }
                        #endregion

                        #region 处理过期的活动状态
                        bool activityState = dal.UpdateActivityState(conStr);
                        if (activityState)
                        {
                            GenCode.WriteLogTest("==========更新过期活动状态成功==========", "Conceal");
                        }
                        else
                        {
                            GenCode.WriteLogTest("==========更新过期活动状态失败==========", "Conceal");
                        }
                        #endregion
                        #endregion
                        Thread.Sleep(1000 * timeSpan);
                    }
                }
                catch (Exception ex)
                {
                    GenCode.WriteLog(ex.Message, "OperateActive");
                }
                Thread.Sleep(1000 * 120);
            }
        }

        /// <summary>
        /// 打乱集合数序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void shuffle<T>(ref List<T> list)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            List<T> newList = new List<T>();//儲存結果的集合  
            foreach (T item in list)
            {
                newList.Insert(rand.Next(0, newList.Count), item);
            }
            newList.Remove(list[0]);//移除list[0]的值  
            newList.Insert(rand.Next(0, newList.Count), list[0]);//再重新隨機插入第一筆  
            list = newList;
        }
        /// <summary>
        /// 在两个数之间随机出固定的数量
        /// </summary>
        /// <param name="shu"></param>
        /// <param name="minNum"></param>
        /// <param name="maxNum"></param>
        /// <returns></returns>
        public static List<long> GetTowLongNumRandom(int shu, long minNum, long maxNum, List<long> fwCode)
        {
            List<long> nos = new List<long>();
            var num = maxNum - minNum + 1;
            while (nos.Count < shu)
            {
                var rnd = new Random((int)DateTime.Now.Ticks);
                if (num < int.MaxValue)
                {
                    int i = 0;
                    while (i < shu)
                    {
                        int numa = rnd.Next(0, Convert.ToInt32(num));
                        long no = minNum + numa;
                        if (!nos.Contains((no)) && fwCode.Contains(no))
                        {
                            nos.Add(no);
                            i++;
                        }
                    }
                }
                else
                {
                    num = rnd.Next(0, int.MaxValue);
                    var no = minNum + num;
                    nos.Add(no);
                }
            }
            return nos;
        }
    }
}
