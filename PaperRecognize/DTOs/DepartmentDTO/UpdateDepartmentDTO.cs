using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs.DepartmentDTO
{
    public class UpdateDepartmentDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public string ZipCode { get; set; }
        public int? ParentId { get; set; }
        public int? Type { get; set; }
    }
}