/*****************************************************************
代码功能：身份验证特性
开发日期：2017年6月06日
作    者：苏凯丽
联系方式：15533621896
版权所有：追溯   
******************************************************************/
using System;

namespace MarketActive.Filter
{
    /// <summary>
    /// 匿名特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AllowAnonymousAttribute : Attribute
    {
        public AllowAnonymousAttribute() { }
    }
}