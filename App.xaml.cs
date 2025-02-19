using System.Configuration;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Text.Json;

namespace PistachoStudiosLauncherWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public JsonRoot jsondata = new JsonRoot();
        protected override void OnStartup(StartupEventArgs e) //descargar y guardar test.txt desde servidor
        {
            
            base.OnStartup(e);
            HttpClient httpclient = new();
            var streamtask = httpclient.GetAsync("https://elchehost.es/launcher.json");
            streamtask.Wait();
            if(streamtask.Result.IsSuccessStatusCode)
            {
                JsonRoot? json = JsonSerializer.Deserialize<JsonRoot>(new StreamReader(streamtask.Result.Content.ReadAsStream()).ReadToEnd());
                if(json != null)
                {
                    jsondata = json;
                    MessageBox.Show(json.game.codename);
                    MessageBox.Show(json.game.uuid);
                    if (!Path.Exists(Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\"))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\");
                    }
                    FileStream fs = new(Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\" + "launcher.json", FileMode.OpenOrCreate);
                    fs.SetLength(0);
                    fs.Flush();
                    streamtask.Result.Content.ReadAsStream().CopyTo(fs);
                }
            }
            
        }
    }
}
