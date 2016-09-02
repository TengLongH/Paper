using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs.UserDTO
{
    public class GetUserDTO
    {
        public string Name { get; set; }
        public int Role { get; set; }
        public int? DepartmentId { get; set; }
    }
}