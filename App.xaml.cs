using System.IO;
using System.Net.Http;
using System.Windows;
using System.Text.Json;
using System.Diagnostics;
using System.Text;
using System.Net;

namespace PistachoStudiosLauncherWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public readonly static string serverlocation = "https://elchehost.es";


        public JsonLauncherRoot jsondata = new JsonLauncherRoot();
        protected override void OnStartup(StartupEventArgs e) //descargar y guardar test.txt desde servidor
        {
            base.OnStartup(e);

            //jankiest code ever

            HttpClient httpclient = new();
            //httpclient.DefaultRequestHeaders.UserAgent.Clear();
            //httpclient.DefaultRequestHeaders.UserAgent.ParseAdd("PistachoStudiosLauncherWPF/1.0");
            var streamtask = httpclient.GetAsync(serverlocation + "/launcher.json");
            streamtask.Wait();
            if (streamtask.Result.IsSuccessStatusCode)
            {
                string readtoend = new StreamReader(streamtask.Result.Content.ReadAsStream()).ReadToEnd();
                if (!readtoend.Contains("<!DOCTYPE html>")) {
                    JsonLauncherRoot? json = JsonSerializer.Deserialize<JsonLauncherRoot>(readtoend);
                    if (json != null && json.game != null && json.game.modloader != null && json.game.server != null)
                    {
                        jsondata = json;
                        if (!Path.Exists(Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\")) //if web folder does not exist, create it
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\");
                        }
                        if (!Path.Exists(Path.GetDirectoryName(Environment.ProcessPath) + "\\json\\")) //if json folder does not exist, create it and copy downloaded launcher.json
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(Environment.ProcessPath) + "\\json\\");
                            FileStream fs = new(Path.GetDirectoryName(Environment.ProcessPath) + "\\json\\launcher.json", FileMode.OpenOrCreate);
                            fs.SetLength(0);
                            fs.Flush();
                            fs.Write(Encoding.UTF8.GetBytes(readtoend), 0, readtoend.Length);

                            DownloadFile df = new();
                            df.Show();

                            FileStream ssfs = new(Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\" + json.game.uuid + ".7z", FileMode.OpenOrCreate);
                            ssfs.SetLength(0);
                            ssfs.Flush();
                            //streamtask = httpclient.GetAsync(serverlocation + "/template/" + json.game.uuid + ".7z");
                            var str23 = httpclient.GetStreamAsync(serverlocation + "/template/" + json.game.uuid + ".7z");
                            //streamtask.Wait();
                            /*if (!streamtask.Result.IsSuccessStatusCode)
                            {
                                MessageBox.Show("No se pudo actualizar el launcher." + Environment.NewLine + "Abra la aplicación para intentarlo de nuevo." + Environment.NewLine + "C", "Fallo en la actualización", MessageBoxButton.OK, MessageBoxImage.Error);
                                File.Delete(Path.GetDirectoryName(Environment.ProcessPath) + "\\json\\launcher.json");
                                df.Close();
                                return;
                            }
                            if (!Path.Exists(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\")) //if web folder does not exist, create it
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\");
                            }
                            else
                            {
                                if (Directory.GetFiles(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\").Length > 0)
                                {
                                    foreach(string file in Directory.GetFiles(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\"))
                                    {
                                        File.Delete(file);
                                    }
                                }
                            }*/
                            str23.Result.CopyTo(ssfs);
                            ssfs.Flush();
                            ssfs.Close();
                            Process un7z = new();
                            un7z.StartInfo.FileName = Path.GetDirectoryName(Environment.ProcessPath) + @"\7z\7zr.exe";
                            un7z.StartInfo.Arguments = "x -y -o\"" + Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\\" " + Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\" + json.game.uuid + ".7z";
                            un7z.StartInfo.UseShellExecute = false;
                            un7z.StartInfo.RedirectStandardOutput = true;
                            un7z.StartInfo.CreateNoWindow = true;
                            un7z.Start();
                            un7z.WaitForExit();

                            FileStream ssfs2 = new(Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\screenshots.7z", FileMode.OpenOrCreate);
                            ssfs2.SetLength(0);
                            ssfs2.Flush();
                            //streamtask = httpclient.GetAsync(serverlocation + "/screenshots/screenshots.7z");
                            var str232 = httpclient.GetStreamAsync(serverlocation + "/screenshots/screenshots.7z");
                            //streamtask.Wait();
                            /*if (!streamtask.Result.IsSuccessStatusCode)
                            {
                                MessageBox.Show("No se pudo actualizar el launcher." + Environment.NewLine + "Abra la aplicación para intentarlo de nuevo." + Environment.NewLine + "C", "Fallo en la actualización", MessageBoxButton.OK, MessageBoxImage.Error);
                                File.Delete(Path.GetDirectoryName(Environment.ProcessPath) + "\\json\\launcher.json");
                                df.Close();
                                return;
                            }
                            if (!Path.Exists(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\")) //if web folder does not exist, create it
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\");
                            }
                            else
                            {
                                if (Directory.GetFiles(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\").Length > 0)
                                {
                                    foreach(string file in Directory.GetFiles(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\"))
                                    {
                                        File.Delete(file);
                                    }
                                }
                            }*/
                            str232.Result.CopyTo(ssfs2);
                            ssfs2.Flush();
                            ssfs2.Close();
                            Process un7z2 = new();
                            un7z2.StartInfo.FileName = Path.GetDirectoryName(Environment.ProcessPath) + @"\7z\7zr.exe";
                            un7z2.StartInfo.Arguments = "x -y -o\"" + Path.GetDirectoryName(Environment.ProcessPath) + "\\media\\img\\\" " + Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\screenshots.7z";
                            un7z2.StartInfo.UseShellExecute = false;
                            un7z2.StartInfo.RedirectStandardOutput = true;
                            un7z2.StartInfo.CreateNoWindow = true;
                            un7z2.Start();
                            un7z2.WaitForExit();
                            //MessageBox.Show(un7z.StandardOutput.ReadToEnd());
                            MessageBox.Show("Actualización completada. Abra la aplicación de nuevo.", "Actualización", MessageBoxButton.OK, MessageBoxImage.Information);
                            df.Close();
                        }
                        else //compare if the file exists then check if web version is different. if true, replace and update
                        {
                            if (File.Exists(Path.GetDirectoryName(Environment.ProcessPath) + "\\json\\launcher.json"))
                            {
                                FileStream localfs = new(Path.GetDirectoryName(Environment.ProcessPath) + "\\json\\launcher.json", FileMode.OpenOrCreate);
                                string readtoendlocal = new StreamReader(localfs).ReadToEnd();

                                JsonLauncherRoot? localjson = null;
                                try
                                {
                                    localjson = JsonSerializer.Deserialize<JsonLauncherRoot>(readtoendlocal);
                                } catch (Exception)
                                {
                                    
                                }
                                if (localjson != null)
                                {
                                    if (readtoend != readtoendlocal)
                                    {
                                        MessageBoxResult mbr = MessageBox.Show("Se ha encontrado una actualización." + Environment.NewLine + "Presiona Aceptar para actualizar o Cancelar para salir del programa.", "Actualización disponible", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                                        if (mbr == MessageBoxResult.Cancel)
                                        {
                                            Environment.Exit(0);
                                        }
                                        DownloadFile df = new();
                                        df.Show();
                                        localfs.SetLength(0);
                                        localfs.Flush();
                                        localfs.Write(Encoding.UTF8.GetBytes(readtoend), 0, readtoend.Length);

                                        FileStream ssfs = new(Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\" + json.game.uuid + ".7z", FileMode.OpenOrCreate);
                                        ssfs.SetLength(0);
                                        ssfs.Flush();
                                        //streamtask = httpclient.GetAsync(serverlocation + "/template/" + json.game.uuid + ".7z");
                                        var str23 = httpclient.GetStreamAsync(serverlocation + "/template/" + json.game.uuid + ".7z");
                                        //streamtask.Wait();
                                        /*if (!streamtask.Result.IsSuccessStatusCode)
                                        {
                                            MessageBox.Show("No se pudo actualizar el launcher." + Environment.NewLine + "Abra la aplicación para intentarlo de nuevo." + Environment.NewLine + "C", "Fallo en la actualización", MessageBoxButton.OK, MessageBoxImage.Error);
                                            File.Delete(Path.GetDirectoryName(Environment.ProcessPath) + "\\json\\launcher.json");
                                            df.Close();
                                            return;
                                        }
                                        if (!Path.Exists(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\")) //if web folder does not exist, create it
                                        {
                                            Directory.CreateDirectory(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\");
                                        }
                                        else
                                        {
                                            if (Directory.GetFiles(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\").Length > 0)
                                            {
                                                foreach(string file in Directory.GetFiles(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\"))
                                                {
                                                    File.Delete(file);
                                                }
                                            }
                                        }*/
                                        str23.Result.CopyTo(ssfs);
                                        ssfs.Flush();
                                        ssfs.Close();
                                        Process un7z = new();
                                        un7z.StartInfo.FileName = Path.GetDirectoryName(Environment.ProcessPath) + @"\7z\7zr.exe";
                                        un7z.StartInfo.Arguments = "x -y -o\"" + Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\\" " + Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\" + json.game.uuid + ".7z";
                                        un7z.StartInfo.UseShellExecute = false;
                                        un7z.StartInfo.RedirectStandardOutput = true;
                                        un7z.StartInfo.CreateNoWindow = true;
                                        un7z.Start();
                                        un7z.WaitForExit();
                                        //MessageBox.Show(un7z.StandardOutput.ReadToEnd());

                                        FileStream ssfs2 = new(Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\screenshots.7z", FileMode.OpenOrCreate);
                                        ssfs2.SetLength(0);
                                        ssfs2.Flush();
                                        //streamtask = httpclient.GetAsync(serverlocation + "/screenshots/screenshots.7z");
                                        var str232 = httpclient.GetStreamAsync(serverlocation + "/screenshots/screenshots.7z");
                                        //streamtask.Wait();
                                        /*if (!streamtask.Result.IsSuccessStatusCode)
                                        {
                                            MessageBox.Show("No se pudo actualizar el launcher." + Environment.NewLine + "Abra la aplicación para intentarlo de nuevo." + Environment.NewLine + "C", "Fallo en la actualización", MessageBoxButton.OK, MessageBoxImage.Error);
                                            File.Delete(Path.GetDirectoryName(Environment.ProcessPath) + "\\json\\launcher.json");
                                            df.Close();
                                            return;
                                        }
                                        if (!Path.Exists(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\")) //if web folder does not exist, create it
                                        {
                                            Directory.CreateDirectory(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\");
                                        }
                                        else
                                        {
                                            if (Directory.GetFiles(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\").Length > 0)
                                            {
                                                foreach(string file in Directory.GetFiles(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\"))
                                                {
                                                    File.Delete(file);
                                                }
                                            }
                                        }*/
                                        str232.Result.CopyTo(ssfs2);
                                        ssfs2.Flush();
                                        ssfs2.Close();
                                        Process un7z2 = new();
                                        un7z2.StartInfo.FileName = Path.GetDirectoryName(Environment.ProcessPath) + @"\7z\7zr.exe";
                                        un7z2.StartInfo.Arguments = "x -y -o\"" + Path.GetDirectoryName(Environment.ProcessPath) + "\\media\\img\\\" " + Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\screenshots.7z";
                                        un7z2.StartInfo.UseShellExecute = false;
                                        un7z2.StartInfo.RedirectStandardOutput = true;
                                        un7z2.StartInfo.CreateNoWindow = true;
                                        un7z2.Start();
                                        un7z2.WaitForExit();
                                        //MessageBox.Show(un7z.StandardOutput.ReadToEnd());
                                        MessageBox.Show("Actualización completada. Abra la aplicación de nuevo.", "Actualización", MessageBoxButton.OK, MessageBoxImage.Information);
                                        df.Close();
                                        return;
                                    }
                                } else
                                {
                                    MessageBox.Show("localjson is null. please delete json/launcher.json", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    Environment.Exit(-1);
                                    return;
                                }
                            }
                            else //if file does not exist, do not check for differences. just save
                            {
                                FileStream fs = new(Path.GetDirectoryName(Environment.ProcessPath) + "\\json\\launcher.json", FileMode.OpenOrCreate);
                                fs.SetLength(0);
                                fs.Flush();
                                fs.Write(Encoding.UTF8.GetBytes(readtoend), 0, readtoend.Length);
                                DownloadFile df = new();
                                df.Show();


                                FileStream ssfs = new(Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\" + json.game.uuid + ".7z", FileMode.OpenOrCreate);
                                ssfs.SetLength(0);
                                ssfs.Flush();
                                streamtask = httpclient.GetAsync(serverlocation + "/template/" + json.game.uuid + ".7z");
                                var str23 = httpclient.GetStreamAsync(serverlocation + "/template/" + json.game.uuid + ".7z");
                                //streamtask.Wait();
                                /*if (!streamtask.Result.IsSuccessStatusCode)
                                {
                                    MessageBox.Show("No se pudo actualizar el launcher." + Environment.NewLine + "Abra la aplicación para intentarlo de nuevo." + Environment.NewLine + "C", "Fallo en la actualización", MessageBoxButton.OK, MessageBoxImage.Error);
                                    File.Delete(Path.GetDirectoryName(Environment.ProcessPath) + "\\json\\launcher.json");
                                    df.Close();
                                    return;
                                }
                                if (!Path.Exists(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\")) //if web folder does not exist, create it
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\");
                                }
                                else
                                {
                                    if (Directory.GetFiles(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\").Length > 0)
                                    {
                                        foreach(string file in Directory.GetFiles(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\"))
                                        {
                                            File.Delete(file);
                                        }
                                    }
                                }*/
                                str23.Result.CopyTo(ssfs);
                                ssfs.Flush();
                                ssfs.Close();


                                Process un7z = new();
                                un7z.StartInfo.FileName = Path.GetDirectoryName(Environment.ProcessPath) + @"\7z\7zr.exe";
                                un7z.StartInfo.Arguments = "x -y -o\"" + Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\\" " + Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\" + json.game.uuid + ".7z";
                                un7z.StartInfo.UseShellExecute = false;
                                un7z.StartInfo.RedirectStandardOutput = true;
                                un7z.StartInfo.CreateNoWindow = true;
                                un7z.Start();
                                un7z.WaitForExit();
                                //MessageBox.Show(un7z.StandardOutput.ReadToEnd());

                                FileStream ssfs2 = new(Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\screenshots.7z", FileMode.OpenOrCreate);
                                ssfs2.SetLength(0);
                                ssfs2.Flush();
                                //streamtask = httpclient.GetAsync(serverlocation + "/screenshots/screenshots.7z");
                                var str232 = httpclient.GetStreamAsync(serverlocation + "/screenshots/screenshots.7z");
                                //streamtask.Wait();
                                /*if (!streamtask.Result.IsSuccessStatusCode)
                                {
                                    MessageBox.Show("No se pudo actualizar el launcher." + Environment.NewLine + "Abra la aplicación para intentarlo de nuevo." + Environment.NewLine + "C", "Fallo en la actualización", MessageBoxButton.OK, MessageBoxImage.Error);
                                    File.Delete(Path.GetDirectoryName(Environment.ProcessPath) + "\\json\\launcher.json");
                                    df.Close();
                                    return;
                                }
                                if (!Path.Exists(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\")) //if web folder does not exist, create it
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\");
                                }
                                else
                                {
                                    if (Directory.GetFiles(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\").Length > 0)
                                    {
                                        foreach(string file in Directory.GetFiles(Path.GetDirectoryName(Environment.ProcessPath) + "\\game\\"))
                                        {
                                            File.Delete(file);
                                        }
                                    }
                                }*/
                                str232.Result.CopyTo(ssfs2);
                                ssfs2.Flush();
                                ssfs2.Close();
                                Process un7z2 = new();
                                un7z2.StartInfo.FileName = Path.GetDirectoryName(Environment.ProcessPath) + @"\7z\7zr.exe";
                                un7z2.StartInfo.Arguments = "x -y -o\"" + Path.GetDirectoryName(Environment.ProcessPath) + "\\media\\img\\\" " + Path.GetDirectoryName(Environment.ProcessPath) + "\\web\\screenshots.7z";
                                un7z2.StartInfo.UseShellExecute = false;
                                un7z2.StartInfo.RedirectStandardOutput = true;
                                un7z2.StartInfo.CreateNoWindow = true;
                                un7z2.Start();
                                un7z2.WaitForExit();
                                //MessageBox.Show(un7z.StandardOutput.ReadToEnd());
                                MessageBox.Show("Actualización completada. Abra la aplicación de nuevo.", "Actualización", MessageBoxButton.OK, MessageBoxImage.Information);
                                df.Close();
                                return;
                            }
                        }
                    }
                } else
                {
                    MessageBox.Show("Se recibió una respuesta inesperada del servidor." + Environment.NewLine + "Abra la aplicación para intentarlo de nuevo.", "Error mientras se conectaba al servidor", MessageBoxButton.OK, MessageBoxImage.Error);
                    File.Delete(Path.GetDirectoryName(Environment.ProcessPath) + "\\json\\launcher.json");
                    Environment.Exit(-1);
                }
            } else
            {
                MessageBox.Show("No se pudo conectar con el servidor." + Environment.NewLine + "Abra la aplicación para intentarlo de nuevo.", "Error mientras se conectaba al servidor", MessageBoxButton.OK, MessageBoxImage.Error);
                File.Delete(Path.GetDirectoryName(Environment.ProcessPath) + "\\json\\launcher.json");
                Environment.Exit(-1);
            }

        }






    }
}
