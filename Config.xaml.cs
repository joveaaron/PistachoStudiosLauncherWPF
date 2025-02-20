using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace PistachoStudiosLauncherWPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Config : Window
    {
        public JsonConfigRoot? jsonconfig = new();
        public Config()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(File.Exists(Path.GetDirectoryName(Environment.ProcessPath) + "\\json\\config.json")) //check for config.json
            {
                var jsonconfigfs = new FileStream(Path.GetDirectoryName(Environment.ProcessPath) + "\\json\\config.json", FileMode.OpenOrCreate);
                try
                {
                    jsonconfig = JsonSerializer.Deserialize<JsonConfigRoot>(jsonconfigfs);
                } catch (Exception)
                {
                    jsonconfigfs.Close();
                    MessageBox.Show("Error mientras se leía el archivo json/config.json." + Environment.NewLine + "Se reemplazaron los ajustes con los por defecto.", "Error de lectura", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                jsonconfigfs.Close();
                if (jsonconfig != null)
                {
                    usernametb.Text = jsonconfig.username;
                }
            }
            else //if not found, create it with default values
            {
                var jsonconfigfs = new FileStream(Path.GetDirectoryName(Environment.ProcessPath) + "\\json\\config.json", FileMode.OpenOrCreate);
                jsonconfigfs.SetLength(0);
                jsonconfigfs.Flush();
                byte[] bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize<JsonConfigRoot>(new()));
                jsonconfigfs.Write(bytes, 0, bytes.Length);
                jsonconfigfs.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            usernametb.Text = applyUsernameConventions(usernametb.Text);
            //MessageBox.Show(usernametb.Text);
            if (jsonconfig != null) 
            {
                jsonconfig.username = usernametb.Text;
                var jsonconfigfs = new FileStream(Path.GetDirectoryName(Environment.ProcessPath) + "\\json\\config.json", FileMode.OpenOrCreate);
                jsonconfigfs.SetLength(0);
                jsonconfigfs.Flush();
                byte[] bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(jsonconfig));
                jsonconfigfs.Write(bytes, 0, bytes.Length);
                jsonconfigfs.Close();
                DialogResult = true;
            }
            MessageBox.Show("Inicia el launcher de nuevo para continuar.");
            Environment.Exit(0);
        }

        public string applyUsernameConventions(string username) 
        {
            var originalusername = username;
            if (username.Length < 4)
            {
                while (username.Length < 3)
                {
                    username += "_";
                }
            }
            else if (username.Length > 16)
            {
                username = username[..16];
            }

            Regex rgx = new Regex("[^A-Za-z0-9_]");
            string rgxout = rgx.Replace(username, "_");
            if(originalusername != rgxout)
            {
                MessageBox.Show("Se cambió el nombre de usuario a " + rgxout + " por no respetar las siguientes normas:" + Environment.NewLine + "· Caracteres de la A a la Z (mayúsculas y minúsculas)" + Environment.NewLine + "· Caracteres numéricos (0-9)" + Environment.NewLine + "· Guión bajo (_)" + Environment.NewLine + "· El nombre de usuario debe de ser de 3 a 16 caracteres", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            return rgxout;
        }
    }
}
