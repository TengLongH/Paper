using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs.AndroidDTO
{
    public class ClaimDTO
    {
        public string Username { get; set; }
        public int AuthorPersonId { get; set; }
        public bool Claim { get; set; }
    }
}