using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace FileApi.Filters
{
    /// <summary>
    /// GetImageHandler 的摘要说明
    /// </summary>
    public class GetImageHandler : IHttpHandler
    {
        private int[] _imageWidthLimits = new[] { 100, 255, 320 };

        public void ProcessRequest(HttpContext context)
        {
            //防盗链
            //if (context.Request.UrlReferrer == null || !context.Request.UrlReferrer.Host.Contains("cczcrv.com"))
            //{
            //    CreateNotFoundResponse(context);
            //    return;
            //}

            DateTime lastCacheTime;
            if (DateTime.TryParse(context.Request.Headers["If-Modified-Since"], out lastCacheTime))
            {
                if ((DateTime.Now - lastCacheTime).TotalMinutes < 20)
                {
                    CreateCacheResponse(context);
                    return;
                }
            }

            //图片不存在，返回默认图片
            var path = context.Server.MapPath(context.Request.Url.AbsolutePath);
            if (!System.IO.File.Exists(path))
            {
                CreateNotFoundResponse(context);
                return;
            }

            int width = 0;
            var strWidth = context.Request.Params["width"];
            if (!string.IsNullOrWhiteSpace(strWidth) && int.TryParse(strWidth, out width))
            {
                //验证请求的图片的尺寸是否在允许的范围内
                if (!_imageWidthLimits.Contains(width))
                {
                    CreateNotFoundResponse(context);
                    return;
                }

                var index = path.LastIndexOf('\\');

                //缩略图目录不存在，创建目录
                var thumbnailDirectory = "";
                if (!Directory.Exists(thumbnailDirectory))
                {
                    Directory.CreateDirectory(thumbnailDirectory);
                }
                var thumbnailPath = "";
                //缩略图不存在，生成缩略图
                if (!System.IO.File.Exists(thumbnailPath))
                {
                    var image = Image.FromFile(path);

                    //width大于图片本身宽度，则返回原图
                    if (width >= image.Width)
                    {
                        CreateImageResponse(context, path);
                        return;
                    }
                    //ThumbnailHelper.MakeThumbnail(image, thumbnailPath, width, 100, ThumbnailModel.W);
                }
                CreateImageResponse(context, thumbnailPath);
                return;
            }
            CreateImageResponse(context, path);
        }

        public bool IsReusable { get { return false; } }

        #region 私有方法

        /// <summary>
        /// 返回图片
        /// </summary>
        /// <param name="context">当前上下文</param>
        /// <param name="filePath">图片路径</param>
        private void CreateImageResponse(HttpContext context, string filePath)
        {
            context.Response.Cache.SetLastModified(DateTime.Now);
            context.Response.ContentType = "image/JPEG";
            context.Response.WriteFile(filePath);
            context.Response.End();
        }

        /// <summary>
        /// 返回默认图片
        /// </summary>
        /// <param name="context">当前上下文</param>
        private void CreateNotFoundResponse(HttpContext context)
        {
            var path = context.Server.MapPath("/upload/image/404.png");
            CreateImageResponse(context, path);
        }

        /// <summary>
        /// 返回缓存的内容，HttpCode等于304
        /// </summary>
        /// <param name="context"></param>
        private void CreateCacheResponse(HttpContext context)
        {
            context.Response.StatusCode = 304;
            context.Response.End();
        }

        #endregion 私有方法
    }
}