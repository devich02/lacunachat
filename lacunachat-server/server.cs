using System;

using System.Net;
using WebullApi;
using Lacuna;
using Lacuna.Crypto;

using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;

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
                    try { db.CreateTable<tables.AccessTokens>(); "Created table: AccessTokens".WriteLine(ConsoleColor.Green); } catch (Exception ex) { $"Table AccessTokens already exists ({ex.Message})".WriteLine(ConsoleColor.Yellow); }

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
                                        else if (request["type"].ToString() == "login/create")
                                        {
                                            String username = request["user"].ToString();
                                            String decryptPass = request["pass"].ToString();
                                            String challenge = request["challenge"].ToString();

                                            // todo: db needs locking mechanism
                                            // todo: db api needs sanitation 
                                            var user = db.ExecuteTo<tables.Users>($"Users.where(Name == \"{username}\")").ToList();


                                            var decryptKey = BigInt128.FromHex(decryptPass);
                                            var testKey = BigInt128.FromHex(challenge);
                                            AES decryptor = new AES(decryptKey);

                                            response["type"] = "login/create";

                                            // Create the user
                                            if (user.Count == 0)
                                            {
                                                var masterKey = BigInt128.Random();

                                                db.GetTable<tables.Users>()
                                                  .StreamingInsert(new List<tables.Users> { 
                                                      new tables.Users {
                                                          Name = username,
                                                          Key = Convert.ToHexString(decryptor.Encrypt(masterKey.ToByteArray())),
                                                          DecryptKey = Convert.ToHexString(decryptor.Encrypt(testKey.ToByteArray()))
                                                      }
                                                  });
                                            }
                                            // Log the user in
                                            else
                                            {
                                                if (testKey != new BigInt128(decryptor.Decrypt(Convert.FromHexString(user.First().DecryptKey))))
                                                {
                                                    response["error"] = "Invalid login/cannot create";
                                                }
                                                else
                                                {
                                                    response["friends"] = user.First().Friends;
                                                }
                                            }
                                        }
                                    }

                                    client.GetStream().SendEncryptedString(Constants.BaseCryptor, response.ToString(Newtonsoft.Json.Formatting.None), lockObj);
                                }
                                catch (Exception ex)
                                {
                                    ex.Message.ToString().WriteLine(ConsoleColor.Red);
                                }

                                client.Close();
                                client.Dispose();

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
