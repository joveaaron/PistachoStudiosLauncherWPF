using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.VisualBasic;
using PistachoStudiosLauncherWPF.JsonClasses;

namespace PistachoStudiosLauncherWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer fadetimer = new(DispatcherPriority.Render);
        DispatcherTimer nextphoto = new(DispatcherPriority.Render);
        List<string> imagefilenames = [];
        int totransitionimage = 0;

        public string launcherjson_path = AppDomain.CurrentDomain.BaseDirectory + "launcher.json";
        public string prismlauncher_accountsjson_path = AppDomain.CurrentDomain.BaseDirectory + "prism/accounts.json";

        LauncherJson? ljson = new();

        public MainWindow()
        {
            InitializeComponent();
            fadetimer.Interval = new(10000);
            fadetimer.Tick += fadeTimer_Tick;
            nextphoto.Interval = new(0, 0, 20);
            nextphoto.Tick += Nextphoto_Tick;
            ljson = JsonSerializer.Deserialize<LauncherJson>(File.ReadAllText(launcherjson_path));
            if(ljson != null)
            {
                gamelabel.Content = "InstanceID: " + ljson.game.instanceid;
                playbtn.Content = "Jugar a " + ljson.game.name + Environment.NewLine + ljson.game.version;
            } else
            {
                ljson = new();
                Application.Current.Shutdown();
            }
        }

        private void Nextphoto_Tick(object? sender, EventArgs e)
        {
            fadetimer.Start();
        }
        private void fadeTimer_Tick(object? sender, EventArgs e)
        {
            if(frntimage.Opacity > 0)
            {
                frntimage.Opacity -= 0.005;
                
            } else
            {
                fadetimer.Stop();
                
                frntimage.Source = backimage.Source;
                frntimage.Opacity = 1;
                totransitionimage++;
                if(imagefilenames.Count == totransitionimage)
                {
                    totransitionimage = 0;
                }
                backimage.Source = Utils.ToImageSource(System.Drawing.Image.FromFile(imagefilenames[totransitionimage]), ImageFormat.Png);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bkgcyclecheckbox.IsChecked = true;
            nextphoto.Start();
            
            string? path = Path.GetDirectoryName(Environment.ProcessPath);
            if (path != null)
            {
                string[] files = Directory.GetFiles(path + "\\media\\img\\");
                for (int i = 0; i < files.Length; i++)
                {
                    if (Path.GetExtension(files[i]).Equals(".png", StringComparison.InvariantCultureIgnoreCase))
                    {
                        imagefilenames.Add(files[i]);
                    }
                }
                //MessageBox.Show(imagefilenames.Length.ToString());
                Utils.Shuffle(imagefilenames);
            }
            frntimage.Source = Utils.ToImageSource(System.Drawing.Image.FromFile(imagefilenames[0]), ImageFormat.Png);
            totransitionimage = 1;
            backimage.Source = Utils.ToImageSource(System.Drawing.Image.FromFile(imagefilenames[1]), ImageFormat.Png);
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (bkgcyclecheckbox.IsChecked == true)
            {
                nextphoto.Start();
            }
            else
            {
                nextphoto.Stop();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) //config button
        {
            string result = Interaction.InputBox("Presiona cancelar para mantener el anterior.", "Cambiar nombre de usuario", "usuario_epico");
            if(result != string.Empty && result != "")
            {
                File.WriteAllText(prismlauncher_accountsjson_path, AccountsJsonHelper.UsernameToAccountsJson(result));
            }
        }
        private void playbtn_Click(object sender, RoutedEventArgs e)
        {
            Process proc = new();
            proc.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "prism/prismlauncher.exe";
            proc.StartInfo.Arguments = "-l " + ljson.game.instanceid; //warning can be safely ignored.
            proc.StartInfo.UseShellExecute = true;
            proc.Start();
            Cursor = Cursors.Wait;
            Thread.Sleep(5000);
            Application.Current.Shutdown();
        }
    }
}