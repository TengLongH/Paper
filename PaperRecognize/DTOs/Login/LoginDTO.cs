using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs.Login
{
    public class LoginDTO
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public int Role {get;set;}
    }
}