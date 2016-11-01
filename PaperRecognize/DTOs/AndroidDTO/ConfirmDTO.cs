using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs.AndroidDTO
{
    public class ConfirmDTO
    {

        public string Username { get; set; }
        public int CandidateId { get; set; }
        public bool Belongs { get; set; }

    }
}