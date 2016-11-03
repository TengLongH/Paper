using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs.Author
{
    public class GetAuthorDTO
    {
        public int AuthorId { get; set; }
        public string PaperName { get; set; }
        public DateTime PublishDate { get; set; }
        public int? PaperId { get; set; }
        public string AuthorName { get; set; }
        public int SystemCandidateCount { get; set; }

    }
}