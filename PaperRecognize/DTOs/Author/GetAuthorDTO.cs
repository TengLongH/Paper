using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs.Author
{
    public class GetAuthorDTO
    {
        public int Id { get; set; }
        public int? PaperId { get; set; }
        public int? Ordinal { get; set; }
        public string NameEN { get; set; }
        public string NameENAbbr { get; set; }
        public string Department { get; set; }
    }
}