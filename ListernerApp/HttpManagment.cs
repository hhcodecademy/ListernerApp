using ListernerApp.Model;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ListernerApp
{
    internal class HttpManagment
    {
        public static async Task HandleLoginRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                string requestBody = await reader.ReadToEndAsync();
                Console.WriteLine($"Received request body: {requestBody}");

                // Configure JsonSerializer options
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Ensure case sensitivity
                };

                // Deserialize JSON to User object
                User loginUser = JsonSerializer.Deserialize<User>(requestBody, options);

                // Simple login check (for demonstration purposes)
                User user = DataStore.Users.Find(u => u.Username == loginUser.Username && u.Password == loginUser.Password);

                if (user != null)
                {
                    string responseString = $"Login successful. User ID: {user.Id} for user name {user.Username}";
                    setHttpResponse(response, HttpStatusCode.OK, responseString);

                }
                else
                {
                    string responseString = "Invalid username or password";
                    setHttpResponse(response, HttpStatusCode.Unauthorized, responseString);

                }
            }

            response.Close();
        }
        public static async Task HandleCreateUserRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                string requestBody = await reader.ReadToEndAsync();
                Console.WriteLine($"Received request body: {requestBody}");

                // Configure JsonSerializer options
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Ensure case sensitivity
                };

                // Deserialize JSON to User object
                User newUser = JsonSerializer.Deserialize<User>(requestBody, options);

                // Simple  check (for create purposes)
                User user = DataStore.Users.Find(u => u.Username == newUser.Username );

                if (user != null)
                {
                    string responseString = $"User exsist on Store. User ID: {user.Id} for user name {user.Username}";
                    setHttpResponse(response, HttpStatusCode.OK, responseString);

                }
                else
                {
                    var createdUser = DataStore.CreateUser(newUser);
                    string responseString = $"User succesfully created User ID: {createdUser.Id} for user name {createdUser.Username}";
                    setHttpResponse(response, HttpStatusCode.OK, responseString);

                }
            }

            response.Close();
        }

        public static async Task HandleBalanceRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (request.QueryString["Id"] != null && int.TryParse(request.QueryString["Id"], out int userId))
            {

                User user = DataStore.Users.Find(u => u.Id == userId);

                if (user != null)
                {
                    UserBalance userBalance = DataStore.Balances.Find(b => b.UserId == userId);

                    string responseString = $"User balance: {userBalance.Balance}";
                    setHttpResponse(response, HttpStatusCode.OK, responseString);
                }
                else
                {
                    string responseString = "User not found";
                    setHttpResponse(response, HttpStatusCode.NotFound, responseString);

                }
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            response.Close();
        }

        public static HttpListenerResponse setHttpResponse(HttpListenerResponse response, HttpStatusCode httpStatusCode, string responseString)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.StatusCode = (int)httpStatusCode;
            return response;
        }
    }
}
