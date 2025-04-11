using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace PistachoStudiosLauncherWPF.JsonClasses
{
    public class LauncherJson
    {
        public LauncherGame game { get; set; } = new();
        public LauncherUpdateable screenshots { get; set; } = new();
        public int version { get; set; }
    }

    public class LauncherGame
    {
        public string name { get; set; } = "";
        public string instanceid { get; set; } = "";
        public LauncherUpdateable instance { get; set; } = new();
        public string version { get; set; } = "";
    }

    public class LauncherUpdateable
    {
        public int issuetimestamp { get; set; }
        public string uri { get; set; } = "";
    }

    public static class AccountsJsonHelper
    {
        public static string UsernameToAccountsJson(string username)
        {
            AccountsJson accsjson = new();
            accsjson.accounts = [new Account()];
            accsjson.accounts[0].active = true;
            AccountProfile accprofile = new();
            accprofile.id = a_mcofflineuuidgen(username);
            accprofile.name = username;
            accsjson.accounts[0].profile = accprofile;
            AccountYggdrasil accygg = new();
            AccountYggdrasilExtra accyggextra = new();
            accyggextra.clientToken = BitConverter.ToString(Guid.NewGuid().ToByteArray()).Replace("-", "").ToLower();
            accyggextra.userName = username;
            accygg.extra = accyggextra;
            accygg.iat = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            accsjson.accounts[0].ygg = accygg;
            return JsonSerializer.Serialize(accsjson);
        }

        static string a_mcofflineuuidgen(string username)
        {
            byte[] input = Encoding.UTF8.GetBytes("OfflinePlayer:" + username);
            MD5 md5 = MD5.Create();
            byte[] digest = MD5.HashData(input);
            digest[6] = (byte)((digest[6] & 0x0f) | 0x30); // set to version 3
            digest[8] = (byte)((digest[8] & 0x3f) | 0x80); // set to IETF variant
            return BitConverter.ToString(digest).Replace("-", "").ToLower();
        }
    }

    public class AccountsJson
    {
        public Account[]? accounts { get; set; }
        public string formatVersion { get; set; } = "3";
    }
    public class Account
    {
        public bool active { get; set; } = false;
        public AccountProfile? profile { get; set; }
        public string type { get; set; } = "Offline"; //DO NOT MODIFY
        public AccountYggdrasil? ygg { get; set; }

    }
    public class AccountProfile
    {
        //public string? cape { get; set; } //LEAVE AS NULL
        public AccountProfileCape[] capes { get; set; } = []; //DO NOT MODIFY
        public string? id { get; set; }
        public string? name { get; set; }
        public AccountProfileSkin skin { get; set; } = new AccountProfileSkin(); //DO NOT MODIFY

    }
    public class AccountProfileCape //DO NOT USE, LEAVE AccountProfileCape[] AS EMPTY ARRAY
    {
        public string? alias { get; set; }
        public string? id { get; set; }
        public string? url { get; set; }
    }
    public class AccountProfileSkin //LEAVE THIS AS IS, DO NOT CHANGE AFTERWARDS
    {
        public string id { get; set; } = "";
        public string url { get; set; } = "";
        public string variant { get; set; } = "";
    }
    public class AccountYggdrasil
    {
        public AccountYggdrasilExtra? extra { get; set; }
        public int? iat { get; set; }
        public string token { get; set; } = "0"; //DO NOT MODIFY

    }
    public class AccountYggdrasilExtra
    {
        public string? clientToken { get; set; }
        public string? userName { get; set; }
    }
}
