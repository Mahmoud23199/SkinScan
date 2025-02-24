using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinScan_BL.Contracts
{
    public interface ISaveFileService
    {
        Task<string> SaveFileAsync(IFormFile file);
        Task<string> SaveFileDecodeBase64Async(string base64String, string fileName);
    }
}
