using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FineUpload.Helpers;
using ImageResizer;

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
            var dir = System.Web.HttpContext.Current.Server.MapPath("~/Content/Uploads/");

            Dictionary<string, string> listaDeArquivos;

            try
            {
                listaDeArquivos = upload.SaveAsVersions(dir, upload, autoCreateDirectory: true);

            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, error: ex.Message);
            }

            return new FineUploaderResult(true, new { img = "Content/Uploads/" + listaDeArquivos["_thumb"] });
        }


    }
}
