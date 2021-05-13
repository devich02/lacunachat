using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Lacuna;
using Lacuna.Crypto;
using Newtonsoft.Json.Linq;
using WebullApi;

namespace lacunachat
{

    public static class Constants
    {
        public static readonly BigInt16 BaseKey =  Keys.FromPassword16(
                     ("{F6D4A5C7-F6C3-43A2-A3C8-63D6344D9ED2}"),
                     ("{69749AB8-6CB7-40E9-A433-E76809B08CD9}"),
                     ("{EAEA4B86-A083-48EC-BD3C-1E7E1EA26962}")
             );

        public static readonly AES BaseCryptor = new AES(BaseKey);

        public static readonly String BaseSalt = "{2A76115C-EE76-48C4-8F7A-5CEAFDD3E6A3}";
    }


    public class ChatServer
    {
        public String ip = "";
        public int port = 0;

        public ChatServer(String address)
        {
            var parts = address.Split(':');
            ip = parts[0];
            port = int.Parse(parts[1]);
        }


        public void Test()
        {
            TcpClient ping = new TcpClient(ip, port);

            ping.GetStream().SendEncryptedString(Constants.BaseCryptor, new JObject
            {
                ["type"] = "ping"
            }.ToString(Newtonsoft.Json.Formatting.None), new object());

            var responseStr = ping.GetStream().ReadEncryptedString(Constants.BaseCryptor);
            var response = JToken.Parse(responseStr);

            if (response["error"] != null) throw new Exception(response["error"].ToString());
        }

        public String Login(String username, String password)
        {
            TcpClient ping = new TcpClient(ip, port);

            var decryptKey = Keys.FromPassword128(password, Constants.BaseSalt, Constants.BaseKey.ToHexString());
            var testKey = Keys.FromPassword128(username, Constants.BaseSalt, password);

            ping.GetStream().SendEncryptedString(Constants.BaseCryptor, new JObject
            {
                ["type"] = "login/create",
                ["user"] = username,
                ["pass"] = decryptKey.ToHexString(),
                ["challenge"] = testKey.ToHexString()
            }.ToString(Newtonsoft.Json.Formatting.None), new object());

            var responseStr = ping.GetStream().ReadEncryptedString(Constants.BaseCryptor);
            var response = JToken.Parse(responseStr);

            if (response["error"] != null) throw new Exception(response["error"].ToString());

            return "";
        }

    }

}
