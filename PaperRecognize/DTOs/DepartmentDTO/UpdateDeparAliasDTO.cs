using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs.DepartmentDTO
{
    public class UpdateDeparAliasDTO
    {
        public int DepartmentId { get; set; }

        public string Alias { get; set; }
    }
}