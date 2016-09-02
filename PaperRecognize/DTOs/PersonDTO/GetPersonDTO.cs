using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs.PersonDTO
{
    public class GetPersonDTO
    {
        public string PersonNo { get; set; }
        public string NameCN { get; set; }
        public string NameEN { get; set; }
        public string Sex { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public int? DepartmentId { get; set; }
    }
}