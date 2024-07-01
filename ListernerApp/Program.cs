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
            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                if (request.Url.AbsolutePath == "/login/" && request.HttpMethod == "POST")
                {
                    await HandleLoginRequest(request, response);
                }
                else if (request.Url.AbsolutePath == "/balance" && request.HttpMethod == "GET")
                {
                    await HandleBalanceRequest(request, response);
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Close();
                }
            }
        }
        static async Task HandleLoginRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            using (var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding))
            {
                string requestBody = await reader.ReadToEndAsync();
                Console.WriteLine($"Received request body: {requestBody}");

                // Deserialize the request body into a User object
                User loginUser = JsonSerializer.Deserialize<User>(requestBody);

                var users = await GetUserAsync();

                // Simple login check (for demonstration purposes)
                User user = users.Find(u => u.Username == loginUser.Username && u.Password == loginUser.Password);

                if (user != null)
                {
                    string responseString = $"Login successful. User ID: {user.Id}";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    string responseString = "Invalid username or password";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
            }

            response.Close();
        }

        static async Task HandleBalanceRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (request.QueryString["Id"] != null && int.TryParse(request.QueryString["Id"], out int userId))
            {
                var users = await GetUserAsync();
                User user = users.Find(u => u.Id == userId);

                if (user != null)
                {
                    decimal balance =await GetBalanceAsync(user.Id);
                    string responseString = $"User balance: {balance}";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    string responseString = "User not found";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            response.Close();
        }

        public static async Task<decimal> GetBalanceAsync(int userId)
        {
            string path = "Data/Balance.txt";
            decimal currentBalance = 0;

            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        var balanceArray = line.Split(' ');
                        if (balanceArray.Length == 2)
                        {
                            if (userId == int.Parse(balanceArray[0]))
                            {
                                currentBalance = decimal.Parse(balanceArray[1]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
            }

            return currentBalance;
        }

        public static async Task<List<User>> GetUserAsync()
        {
            string path = "Data/Users.txt";
            List<User> users = new List<User>();

            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        var userArray = line.Split(' ');
                        if (userArray.Length == 3)
                        {
                            User user = new User
                            {
                                Id = int.Parse(userArray[0]),   
                                Username = userArray[1],
                                Password = userArray[2]
                            };
                            users.Add(user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
            }

            return users;
        }
    }
}

