using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (Request.Files.Count <= 0)
            {
                return;
            }

            HttpPostedFile file = Request.Files[0];
            //HttpPostedFile to binary
            int FileLen = file.ContentLength;
            Byte[] FileData = new Byte[FileLen];
            Stream sr = file.InputStream;//创建数据流对象
            sr.Read(FileData, 0, FileLen);
            sr.Close();

            using (var client = new HttpClient())
            using (var content = new MultipartFormDataContent())
            {
                // Make sure to change API address
                client.BaseAddress = new Uri("http://localhost:8585/");

                // Add first file content
                var fileContent1 = new ByteArrayContent(FileData);
                fileContent1.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = file.FileName
                };

                content.Add(fileContent1);

                // Make a call to Web API
                var result = client.PostAsync("/api/FileUpload/PostFile", content).Result;

                Response.Write(result.StatusCode);
                Response.Write(result.Content);
            }
        }
    }
}