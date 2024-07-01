using ListernerApp.Model;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ListernerApp
{
    internal class HttpManagment
    {
        private readonly HttpListenerRequest request;
        private readonly HttpListenerResponse response;

        public HttpManagment(HttpListenerContext context)
        {
            request = context.Request;
            response = context.Response;
        }

        public async Task HandleLoginRequest()
        {
            try
            {
                string requestBody = await ReadRequestBodyAsync();
                User loginUser = DeserializeJson<User>(requestBody);

                User user = DataStore.Users.Find(u => u.Username == loginUser.Username && u.Password == loginUser.Password);

                if (user != null)
                {
                    string responseString = $"Login successful. User ID: {user.Id} for user name {user.Username}";
                    SetHttpResponse(HttpStatusCode.OK, responseString);
                }
                else
                {
                    SetHttpResponse(HttpStatusCode.Unauthorized, "Invalid username or password");
                }
            }
            catch (Exception ex)
            {
                SetHttpResponse(HttpStatusCode.InternalServerError, $"Error processing request: {ex.Message}");
            }
            finally
            {
                response.Close();
            }
        }

        public async Task HandleCreateUserRequest()
        {
            try
            {
                string requestBody = await ReadRequestBodyAsync();
                User newUser = DeserializeJson<User>(requestBody);

                User user = DataStore.Users.Find(u => u.Username == newUser.Username);

                if (user != null)
                {
                    string responseString = $"User exists on Store. User ID: {user.Id} for user name {user.Username}";
                    SetHttpResponse(HttpStatusCode.OK, responseString);
                }
                else
                {
                    var createdUser = DataStore.CreateUser(newUser);
                    string responseString = $"User successfully created. User ID: {createdUser.Id} for user name {createdUser.Username}";
                    SetHttpResponse(HttpStatusCode.OK, responseString);
                }
            }
            catch (Exception ex)
            {
                SetHttpResponse(HttpStatusCode.InternalServerError, $"Error processing request: {ex.Message}");
            }
            finally
            {
                response.Close();
            }
        }

        public async Task HandleBalanceRequest()
        {
            try
            {
                if (request.QueryString["Id"] != null && int.TryParse(request.QueryString["Id"], out int userId))
                {
                    User user = DataStore.Users.Find(u => u.Id == userId);

                    if (user != null)
                    {
                        UserBalance userBalance = DataStore.Balances.Find(b => b.UserId == userId);
                        string responseString = $"User balance: {userBalance.Balance}";
                        SetHttpResponse(HttpStatusCode.OK, responseString);
                    }
                    else
                    {
                        SetHttpResponse(HttpStatusCode.NotFound, "User not found");
                    }
                }
                else
                {
                    SetHttpResponse(HttpStatusCode.BadRequest, "Invalid or missing user ID");
                }
            }
            catch (Exception ex)
            {
                SetHttpResponse(HttpStatusCode.InternalServerError, $"Error processing request: {ex.Message}");
            }
            finally
            {
                response.Close();
            }
        }

        private async Task<string> ReadRequestBodyAsync()
        {
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private T DeserializeJson<T>(string jsonString)
        {
            return JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        private void SetHttpResponse(HttpStatusCode httpStatusCode, string responseString)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.StatusCode = (int)httpStatusCode;
        }
    }

  
}
