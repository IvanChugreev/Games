using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;

namespace Games
{
    public static class ImageHelper
    {
        private const int _IMAGE_WIDTH = 150;
        private const int _IMAGE_HEIGHT = 200;

        public static async Task<string> LoadImageAsync(IFormFile upload, IWebHostEnvironment environment)
        {
            if (upload == null)
            {
                return "";
            }

            var fileName = Path.GetFileName(upload.FileName);

            var extFile = fileName.Substring(fileName.Length - 3);

            if (extFile.Contains("png") || extFile.Contains("jpg") || extFile.Contains("bmp"))
            {
                var image = await Image.LoadAsync(upload.OpenReadStream());

                image.Mutate(x => x.Resize(_IMAGE_WIDTH, _IMAGE_HEIGHT));

                string path = "\\wwwroot\\images\\" + fileName;

                var rootPath = environment.ContentRootPath;

                await image.SaveAsync(rootPath + path);

                return fileName;
            }
            else
            {
                return "";
            }
        }
    }
}
