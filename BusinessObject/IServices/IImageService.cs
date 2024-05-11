using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.IServices
{
    public interface IImageService
    {
        public Task<string> UploadImage(IFormFile file);
    }
}
