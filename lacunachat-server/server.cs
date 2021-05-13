using System;

using System.Net;
using WebullApi;
using Lacuna;
using Lacuna.Crypto;

using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace lacunachat
{
    class server
    {
        static void Help()
        {
            @"
+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

lacunachat-server

    --db-name <addr>          | The server name hosting the chat db
    --db-port [<port>=45864]  | Optional. The server port number
    
    --port [<port>=43393]     | Optional. The chat server port number

+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
".Print();
        }
        static void Main(string[] vargs)
        {
            Console.WriteLine();


            ArgParser args = new ArgParser(vargs);

            if (args["--help"] != null)
            {
                Help();
                return;
            }

            int port = 43393;
            if (args["--port"] != null)
            {
                try
                {
                    port = int.Parse(args["--port"][0]);
                }
                catch(Exception ex)
                {
                    "Invalid port specification".WriteLine(ConsoleColor.Yellow);
                    ex.Message.WriteLine(ConsoleColor.Red);
                }
            }

            if (args["--db-name"] == null
                || args["--db-name"].Count == 0)
            {
                "--db-name required".WriteLine(ConsoleColor.Red);
                Help();
                return;
            }



            int dbport = 45864;
            string dbserver = args["--db-name"][0];

            if (args["--db-port"] != null)
            {
                try
                {
                    dbport = int.Parse(args["--db-port"][0]);
                }
                catch (Exception ex)
                {
                    "Invalid db port specification".WriteLine(ConsoleColor.Yellow);
                    ex.Message.WriteLine(ConsoleColor.Red);
                }
            }

            try
            {
                using (var db = new Connection(dbserver, (ushort)dbport))
                {

                    try { db.CreateTable<tables.Users>(); "Created table: Users".WriteLine(ConsoleColor.Green); } catch (Exception ex) { $"Table Users already exists ({ex.Message})".WriteLine(ConsoleColor.Yellow); }
                    try { db.CreateTable<tables.Messages>(); "Created table: Messages".WriteLine(ConsoleColor.Green); } catch (Exception ex) { $"Table Messages already exists ({ex.Message})".WriteLine(ConsoleColor.Yellow); }

                    TcpListener server = new TcpListener(IPAddress.Any, port);

                    server.Start();
                    "Listening..".WriteLine(ConsoleColor.Green);
                    while (true)
                    {
                        try
                        {
                            TcpClient client = server.AcceptTcpClient();

                            (new Task(() => {

                                object lockObj = new object();

                                try
                                {
                                    var requestStr = client.GetStream().ReadEncryptedString(Constants.BaseCryptor);
                                    var request = JToken.Parse(requestStr);

                                    $"New request from {client.Client.RemoteEndPoint} : {request}".WriteLine(ConsoleColor.Green);

                                    JObject response = new JObject();

                                    if (request["type"] == null) response["error"] = "Missing type";
                                    else
                                    {
                                        if (request["type"].ToString() == "ping") { response["type"] = "pong"; }
                                    }

                                    client.GetStream().SendEncryptedString(Constants.BaseCryptor, response.ToString(Newtonsoft.Json.Formatting.None), lockObj);

                                    client.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    ex.Message.ToString().WriteLine(ConsoleColor.Red);
                                }
                            
                            })).Start();
                        }
                        catch (Exception ex)
                        {
                            ex.ToString().WriteLine(ConsoleColor.Red);
                        }
                    }



                }
            }
            catch (Exception ex)
            {
                "Failed to connect to db".WriteLine(ConsoleColor.Yellow);
                ex.Message.WriteLine(ConsoleColor.Red);
            }

        }
    }
}
