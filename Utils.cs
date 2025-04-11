using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Diagnostics;
using System.Security.Policy;

namespace PistachoStudiosLauncherWPF
{
    public static class Utils
    {
        public static ImageSource ToImageSource(System.Drawing.Image image, ImageFormat imageFormat)
        {
            BitmapImage bitmap = new BitmapImage();

            using (MemoryStream stream = new MemoryStream())
            {
                // Save to the stream
                image.Save(stream, imageFormat);

                // Rewind the stream
                stream.Seek(0, SeekOrigin.Begin);

                // Tell the WPF BitmapImage to use this stream
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
            }

            return bitmap;
        }

        private static Random rng = new Random();

        public static Task WaitForExitAsync(this Process process,
    CancellationToken cancellationToken = default(CancellationToken))
        {
            if (process.HasExited) return Task.CompletedTask;

            var tcs = new TaskCompletionSource<object>();
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => tcs.TrySetResult(null);
            if (cancellationToken != default(CancellationToken))
                cancellationToken.Register(() => tcs.SetCanceled());

            return process.HasExited ? Task.CompletedTask : tcs.Task;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static string PathCheck(string path)
        {
            if(path != string.Empty)
            {
                return path;
            } else
            {
                return "index.html";
            }
        }

        public class HttpHelper
        {
            private HttpClient _client;
            public HttpHelper(HttpClient client)
            {
                _client = client;
            }
            public async Task<bool> GetAndStoreAsync(string url, string filePath)
            {
                var response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    using (FileStream fs = new(filePath, FileMode.Create))
                    {
                        await response.Content.CopyToAsync(fs);
                        await fs.FlushAsync();
                    }
                }
                return response.IsSuccessStatusCode;
            }
            public async Task<Stream?> GetStreamIfSuccessAsync(string url)
            {
                var response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStreamAsync();
                }
                return null;
            }
        }
        public class _7zrexeHelper //7zr only supports 7z archives
        {
            private string _7zrexePath;
            public _7zrexeHelper(string executableLocation)
            {
                _7zrexePath = executableLocation;
            }
            public Process Extract7z(string fileName, string outputDirectory)
            {
                Process _7zr = new();
                _7zr.StartInfo.FileName = _7zrexePath;
                _7zr.StartInfo.Arguments = "x -y -o\"" + outputDirectory + "\" \"" + fileName + "\"";
                _7zr.StartInfo.UseShellExecute = false;
                _7zr.StartInfo.RedirectStandardOutput = true;
                _7zr.StartInfo.RedirectStandardError = true;
                _7zr.StartInfo.CreateNoWindow = true;
                _7zr.Start();
                _7zr.WaitForExit();
                return _7zr;
            }
        }
    }
}
