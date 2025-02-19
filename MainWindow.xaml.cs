using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Versioning;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace PistachoStudiosLauncherWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer fadetimer = new(DispatcherPriority.Render);
        List<string> imagefilenames = [];
        int totransitionimage = 0;

        public MainWindow()
        {
            InitializeComponent();
            fadetimer.Interval = new(10000);
            fadetimer.Tick += fadeTimer_Tick;
            App app = (App)Application.Current;
            if (app.jsondata != null && app.jsondata.game != null && app.jsondata.game.modloader != null && app.jsondata.game.server != null)
            {
                if(app.jsondata.game.fancyname == null)
                {
                    gamelabel.Content = "Juego actual: " + app.jsondata.game.codename;
                } else {
                    gamelabel.Content = "Juego actual: " + app.jsondata.game.fancyname;
                }
            }
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            fadetimer.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
    }
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
    }

}