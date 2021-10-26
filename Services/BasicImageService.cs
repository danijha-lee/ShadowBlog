using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ShadowBlog.Services.Interfaces;

namespace ShadowBlog.Services
{
    public class BasicImageService : IImageService
    {
        public string ContentType(IFormFile file)
        {
            if (file is null)
            {
                return null;
            }

            return file.ContentType;
        }

        public string DecodeImage(byte[] data, string type)
        {
            if (data is null || type is null)
            {
                return null;
            }
            return $"data:image/{type};base64,{Convert.ToBase64String(data)}";
        }

        public async Task<byte[]> EncodeImageAsync(IFormFile file)
        {
            if (file is null)
            {
                return null;
            }

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> EncodeImageAsync(string fileName)
        {
            var file = $"{Directory.GetCurrentDirectory()}/wwwroot/img/{fileName}";
            return await File.ReadAllBytesAsync(file);
        }

        public int Size(IFormFile file)
        {
            if (file is null)
            {
                return 0;
            }

            return Convert.ToInt32(file.Length);
        }

        public bool ValidImage(IFormFile file)
        {
            return ValidType(file) && ValidSize(file);
        }

        public bool ValidSize(IFormFile file)
        {
            const int maxFileSize = 2 * 1024 * 1024;
            return Size(file) < maxFileSize;
        }

        public bool ValidType(IFormFile file)
        {
            var acceptableTypes = new List<String>();
            acceptableTypes.Add("jpg");
            acceptableTypes.Add("jpeg");
            acceptableTypes.Add("gif");
            acceptableTypes.Add("bmp");
            acceptableTypes.Add("png");

            var fileContentType = ContentType(file).Split("/")[1];

            var position = acceptableTypes.IndexOf(fileContentType);

            return position > 0;
        }
    }
}