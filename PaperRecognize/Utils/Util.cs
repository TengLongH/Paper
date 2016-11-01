using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PaperRecognize.DTOs;
using System.Web.SessionState;
using System.Net.Http;
using System.Web.Script.Serialization;
namespace PaperRecognize.Utils
{
    /// <summary>
    /// 工具类，包含一些静态的工具方法
    /// </summary>
    public class Util
    {
        /// <summary>
        /// 将对象转换为json然后用HttpResponseMessage封装。多在Controller里使用
        /// </summary>
        /// <param name="obj">需要封装的对象</param>
        /// <returns>将输入封装的HttpResponseMessage</returns>
        public static HttpResponseMessage toJson(Object obj)
        {
            String str;
            if (obj is String || obj is Char)
            {
                str = obj.ToString();
            }
            else
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                str = serializer.Serialize(obj);
            }
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, System.Text.Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }
        
    }
}