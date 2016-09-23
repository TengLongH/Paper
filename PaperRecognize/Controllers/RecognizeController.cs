using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PaperRecognize.Repository;
using PaperRecognize.DTOs;
using PaperRecognize.DTOs.PaperDTO;
using PaperRecognize.Business;
using System.Web;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using PaperRecognize.Import;

namespace PaperRecognize.Controllers
{
    public class RecognizeController : ApiController
    {
        private RecognizeRepository repository = new RecognizeRepository();


        public IEnumerable<GetAuthorPersonDTO> Put(UpdateAuthorPersonDTO update)
        {
            return repository.UpdateAuthorPerson(update);
        }

        [Route("api/recognize/pretreatment")]
        public string GetPretreatment()
        {
            Pretreatment p = new Pretreatment();
            try
            {
                p.pretreatPaper();
            }
            catch (Exception e)
            {
                return e.Message + e.StackTrace;
            }

            return "sucess";
        }


        [Route("api/recognize/importpapers")]
        public string GetImportPaper()
        {
            string path = HttpRuntime.AppDomainAppPath + @"App_Data/Paper";
            string[] files = Directory.GetFiles(path);
            if (null == files) return "success";
            ImportPaper import = new ImportPaper();
            foreach (string name in files)
            {
                if (name.EndsWith(".xls") || name.EndsWith(".xlsx"))
                {
                    import.ExcelToSQLServer(name, "Sheet1", true);
                    File.Delete( name );
                }
            }
            return "success";
        }

        [Route("api/recognize/search")]
        public string GetSearchAuthor()
        {
            string response = DateTime.Now.ToString();
            LookupAuthor lookup = new LookupAuthor();
            lookup.LookupPapers();
            return response + " to " + DateTime.Now.ToString();
        }
    }
}
