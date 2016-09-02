using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs.Upload
{
    public class UploadDTO
    {
        public string FileName { get; set; }
        public string Data { get; set; }
        public string Type { get; set; }
    }
}