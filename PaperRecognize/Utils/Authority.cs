using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaperRecognize.DTOs.Login;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace PaperRecognize.Utils
{
    public class Authority
    {
        private static JObject json;
        private static Authority instance;
        private Authority()
        {

            string root = HttpContext.Current.Server.MapPath("~/authority.json");
            string jsonText = File.ReadAllText( root );
            json = JObject.Parse(jsonText);
        }

        public static Authority GetInstance()
        {
            if (null == instance)
            {
                instance = new Authority();
            }
            return instance;
        }
        public bool Accept(MethodBase method, Object userInfo)
        {

            LoginDTO dto = GetCurrentUser( userInfo );
            if (null == dto) return false;
            string methodName = GetControllerMethod(method);
            return Check( dto.Role, methodName );
        }
        public string GetControllerMethod(MethodBase method )
        {

            StringBuilder builder = new StringBuilder();
            if( null != builder)
            {
                builder.Append(method.DeclaringType.Name);
                builder.Append(".");
                builder.Append(method.Name);
            }
            return builder.ToString();
        }

        public LoginDTO GetCurrentUser( Object userInfo )
        {
            if (null == userInfo) return null;
            LoginDTO dto = new LoginDTO();
            if (userInfo is HttpSessionState)
            {
                HttpSessionState session = (HttpSessionState)userInfo;
                object o = session["username"];
                if (null == o) return null;
                dto.Name = o.ToString() ;
                o = session["role"];
                if (null == o) return null; 
                dto.Role = Int32.Parse(o.ToString());
            }
            //if( userInfo is )
            
            return dto;
        }

        public bool Check(int role, string methodName)
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
                if (methodName.Equals(n))
                {
                    return true;
                }
            }

            return false;
        }

    }
}