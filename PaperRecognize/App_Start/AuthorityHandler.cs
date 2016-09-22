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
    public class AuthorityHandler: DelegatingHandler
    {
        private static JObject json;

        public AuthorityHandler()
        {

            string root = HttpContext.Current.Server.MapPath("~/authority.json");
            string jsonText = File.ReadAllText(root);
            json = JObject.Parse(jsonText);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string url = request.RequestUri.AbsolutePath;
            if (url.Equals("/api/login"))
            {
                //return request.CreateResponse();
                return await base.SendAsync(request, cancellationToken);
            }
          
            var session = HttpContext.Current.Session;
            //if ( Accept(url, session))
            if( url.Length > 0 )
            {
                return await base.SendAsync(request, cancellationToken);
            }
            return new HttpResponseMessage( System.Net.HttpStatusCode.Forbidden );
        }
        public bool Accept(string url, Object userInfo)
        {

            LoginDTO dto = GetCurrentUser(userInfo);
            if (null == dto) return false;
            return Check(dto.Role, url );
        }

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