using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace PaperRecognize.Repository
{
    public class MyProvider : System.Net.Http.MultipartFormDataStreamProvider
    {
       
        public MyProvider(string root):base(root)
        {
            
        }
        public override string GetLocalFileName( HttpContentHeaders headers )
        {
            string name = headers.ContentDisposition.FileName;
            if (null == name) return "unknow.xlsx";
            int start = name.LastIndexOf('.');
            if (start < 0) return "file name format error";
            string prix = name.Substring(start);
            
            prix = prix.Replace("\"", "");
            if (prix != ".xls" && prix != ".xlsx") return "file type is error";
            name = Guid.NewGuid().ToString() + prix;
            return name;
        }
    }
}