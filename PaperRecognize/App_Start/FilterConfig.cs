using System.Web;
using System.Web.Mvc;

namespace PaperRecognize
{
    /// <summary>
    /// 添加过滤器的类
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// 添加过滤器的静态方法
        /// </summary>
        /// <param name="filters">过滤器集合</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
