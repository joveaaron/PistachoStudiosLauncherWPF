using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PistachoStudiosLauncherWPF
{
    public class JsonRoot
    {
        public int ver { get; set; }

        public JsonGame? game { get; set; }
    }
    public class JsonGame
    {
        public string? uuid { get; set; }
        public string? codename { get; set; }
        public string? fancyname { get; set; }
        public string? ver { get; set; }
        public string? mc { get; set; }
        public JsonModloader? modloader { get; set; }
        public JsonServer? server { get; set; }
    }
    public class JsonModloader
    {
        public string? name { get; set; }
        public string? ver { get; set; }
    }
    public class JsonServer
    {
        public string? hostname { get; set; }
        public int gameport { get; set; }
    }
}
