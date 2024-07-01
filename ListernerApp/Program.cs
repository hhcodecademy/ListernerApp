using ListernerApp.Model;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ListernerApp
{
    internal class Program
    {
        static async Task Main(string[] args)
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

        static async Task HandleRequest(HttpListener listener)
        {
            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                string absolutePath = request.Url.AbsolutePath;
                switch (absolutePath)
                {
                    case "/login":
                        await HttpManagment.HandleLoginRequest(request, response);
                        break;
                    case "/balance":
                        await HttpManagment.HandleBalanceRequest(request, response);
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        response.Close();
                        break;
                }


            }
        }




    }
}

