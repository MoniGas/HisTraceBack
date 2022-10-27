namespace LinqModel
{
    /// <summary>
    /// 包装箱码下载接口返回参数
    /// </summary>
    public class RReParam
    {
        public string PackagingIine { get; set; }
        public string MaterialFullName { get; set; }
        public string Count { get; set; }
        public string BatchName { get; set; }
        public string Spec { get; set; }
        public string Url { get; set; }
    }

    #region 刘晓杰于2019年11月4日从CFBack项目移入此

    /// <summary>
    /// 产品下载接口返回参数
    /// </summary>
    public class Materials
    {
        public long Material_ID { get; set; }
        public string MaterialFullName { get; set; }
        public string MaterialSpcificationName { get; set; }
        public string CategoryName { get; set; }
    }

    public class InterFaceMaterial
    {
        public long EnterPriseID { get; set; }

        public string MaterialFullName { get; set; }

        public long MaterialID { get; set; }

        public string DownLoadUrl { get; set; }

        public int CodeCount { get; set; }

        public long FileID { get; set; }
    }

    #endregion
}
