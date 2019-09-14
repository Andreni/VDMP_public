using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;

namespace VDMP.App.Scraper
{
    /// <summary>Used for downloading posters from the TMDb</summary>
    public class ImageDownloader
    {
        /// <summary>Image downloader task.</summary>
        /// <param name="url">The URL.</param>
        /// <param name="pictureName">Name of the picture.</param>
        /// <returns>The path where the image was downloaded</returns>
        public async Task<string> ImageDownloaderTask(string url, string pictureName)
        {
            pictureName = pictureName.Remove(0, 1);
            var localFolder = ApplicationData.Current.LocalFolder;
            var imageStorageFile =
                await localFolder.CreateFileAsync(pictureName, CreationCollisionOption.GenerateUniqueName);
            var client = new HttpClient();

            var bufferedBytes = await client.GetByteArrayAsync(url);

            using (var stream = await imageStorageFile.OpenStreamForWriteAsync())
            {
                stream.Write(bufferedBytes, 0, bufferedBytes.Length);
            }

            return $"ms-appdata:///local/{imageStorageFile.Name}";
        }
    }
}