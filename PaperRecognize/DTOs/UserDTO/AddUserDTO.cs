using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs.UserDTO
{
    public class AddUserDTO
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
        public int? DepartmentId { get; set; }

    }
}