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

                // Deserialize the request body into a User object
                User loginUser = JsonSerializer.Deserialize<User>(requestBody);


                // Simple login check (for demonstration purposes)
                User user = DataStore.Users.Find(u => u.Username == loginUser.Username && u.Password == loginUser.Password);

                if (user != null)
                {
                    string responseString = $"Login successful. User ID: {user.Id}";
                    response = setHttpResponse(response, responseString);
                    response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    string responseString = "Invalid username or password";
                    response = setHttpResponse(response, responseString);
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
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
                    response = setHttpResponse(response, responseString);
                    response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    string responseString = "User not found";
                    response = setHttpResponse(response, responseString);
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            response.Close();
        }

        public static HttpListenerResponse setHttpResponse(HttpListenerResponse response, string responseString)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            return response;
        }
    }
}
