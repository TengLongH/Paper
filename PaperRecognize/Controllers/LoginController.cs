using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PaperRecognize.Models;
using PaperRecognize.DTOs.Login;
using System.Web;
using PaperRecognize.Utils;

namespace PaperRecognize.Controllers
{
    public class LoginController : ApiController
    {
        private DBModel context = new DBModel();

        [Route("api/login")]
        public HttpResponseMessage PostLogin(LoginDTO dto)
        {
            if (null == dto.Name || null == dto.Password)
            {
                return new HttpResponseMessage() { Content = new StringContent("wrong") };
            }
            User user = null;
            try
            {
                user = context.User.FirstOrDefault(
                    u => u.Role == dto.Role 
                    && u.Name == dto.Name 
                    && u.Password == dto.Password);
            }
            catch( Exception e )
            {
                return new HttpResponseMessage() { Content = new StringContent(e.Message) };
            }
            
            if (null == user)
                return Util.toJson("wrong");
            var session = HttpContext.Current.Session;
            session.Add("username", user.Name);
            session.Add("role", user.Role);
            
            if (user.Role == 0)
                return new HttpResponseMessage() { Content = new StringContent("common") };
            if (user.Role == 1)
                return new HttpResponseMessage() { Content = new StringContent("depart") };
            return new HttpResponseMessage() { Content=new StringContent("wrong")};
        }
    }
}
