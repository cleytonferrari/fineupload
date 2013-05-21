using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FineUpload.Helpers;

namespace FineUpload.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public FineUploaderResult UploadFile(FineUploads upload)
        {
            // asp.net mvc will set extraParam1 and extraParam2 from the params object passed by Fine-Uploader

            var dir = System.Web.HttpContext.Current.Server.MapPath("~/Content/Uploads/");

            var filePath = Path.Combine(dir, upload.Filename);
            try
            {
                upload.SaveAs(filePath, overwrite: true, autoCreateDirectory: true);
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, error: ex.Message);
            }

            // the anonymous object in the result below will be convert to json and set back to the browser
            return new FineUploaderResult(true, new { img = "Content/Uploads/" + upload.Filename });
        }
    }
}
