using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SkinScan_Services
{
    public class SkinModel
    {
        private readonly HttpClient _httpClient;

        public SkinModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> PredictAsync(string ApiUrl,string filePath)
        {
            using var formData = new MultipartFormDataContent();
            await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            formData.Add(fileContent, "file", Path.GetFileName(filePath));

            HttpResponseMessage response = await _httpClient.PostAsync(ApiUrl, formData);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync(); // Returns JSON response
            }
            else
            {
                throw new Exception($"Error: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
            }
        }
    }
}
