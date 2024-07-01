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
        private static readonly HttpListener listener = new HttpListener();
        private static readonly string URL = "http://localhost:5000/";

        public static async Task StartListener()
        {
            // Define the prefixes (URLs) the listener should handle
            listener.Prefixes.Add(URL);

            // Start the listener
            listener.Start();
            Console.WriteLine($"Listening for requests on {URL}");

            try
            {
                while (listener.IsListening)
                {
                    // Handle incoming requests
                    await HandleRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while handling requests: {ex.Message}");
            }
            finally
            {
                StopListener();
            }
        }



        private static async Task HandleRequest()
        {
            var context = await listener.GetContextAsync();
            HttpManagment management = new HttpManagment(context);
            string absolutePath = context.Request.Url.AbsolutePath;

            Console.WriteLine($"Received request for: {absolutePath}");

            try
            {
                switch (absolutePath)
                {
                    case "/login":
                        await management.HandleLoginRequest();
                        break;
                    case "/create":
                        await management.HandleCreateUserRequest();
                        break;
                    case "/balance":
                        await management.HandleBalanceRequest();
                        break;
                    default:
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Endpoint not found"));
                        context.Response.Close();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while handling request: {ex.Message}");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Internal server error"));
                context.Response.Close();
            }
        }

        public static void StopListener()
        {
            listener.Stop();
            Console.WriteLine("Listener stopped.");
        }
    }
}
