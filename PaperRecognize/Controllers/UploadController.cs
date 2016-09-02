using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PaperRecognize.DTOs.Upload;
using System.Web;
using PaperRecognize.Utils;
using System.IO;
using PaperRecognize.DTOs;
namespace PaperRecognize.Controllers
{
    public class UploadController : ApiController
    {
        public string PostUpload( UploadDTO dto )
        {
            try
            {
                if (!Util.isSchoolAdmin(HttpContext.Current.Session))
                {
                    return "need school admin";
                }

            }
            catch ( Exception e )
            {
                return e.Message;
            }

            object filename = Request.Properties["filename"];
            object data = Request.Properties["data"];
            if (null == filename || null == data)
            {
                return "filename or file data is empty";
            }

            return "";
        }

        private bool UploadFile(UploadDTO dto)
        {
            if (null == dto) return false;
            if (null == dto.FileName || dto.FileName.Trim().Length <= 0)
            {
                throw new Exception("FileName can't be empty");
            }
            if (null == dto.Data || dto.Data.Trim().Length <= 0)
            {
                throw new Exception("File data can't be empty");
            }
            if (null == dto.Type )
            {
                throw new Exception("FileName can't be empty");
            }

            string root = null;
            switch (dto.Type.ToLower())
            {
                case "paper":
                    if( !(dto.FileName.EndsWith("xlsx") || dto.FileName.EndsWith("xls")))
                        throw new Exception("paper file must be .xlsx or .xls");
                    root = HttpContext.Current.Server.MapPath("~/App_Data/Paper");
                    SaveFile(dto, root);
                    break;
                case "department":
                    root = HttpContext.Current.Server.MapPath("~/App_Data/Department");
                    SaveFile(dto, root);
                    break;
                case "person":
                    root = HttpContext.Current.Server.MapPath("~/App_Data/Person");
                    SaveFile(dto, root);
                    break;
                default:
                    throw new Exception("file type can't be find");
            }
            return true;
        }
        private void SaveFile( UploadDTO dto, string path )
        {
            byte[] bytes = System.Convert.FromBase64String(dto.Data);
            string str = System.Text.Encoding.UTF8.GetString( bytes );
            
            string suffirx = "";
            int start = dto.FileName.LastIndexOf('.');
            if (start > 0)
            {
                suffirx = dto.FileName.Substring( start );
            }
            string name = Guid.NewGuid().ToString() + suffirx ;
            
            File.WriteAllBytes( path + name, bytes ); 

        }
    }
}
