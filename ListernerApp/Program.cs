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
            await ListnerManagment.StartListener();
        }



    }
}

