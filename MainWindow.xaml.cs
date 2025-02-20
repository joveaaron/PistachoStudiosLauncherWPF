﻿using System.Drawing.Imaging;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

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
        string gamename = "";
        JsonConfigRoot? jsonconfig = null;

        public MainWindow()
        {
            InitializeComponent();
            fadetimer.Interval = new(10000);
            fadetimer.Tick += fadeTimer_Tick;
            nextphoto.Interval = new(0, 0, 20);
            nextphoto.Tick += Nextphoto_Tick;
            App app = (App)Application.Current;
            if (app.jsondata != null && app.jsondata.game != null && app.jsondata.game.modloader != null && app.jsondata.game.server != null)
            {
                if(app.jsondata.game.fancyname == null && app.jsondata.game.codename != null)
                {
                    gamename = app.jsondata.game.codename;
                } else if (app.jsondata.game.fancyname != null) {
                    gamename = app.jsondata.game.fancyname;
                }
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

        private void Button_Click(object sender, RoutedEventArgs e) //photo button
        {
            fadetimer.Start();
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

            var jsonconfigfs = new FileStream(Path.GetDirectoryName(Environment.ProcessPath) + "\\json\\config.json", FileMode.OpenOrCreate);
            try
            {
                jsonconfig = JsonSerializer.Deserialize<JsonConfigRoot>(jsonconfigfs);
                jsonconfigfs.Close();
            }
            catch (Exception)
            {
                jsonconfigfs.Close();
                //MessageBox.Show("Error mientras se leía el archivo json/config.json." + Environment.NewLine + "Se reemplazaron los ajustes con los por defecto.", "Error de lectura", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (jsonconfig != null && jsonconfig.username != null)
            {
                updateLabelStatusBar(gamename, jsonconfig.username);
            } else
            {
                MessageBox.Show("La configuración es invalida. Cámbiala ahora.", "Fallo en la configuración", MessageBoxButton.OK, MessageBoxImage.Error);
                Config config = new();
                config.ShowDialog();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) //config button
        {
            Config config = new();
            config.ShowDialog();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(bkgcyclecheckbox.IsChecked == true)
            {
                nextphoto.Start();
            } else
            {
                nextphoto.Stop();
            }
        }
        public void updateLabelStatusBar(string game, string username)
        {
            gamelabel.Content = "Juego actual: " + game + ". Nombre de usuario: " + username;
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