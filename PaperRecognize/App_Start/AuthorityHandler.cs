using Newtonsoft.Json.Linq;
using PaperRecognize.DTOs.Login;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace PaperRecognize.App_Start
{
    /// <summary>
    /// 过滤器类
    /// </summary>
    public class AuthorityHandler: DelegatingHandler
    {
        private static JObject json;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AuthorityHandler()
        {

            string root = HttpContext.Current.Server.MapPath("~/authority.json");
            string jsonText = File.ReadAllText(root);
            json = JObject.Parse(jsonText);
        }

        /// <summary>
        /// 拦截请求，做一些处理
        /// </summary>
        /// <param name="request">拦截的用户请求</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string url = request.RequestUri.AbsolutePath;
            var session = HttpContext.Current.Session;
            if (session["username"] == null) url = "/api/login";
                if (url.Equals("/api/login"))
            {
                //return request.CreateResponse();
                return await base.SendAsync(request, cancellationToken);
            }

            if ( Accept(url, session))
            {
                return await base.SendAsync(request, cancellationToken);
            }
            return new HttpResponseMessage( System.Net.HttpStatusCode.Forbidden );
        }

        /// <summary>
        /// 检查是否允许用户访问这个URL
        /// </summary>
        /// <param name="url">资源的url</param>
        /// <param name="userInfo">用户的相关信息</param>
        /// <returns>如果允许用户访问返回true，否则返回false</returns>
        public bool Accept(string url, Object userInfo)
        {

            LoginDTO dto = GetCurrentUser(userInfo);
            if (null == dto) return false;
            return Check(dto.Role, url );
        }
        /// <summary>
        /// 将在线用户的信息封装成一个LoginDTO对象
        /// </summary>
        /// <param name="userInfo">包含在线用户信息的对象</param>
        /// <returns>将在线用户的信息封装成一个LoginDTO</returns>
        public LoginDTO GetCurrentUser(Object userInfo)
        {
            if (null == userInfo) return null;
            LoginDTO dto = new LoginDTO();
            if (userInfo is HttpSessionState)
            {
                HttpSessionState session = (HttpSessionState)userInfo;
                object o = session["username"];
                if (null == o) return null;
                dto.Name = o.ToString();
                o = session["role"];
                if (null == o) return null;
                dto.Role = Int32.Parse(o.ToString());
            }

            return dto;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool Check(int role, string url)
        {
            string roleStr = null;
            if (0 == role)
            {
                roleStr = "common";
            }
            else
            {
                roleStr = "admin";
            }
            JObject roles = (JObject)json.GetValue("role");
            JToken authorities = roles.GetValue(roleStr);
            if (null == authorities) return false;
            foreach (JToken a in authorities)
            {
                string n = a.ToString();
                if (url.StartsWith(n))
                {
                    return true;
                }
            }

            return false;
        }
    }
}