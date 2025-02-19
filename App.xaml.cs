using System.Configuration;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Text.Json;
using System.Diagnostics;
using System.Text;

namespace PistachoStudiosLauncherWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public readonly static string serverlocation = "https://elchehost.es";


        public JsonRoot jsondata = new JsonRoot();
        protected override void OnStartup(StartupEventArgs e) //descargar y guardar test.txt desde servidor
        {
            base.OnStartup(e);


            HttpClient httpclient = new();
            var streamtask = httpclient.GetAsync(serverlocation + "/launcher.json");
            streamtask.Wait();
            if (streamtask.Result.IsSuccessStatusCode)
            {
                string readtoend = new StreamReader(streamtask.Result.Content.ReadAsStream()).ReadToEnd();
                JsonRoot? json = JsonSerializer.Deserialize<JsonRoot>(readtoend);
                if (json != null && json.game != null && json.game.modloader != null && json.game.server != null)
                {
                    jsondata = json;
                    if (!Path.Exists(Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\")) //if web folder does not exist, create it
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\");
                        FileStream fs = new(Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\" + "launcher.json", FileMode.OpenOrCreate);
                        fs.SetLength(0);
                        fs.Flush();
                        fs.Write(Encoding.UTF8.GetBytes(readtoend), 0, readtoend.Length);
                    }
                    else //compare if the file exists then check if web version is different. if true, replace and update
                    {
                        if (File.Exists(Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\" + "launcher.json"))
                        {
                            FileStream localfs = new(Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\" + "launcher.json", FileMode.OpenOrCreate);
                            string readtoendlocal = new StreamReader(localfs).ReadToEnd();
                            JsonRoot? localjson = JsonSerializer.Deserialize<JsonRoot>(readtoendlocal);
                            if (localjson != json && localjson != null)
                            {
                                localfs.SetLength(0);
                                localfs.Flush();
                                localfs.Write(Encoding.UTF8.GetBytes(readtoendlocal), 0, readtoendlocal.Length);
                            }
                            streamtask = httpclient.GetAsync(serverlocation + "/screenshots/screenshots.7z");
                            streamtask.Wait();
                            if (streamtask.Result.IsSuccessStatusCode)
                            {
                                FileStream ssfs = new(Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\" + "screenshots.7z", FileMode.OpenOrCreate);
                                ssfs.SetLength(0);
                                ssfs.Flush();
                                streamtask.Result.Content.ReadAsStream().CopyTo(ssfs);
                                ssfs.Close();
                                Process un7z = new();
                                un7z.StartInfo.FileName = Path.GetDirectoryName(Environment.ProcessPath) + @"\7z\7zr.exe";
                                un7z.StartInfo.Arguments = "x -y -o\"" + Path.GetDirectoryName(Environment.ProcessPath) + "\\media\\img\\\" " + Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\screenshots.7z";
                                un7z.StartInfo.UseShellExecute = false;
                                un7z.StartInfo.RedirectStandardOutput = true;
                                un7z.Start();
                                un7z.WaitForExit();
                                //MessageBox.Show(un7z.StandardOutput.ReadToEnd());
                            }
                        }
                        else //if file does not exist, do not check for differences. just save
                        {
                            FileStream fs = new(Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\" + "launcher.json", FileMode.OpenOrCreate);
                            fs.SetLength(0);
                            fs.Flush();
                            fs.Write(Encoding.UTF8.GetBytes(readtoend), 0, readtoend.Length);
                        }
                    }
                }
            }

        }
    }
}
