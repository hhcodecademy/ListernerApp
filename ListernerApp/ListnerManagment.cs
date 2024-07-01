using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ListernerApp
{
    internal class ListnerManagment
    {
        public static async Task StartListener()
        {
            // Create a new HttpListener instance
            HttpListener listener = new HttpListener();

            // Define the prefixes (URLs) the listener should handle
            listener.Prefixes.Add("http://localhost:5000/");

            // Start the listener
            listener.Start();
            Console.WriteLine("Listening for requests on http://localhost:5000/");

            // Handle incoming requests
            await HandleRequest(listener);
        }
        private static async Task HandleRequest(HttpListener listener)
        {
            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();

                HttpManagment managment = new HttpManagment(context);

                string absolutePath = context.Request.Url.AbsolutePath;
                switch (absolutePath)
                {
                    case "/login":
                        await managment.HandleLoginRequest();
                        break;
                    case "/create":
                        await managment.HandleCreateUserRequest();
                        break;
                    case "/balance":
                        await managment.HandleBalanceRequest();
                        break;
                    default:
                        break;
                }

            }

        }
    }
}
