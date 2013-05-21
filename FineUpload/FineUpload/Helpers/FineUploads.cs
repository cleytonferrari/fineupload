using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using ImageResizer;

namespace FineUpload.Helpers
{
    [ModelBinder(typeof(ModelBinder))]
    public class FineUploads
    {
        public string Filename { get; set; }
        public Stream InputStream { get; set; }

        public void SaveAs(string destination, bool overwrite = false, bool autoCreateDirectory = true)
        {
            if (autoCreateDirectory)
            {
                var directory = new FileInfo(destination).Directory;
                if (directory != null) directory.Create();
            }

            using (var file = new FileStream(destination, overwrite ? FileMode.Create : FileMode.CreateNew))
                InputStream.CopyTo(file);
        }


        public Dictionary<string, string> SaveAsVersions(string destination, FineUploads upload, bool autoCreateDirectory = true)
        {
            var versions = new Dictionary<string, string>
                               {
                                   {"_thumb", "width=100&height=100&format=jpg&crop=auto"},
                                   {"_medium", "maxwidth=400&maxheight=400format=jpg"},
                                   {"_large", "maxwidth=1600&maxheight=1600&format=jpg"}
                               };

            if (autoCreateDirectory)
            {
                var directory = new FileInfo(destination).Directory;
                if (directory != null) directory.Create();
            }

            //Isso ta muito feio, deve ter outra maneira de fazer isso, ao ler no Build, ele zera o inputStream, entao criei uma copia
            // http://eyeung003.blogspot.com.br/2010/01/c-reading-from-closed-stream.html
            var file = CopyStream(upload.InputStream);

            var retorno = new Dictionary<string, string>();
            var guid = System.Guid.NewGuid().ToString();

            foreach (var suffix in versions.Keys)
            {                
                var fileName = Path.Combine(destination, guid + suffix);
                fileName = ImageBuilder.Current.Build(file, fileName, new ResizeSettings(versions[suffix]), false, true);

                retorno.Add(suffix, Path.GetFileName(fileName));
            }

            file.Flush();
            file.Close();

            return retorno;
        }

        private static Stream CopyStream(Stream inputStream)
        {
            const int readSize = 256;
            byte[] buffer = new byte[readSize];
            MemoryStream ms = new MemoryStream();

            int count = inputStream.Read(buffer, 0, readSize);
            while (count > 0)
            {
                ms.Write(buffer, 0, count);
                count = inputStream.Read(buffer, 0, readSize);
            }
            ms.Seek(0, SeekOrigin.Begin);
            inputStream.Seek(0, SeekOrigin.Begin);
            return ms;
        }  

        public class ModelBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var request = controllerContext.RequestContext.HttpContext.Request;
                var formUpload = request.Files.Count > 0;

                // find filename
                var xFileName = request.Headers["X-File-Name"];
                var qqFile = request["qqfile"];
                var formFilename = formUpload ? request.Files[0].FileName : null;

                var upload = new FineUploads
                {
                    Filename = xFileName ?? qqFile ?? formFilename,
                    InputStream = formUpload ? request.Files[0].InputStream : request.InputStream
                };

                return upload;
            }
        }

    }
}