using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace FileApi.Controllers
{
    public class FileUploadController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> PostFile()
        {
            HttpRequestMessage request = this.Request;

            //是否请求包含multipart/form-data
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data/Uploads");
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            //var provider = new MultipartFormDataStreamProvider(root);

            var provider = new CustomMultipartFormDataStreamProvider(root);
            List<string> files = new List<string>();

            try
            {
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (MultipartFileData file in provider.FileData)
                {
                    files.Add(Path.GetFileName(file.LocalFileName));
                    //Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    //Trace.WriteLine("Server file path: " + file.LocalFileName);
                }

                // Send OK Response along with saved file names to the client.
                return Request.CreateResponse(HttpStatusCode.OK, files);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        // We implement MultipartFormDataStreamProvider to override the filename of File which
        // will be stored on server, or else the default name will be of the format like Body-
        // Part_{GUID}. In the following implementation we simply get the FileName from
        // ContentDisposition Header of the Request Body.
        public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
        {
            public CustomMultipartFormDataStreamProvider(string path)
                : base(path)
            {
            }

            public override string GetLocalFileName(HttpContentHeaders headers)
            {
                return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
            }
        }
    }
}