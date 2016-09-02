using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs.PersonDTO
{
    public class UpdatePersonDTO
    {
        public int Id { get; set; }
        public string PersonNo { get; set; }
        public string NameCN { get; set; }
        public string NameEN { get; set; }
        public string NameENAbbr { get; set; }
        public string Sex { get; set; }
        public DateTime? Birthday { get; set; }
        public string IDCard { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Tutor { get; set; }
        public int? PersonType { get; set; }
    }
}