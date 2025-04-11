using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualBasic;
//using Path = System.IO.Path;

namespace PistachoStudiosLauncherWPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PBUpdate : Window
    {
        public string hosturi = "https://elchehost.es";
        //public string hosturi = "https://magenta-stork.static.domains"; //test server
        public string _7zrexe_download_uri = "https://7-zip.org/a/7zr.exe";
        public string _7zrexe_path = AppDomain.CurrentDomain.BaseDirectory + "7z/7zr.exe";
        public string screenshotsdir = AppDomain.CurrentDomain.BaseDirectory + "media/img/";
        public string instancesdir = AppDomain.CurrentDomain.BaseDirectory + "prism/instances/";
        public string prismlauncher_cfg_path = AppDomain.CurrentDomain.BaseDirectory + "prism/prismlauncher.cfg";
        public string prismlauncherdir = AppDomain.CurrentDomain.BaseDirectory + "prism/";
        public string launcherjson_path = AppDomain.CurrentDomain.BaseDirectory + "launcher.json";
        public string prismlauncher_accountsjson_path = AppDomain.CurrentDomain.BaseDirectory + "prism/accounts.json";
        public string _7zdir = AppDomain.CurrentDomain.BaseDirectory + "7z/";
        public string _7zazip_download_uri = "https://github.com/ip7z/7zip/releases/download/24.09/7z2409-extra.7z";
        public string _7zazip_path = AppDomain.CurrentDomain.BaseDirectory + "7z2409-extra.7z";
        public string _7zaexe_path = AppDomain.CurrentDomain.BaseDirectory + "7z/7za.exe";
        public string prismlauncher_windows_MSVC_portable_9_2zip_download_uri = "https://github.com/Diegiwg/PrismLauncher-Cracked/releases/download/9.4/PrismLauncher-Windows-MinGW-w64-Portable-9.4.zip";

        public string prismlauncher_cfg = "[General]\r\nConfigVersion=1.2\r\nApplicationTheme=system\r\nIconTheme=pe_colored\r\nLanguage=es\r\nStatusBarVisible=true\r\nToolbarsLocked=false\r\n";

        public HttpClient client;
        public Utils.HttpHelper httpHelper;
        public Utils._7zrexeHelper _7zexe;
        public PBUpdate()
        {
            InitializeComponent();
            HttpClientHandler handler = new() { AllowAutoRedirect = true };
            ProgressMessageHandler phm = new(handler);
            phm.HttpReceiveProgress += Phm_HttpProgress;
            phm.HttpSendProgress += Phm_HttpProgress;
            client = new HttpClient(phm);
            httpHelper = new(client);
            _7zexe = new(_7zrexe_path);
        }
        private void PrintTextBox(string text)
        {
            textbox1.Text += text;
            textbox1.CaretIndex = textbox1.Text.Length - 1;
            textbox1.ScrollToEnd();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            await Task.Delay(3000);
            bool success = true;
            //create 7z directory if it doesn't exist
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "7z/"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "7z/");
            }
            //download 7zr if it doesn't exist
            if (!File.Exists(_7zrexe_path))
            {
                PrintTextBox("7zr.exe not found." + Environment.NewLine);
                PrintTextBox("GET " + _7zrexe_download_uri + "... ");
                progbar.IsIndeterminate = false;
                if (await httpHelper.GetAndStoreAsync(_7zrexe_download_uri, _7zrexe_path))
                {
                    PrintTextBox("OK" + Environment.NewLine);
                }
                else
                {
                    PrintTextBox("FAIL" + Environment.NewLine + "Status code is unsuccessful or connection timed out." + Environment.NewLine);
                    success = false;
                }
            }
            //download 7za and extract if it doesn't exist
            if (!File.Exists(_7zaexe_path) && success && File.Exists(_7zrexe_path))
            {
                PrintTextBox("7za.exe not found." + Environment.NewLine);
                PrintTextBox("GET " + _7zazip_download_uri + "... ");
                progbar.IsIndeterminate = false;
                if (await httpHelper.GetAndStoreAsync(_7zazip_download_uri, AppDomain.CurrentDomain.BaseDirectory + Utils.PathCheck(Path.GetFileName(new Uri(_7zazip_download_uri).AbsolutePath))))
                {
                    PrintTextBox("OK" + Environment.NewLine);
                    PrintTextBox("Extracting 7za.exe..." + Environment.NewLine);
                    progbar.IsIndeterminate = true;
                    Process proc = _7zexe.Extract7z(_7zazip_path, AppDomain.CurrentDomain.BaseDirectory + "7z/");
                    if (proc.ExitCode != 0)
                    {
                        PrintTextBox("Error extracting 7za.exe. 7zr Exit code: " + proc.ExitCode + Environment.NewLine + proc.StandardOutput.ReadToEnd() + Environment.NewLine + proc.StandardError.ReadToEnd());
                        success = false;
                    }
                    else
                    {
                        PrintTextBox("7za.exe extracted successfully." + Environment.NewLine);
                        _7zexe = new(_7zaexe_path);
                    }
                    progbar.IsIndeterminate = false;

                }
                else
                {
                    PrintTextBox("FAIL" + Environment.NewLine + "Status code is unsuccessful or connection timed out." + Environment.NewLine);
                    success = false;
                }
            }
            else
            {
                _7zexe = new(_7zaexe_path);
            }
            //download prismlauncher and extract it if it doesn't exist
            if (!Directory.Exists(prismlauncherdir))
            {
                Directory.CreateDirectory(prismlauncherdir);
                PrintTextBox("PrismLauncher not found." + Environment.NewLine);
                PrintTextBox("GET " + prismlauncher_windows_MSVC_portable_9_2zip_download_uri + "... ");
                if (await httpHelper.GetAndStoreAsync(prismlauncher_windows_MSVC_portable_9_2zip_download_uri, AppDomain.CurrentDomain.BaseDirectory + Utils.PathCheck(Path.GetFileName(new Uri(prismlauncher_windows_MSVC_portable_9_2zip_download_uri).AbsolutePath))))
                {
                    PrintTextBox("OK" + Environment.NewLine);
                    PrintTextBox("Extracting PrismLauncher..." + Environment.NewLine);
                    progbar.IsIndeterminate = true;
                    Process proc = _7zexe.Extract7z(AppDomain.CurrentDomain.BaseDirectory + Utils.PathCheck(Path.GetFileName(new Uri(prismlauncher_windows_MSVC_portable_9_2zip_download_uri).AbsolutePath)), prismlauncherdir);
                    if (proc.ExitCode != 0)
                    {
                        PrintTextBox("Error extracting PrismLauncher. 7za Exit code: " + proc.ExitCode + Environment.NewLine + proc.StandardOutput.ReadToEnd() + Environment.NewLine + proc.StandardError.ReadToEnd());
                        success = false;
                    }
                    else
                    {
                        PrintTextBox("PrismLauncher extracted successfully." + Environment.NewLine);
                    }
                }
                else
                {
                    PrintTextBox("FAIL" + Environment.NewLine + "Status code is unsuccessful or connection timed out." + Environment.NewLine);
                    success = false;
                }
            }

            PrintTextBox("Copying prismlauncher.cfg..." + Environment.NewLine);
            await File.WriteAllTextAsync(prismlauncher_cfg_path, prismlauncher_cfg);
            if(!File.Exists(prismlauncher_accountsjson_path))
            {
                PrintTextBox("Generating and copying accounts.json..." + Environment.NewLine);
                string username = "";
                while (username == "")
                {
                    username = Interaction.InputBox("Introduce tu nombre de usuario", "Prism Launcher");
                }
                File.WriteAllText(prismlauncher_accountsjson_path, JsonClasses.AccountsJsonHelper.UsernameToAccountsJson(username));
            }

            //if local launcher.json doesn't exist, download and parse it, then download the game instance and screenshots
            if (!File.Exists(launcherjson_path) && success)
            {
                PrintTextBox("Local launcher.json not found." + Environment.NewLine);
                PrintTextBox("GET " + hosturi + "/launcher.json" + "... ");
                progbar.IsIndeterminate = false;
                using (Stream? stream = await httpHelper.GetStreamIfSuccessAsync(hosturi + "/launcher.json"))
                {
                    if (stream != null) //if the status code is 2xx and ReadAsStreamAsync() returned a valid stream
                    {
                        PrintTextBox("OK" + Environment.NewLine);
                        using (FileStream fs = File.Open(launcherjson_path, FileMode.Create))
                        {
                            await stream.CopyToAsync(fs); //copy server json to local file
                            await fs.FlushAsync();
                        }
                        stream.Position = 0;
                        var server_launcherjson = JsonSerializer.Deserialize<JsonClasses.LauncherJson>(new StreamReader(stream).ReadToEnd());
                        if (server_launcherjson != null) //if parsing of server launcher.json was successful
                        {
                            //PrintTextBox("launcher.json fields: " + Environment.NewLine);
                            statuslabel.Content = "Descargando juego...";
                            PrintTextBox("GET " + server_launcherjson.game.instance.uri + "... ");
                            using (Stream? gamestream = await httpHelper.GetStreamIfSuccessAsync(server_launcherjson.game.instance.uri))
                            {
                                if (gamestream != null)
                                {
                                    PrintTextBox("OK" + Environment.NewLine);
                                    using (FileStream fs2 = File.Open(AppDomain.CurrentDomain.BaseDirectory + Utils.PathCheck(Path.GetFileName(new Uri(server_launcherjson.game.instance.uri).AbsolutePath)), FileMode.Create))
                                    {
                                        await gamestream.CopyToAsync(fs2);
                                        await fs2.FlushAsync();
                                    }
                                    PrintTextBox("Extracting game instance..." + Environment.NewLine);
                                    progbar.IsIndeterminate = true;
                                    Process proc = _7zexe.Extract7z(AppDomain.CurrentDomain.BaseDirectory + Utils.PathCheck(Path.GetFileName(new Uri(server_launcherjson.game.instance.uri).AbsolutePath)), instancesdir);
                                    //Extract the game instance
                                    if (proc.ExitCode != 0)
                                    {
                                        PrintTextBox("Error extracting game instance. 7zr Exit code: " + proc.ExitCode + Environment.NewLine + proc.StandardOutput.ReadToEnd() + Environment.NewLine + proc.StandardError.ReadToEnd());
                                        success = false;
                                    }
                                    else
                                    {
                                        PrintTextBox("Game instance extracted successfully." + Environment.NewLine);
                                    }
                                    progbar.IsIndeterminate = false;
                                    progbar.Value = 1;
                                }
                                else
                                {
                                    PrintTextBox("FAIL" + Environment.NewLine + "Status code in unsuccessful or connection timed out." + Environment.NewLine);
                                    success = false;
                                }
                            }
                            statuslabel.Content = "Descargando capturas...";
                            PrintTextBox("GET " + server_launcherjson.screenshots.uri + "... ");
                            using (Stream? screenshotstream = await httpHelper.GetStreamIfSuccessAsync(server_launcherjson.screenshots.uri))
                            {
                                if (screenshotstream != null)
                                {
                                    PrintTextBox("OK" + Environment.NewLine);
                                    foreach (string file in Directory.GetFiles(screenshotsdir))
                                    {
                                        File.Delete(file);
                                    }
                                    using (FileStream fs2 = File.Open(AppDomain.CurrentDomain.BaseDirectory + Utils.PathCheck(Path.GetFileName(new Uri(server_launcherjson.screenshots.uri).AbsolutePath)), FileMode.Create))
                                    {
                                        await screenshotstream.CopyToAsync(fs2);
                                        await fs2.FlushAsync();
                                    }
                                    PrintTextBox("Extracting screenshots..." + Environment.NewLine);
                                    progbar.IsIndeterminate = true;
                                    Process proc = _7zexe.Extract7z(AppDomain.CurrentDomain.BaseDirectory + Utils.PathCheck(Path.GetFileName(new Uri(server_launcherjson.screenshots.uri).AbsolutePath)), screenshotsdir);
                                    //Extract the game instance
                                    if (proc.ExitCode != 0)
                                    {
                                        PrintTextBox("Error extracting screenshots. 7zr Exit code: " + proc.ExitCode + Environment.NewLine + proc.StandardOutput.ReadToEnd() + Environment.NewLine + proc.StandardError.ReadToEnd());
                                        success = false;
                                    }
                                    else
                                    {
                                        PrintTextBox("Screenshots extracted successfully." + Environment.NewLine);
                                    }
                                    progbar.IsIndeterminate = false;
                                    progbar.Value = 1;
                                }
                                else
                                {
                                    PrintTextBox("FAIL" + Environment.NewLine + "Status code in unsuccessful or connection timed out." + Environment.NewLine);
                                    success = false;
                                }
                            }
                        }
                        else //if parsing of server launcher.json was not successful
                        {
                            PrintTextBox("Error parsing launcher.json" + Environment.NewLine);
                            success = false;
                        }
                    }
                    else //if the status code is not 2xx or ReadAsStreamAsync() returned null
                    {
                        PrintTextBox("FAIL" + Environment.NewLine + "Status code is unsuccessful or connection timed out." + Environment.NewLine);
                        success = false;
                    }
                }
            }
            //if local launcher.json exist, download server launcher.json and compare it with local launcher.json
            //if they are different, check if the game instance or screenshots issuetimestamps are different
            //if any are different, download the new game instance and/or screenshots and extract it/them
            if (File.Exists(launcherjson_path) && success) //if local launcher.json exists
            {
                using (FileStream fs = File.Open(launcherjson_path, FileMode.Open))
                {
                    var local_launcherjson = JsonSerializer.Deserialize<JsonClasses.LauncherJson>(fs);
                    if (local_launcherjson != null) //if parsing of local launcher.json was successful
                    {
                        PrintTextBox("GET " + hosturi + "/launcher.json" + "... ");
                        using (Stream? stream = await httpHelper.GetStreamIfSuccessAsync(hosturi + "/launcher.json"))
                        {
                            if (stream != null) //if the status code is 2xx and ReadAsStreamAsync() returned a valid stream
                            {
                                PrintTextBox("OK" + Environment.NewLine);
                                var server_launcherjson = JsonSerializer.Deserialize<JsonClasses.LauncherJson>(new StreamReader(stream).ReadToEnd());
                                if (server_launcherjson != null) //if parsing of server launcher.json was successful
                                {
                                    //CONTINUE HERE
                                    if (local_launcherjson != server_launcherjson)
                                    {
                                        //Check if game instance issuetimestamp is different
                                        if (local_launcherjson.game.instance.issuetimestamp != server_launcherjson.game.instance.issuetimestamp)
                                        {
                                            statuslabel.Content = "Actualizando juego...";
                                            PrintTextBox("GET " + server_launcherjson.game.instance.uri + "... ");
                                            using (Stream? gamestream = await httpHelper.GetStreamIfSuccessAsync(server_launcherjson.game.instance.uri))
                                            {
                                                if (gamestream != null)
                                                {
                                                    PrintTextBox("OK" + Environment.NewLine);
                                                    using (FileStream fs2 = File.Open(AppDomain.CurrentDomain.BaseDirectory + Utils.PathCheck(Path.GetFileName(new Uri(server_launcherjson.game.instance.uri).AbsolutePath)), FileMode.Create))
                                                    {
                                                        await gamestream.CopyToAsync(fs2);
                                                        await fs2.FlushAsync();
                                                    }
                                                    PrintTextBox("Extracting game instance..." + Environment.NewLine);
                                                    progbar.IsIndeterminate = true;
                                                    Process proc = _7zexe.Extract7z(AppDomain.CurrentDomain.BaseDirectory + Utils.PathCheck(Path.GetFileName(new Uri(server_launcherjson.game.instance.uri).AbsolutePath)), instancesdir);
                                                    //Extract the game instance
                                                    if (proc.ExitCode != 0)
                                                    {
                                                        PrintTextBox("Error extracting game instance. 7zr Exit code: " + proc.ExitCode + Environment.NewLine + proc.StandardOutput.ReadToEnd() + Environment.NewLine + proc.StandardError.ReadToEnd());
                                                        success = false;
                                                    }
                                                    else
                                                    {
                                                        PrintTextBox("Game instance extracted successfully." + Environment.NewLine);
                                                    }
                                                    progbar.IsIndeterminate = false;
                                                    progbar.Value = 1;
                                                }
                                                else
                                                {
                                                    PrintTextBox("FAIL" + Environment.NewLine + "Status code in unsuccessful or connection timed out." + Environment.NewLine);
                                                    success = false;
                                                }
                                            }
                                        }
                                        if (local_launcherjson.screenshots.issuetimestamp != server_launcherjson.screenshots.issuetimestamp)
                                        {
                                            statuslabel.Content = "Actualizando capturas...";
                                            PrintTextBox("GET " + server_launcherjson.screenshots.uri + "... ");
                                            using (Stream? screenshotstream = await httpHelper.GetStreamIfSuccessAsync(server_launcherjson.screenshots.uri))
                                            {
                                                if (screenshotstream != null)
                                                {
                                                    PrintTextBox("OK" + Environment.NewLine);
                                                    foreach (string file in Directory.GetFiles(screenshotsdir))
                                                    {
                                                        File.Delete(file);
                                                    }
                                                    using (FileStream fs2 = File.Open(AppDomain.CurrentDomain.BaseDirectory + Utils.PathCheck(Path.GetFileName(new Uri(server_launcherjson.screenshots.uri).AbsolutePath)), FileMode.Create))
                                                    {
                                                        await screenshotstream.CopyToAsync(fs2);
                                                        await fs2.FlushAsync();
                                                    }
                                                    PrintTextBox("Extracting screenshots..." + Environment.NewLine);
                                                    progbar.IsIndeterminate = true;
                                                    Process proc = _7zexe.Extract7z(AppDomain.CurrentDomain.BaseDirectory + Utils.PathCheck(Path.GetFileName(new Uri(server_launcherjson.screenshots.uri).AbsolutePath)), screenshotsdir);
                                                    //Extract the game instance
                                                    if (proc.ExitCode != 0)
                                                    {
                                                        PrintTextBox("Error extracting screenshots. 7zr Exit code: " + proc.ExitCode + Environment.NewLine + proc.StandardOutput.ReadToEnd() + Environment.NewLine + proc.StandardError.ReadToEnd());
                                                        success = false;
                                                    }
                                                    else
                                                    {
                                                        PrintTextBox("Screenshots extracted successfully." + Environment.NewLine);
                                                    }
                                                    progbar.IsIndeterminate = false;
                                                    progbar.Value = 1;
                                                }
                                                else
                                                {
                                                    PrintTextBox("FAIL" + Environment.NewLine + "Status code in unsuccessful or connection timed out." + Environment.NewLine);
                                                    success = false;
                                                }
                                            }
                                        }

                                    }
                                    else
                                    {
                                        //no updates, continue with normality
                                    }
                                    if (success)
                                    {
                                        fs.Close();
                                        File.Delete(launcherjson_path);
                                        using (FileStream fs3 = File.Open(launcherjson_path, FileMode.Create))
                                        {
                                            stream.Position = 0;
                                            await stream.CopyToAsync(fs3);
                                            await fs3.FlushAsync();
                                        }
                                    }
                                }
                                else //if parsing of server launcher.json was not successful
                                {
                                    PrintTextBox("Error parsing launcher.json" + Environment.NewLine);
                                    success = false;
                                }
                            }
                            else //if the status code is not 2xx or ReadAsStreamAsync() returned null
                            {
                                PrintTextBox("FAIL" + Environment.NewLine + "Status code is unsuccessful or connection timed out." + Environment.NewLine);
                                success = false;
                            }
                        }

                    }
                    else //if parsing of local launcher.json was not successful
                    {
                        File.Delete(launcherjson_path);
                        PrintTextBox("Error parsing local file launcher.json.");
                        MessageBox.Show("Error parsing local file launcher.json.", "Fatal error", MessageBoxButton.OK, MessageBoxImage.Error);
                        success = false;
                    }
                }
            }
            Cursor = Cursors.Arrow;
            //check if success was ever set to false. in that case, stop the program before opening the launcher
            if (!success)
            {
                MessageBox.Show("An error occurred and the program needs to be stopped. Please restart the application to try again.", "Fatal error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                PrintTextBox("Updated successfully." + Environment.NewLine);
                MainWindow mw = new();
                Application.Current.MainWindow = mw;
                Close();
                mw.Show();
            }
        }

        private void Phm_HttpProgress(object? sender, HttpProgressEventArgs e) //update progress bar according to current http transfer progress
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            { //run on the UI thread
                progbar.Value = ((double?)e.BytesTransferred / e.TotalBytes) ?? 0d;
            }));
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            this.Width = 488;
            this.Height = 131;
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            this.Width = 488;
            this.Height = 254;
        }
    }
}
