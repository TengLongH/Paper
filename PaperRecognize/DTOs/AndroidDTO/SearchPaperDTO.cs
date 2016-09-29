using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs.AndroidDTO
{
    public class SearchPaperDTO
    {
        public string PaperName { get; set; }
        public string AuthorName { get; set; }
        public string Depart { get; set; }
    }
}