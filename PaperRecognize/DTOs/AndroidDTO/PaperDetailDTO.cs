using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs.AndroidDTO
{
    public class PaperDetailDTO
    {
        public int Id { get; set; }
        public string ChineseName { get; set; }
        public string DepartmentName { get; set; }
        public string LabName { get; set; }
        public string PaperName { get; set; }
        public string JournalName { get; set; }
        public string AuthorKeyWord { get; set; }
        public string KeyWords { get; set; }
        public string Abstract { get; set; }
        public string AuthorsAddress { get; set; }
        public DateTime? PublishDate { get; set; }

    }
}