/********************************************************************************
** 作者： 赵慧敏
** 创始时间：2017-04-21
***联系方式 :13313318725
** 描述：枚举转换
***版本：v2.0
***版权：农业项目组  
*********************************************************************************/

namespace Common.Tools
{
    /// <summary>
    /// 生成码枚举转换
    /// </summary>
    public static class CodeNodeToType
    {
        public static int ToType(string codeNode)
        {
            int result = 0;
            switch (int.Parse(codeNode))
            {
                case (int)EnumFile.CodeType.single:
                    result = (int)EnumFile.GenCodeType.single;
                    break;
                case (int)EnumFile.CodeType.bGroup:
                    result = (int)EnumFile.GenCodeType.trap;
                    break;
                case (int)EnumFile.CodeType.boxCode:
                    result = (int)EnumFile.GenCodeType.boxCode;
                    break;
                case (int)EnumFile.CodeType.localSingle:
                    result = (int)EnumFile.GenCodeType.localCreate;
                    break;
                case (int)EnumFile.CodeType.localBox:
                    result = (int)EnumFile.GenCodeType.localCreateBox;
                    break;
                case (int)EnumFile.CodeType.localGift:
                    result = (int)EnumFile.GenCodeType.localGift;
                    break;
                case (int)EnumFile.CodeType.gift:
                    result = (int)EnumFile.GenCodeType.gift;
                    break;
                case (int)EnumFile.CodeType.bSingle:
                    result = (int)EnumFile.GenCodeType.trap;
                    break;
            }
            return result;
        }
    }
}
