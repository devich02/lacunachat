using System;

using System.Net;
using WebullApi;

using Lacuna;

namespace lacunachat_server
{
    class Program
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

                    try { } catch { }

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
