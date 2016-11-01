using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs
{
    public class GetAuthorPersonDTO
    {
        public int Id { get; set; }
        public int? AuthorId { get; set; }
        public string PersonNo { get; set; }
        public string Name { get; set; }
        public CandidateStatus status { get; set; }
    }
}