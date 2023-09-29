using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Iminetsoft
{
        public class ImgBBApi
        {
                public string ApiKey { get; set; } = String.Empty;

                public ImgBBApi() { }

                public ImgBBApi(string apikey) { ApiKey = apikey; }

                public async Task<List<string>> UploadImgbbAsync(List<string> ImagePaths)
                {
                        List<string> LinksToUploadedImages = new List<string>();
                        string ResponseJsonContent = string.Empty;

                        using (HttpClient client = new HttpClient())
                        {
                                try
                                {
                                        foreach (string imagePath in ImagePaths)
                                        {
                                                /* SORGU GONDERDIK VE JSON RESPONSE-U ELDE ETDIK: */
                                                string requestUrl = $"https://api.imgbb.com/1/upload?key={ApiKey}";
                                                MultipartFormDataContent content = new MultipartFormDataContent();
                                                string base64OfImage = Convert.ToBase64String(File.ReadAllBytes(imagePath));
                                                content.Add(new StringContent(base64OfImage), "image");
                                                content.Add(new StringContent(Path.GetFileNameWithoutExtension(imagePath)), "name");
                                                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUrl) { Content = content };
                                                HttpResponseMessage response = await client.SendAsync(request);
                                                ResponseJsonContent = await response.Content.ReadAsStringAsync();

                                                /* JSON RESPONSE-DAN LAZIMI HISSELERI EXTRACT EDEK: */
                                                JObject jsonObject = JObject.Parse(ResponseJsonContent);
                                                LinksToUploadedImages.Add(jsonObject.SelectToken("data.url").ToString());
                                        }
                                }
                                catch { }
                        }

                        return LinksToUploadedImages;
                }
        }
}