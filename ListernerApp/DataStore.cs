using ListernerApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListernerApp
{
    internal class DataStore
    {
        public static List<User> Users { get; private set; }
        public static List<UserBalance> Balances { get; private set; }
        static DataStore()
        {
            Users = new List<User>();
            Balances = new List<UserBalance>();
            SeedData();
        }
        public static void SeedData()
        {
            Users.Add(new User { Id = 1, Username = "admin", Password = "123" });
            Users.Add(new User { Id = 2, Username = "mikayil", Password = "789" });
            Users.Add(new User { Id = 3, Username = "nicat", Password = "456" });

            Balances.Add(new UserBalance { UserId = 1, Balance = 2000 });
            Balances.Add(new UserBalance { UserId = 2, Balance = 3000 });
            Balances.Add(new UserBalance { UserId = 3, Balance = 4000 });
        }

        public static User CreateUser(User user)
        {
            var newUser = new User { Id = Users.Count()+1, Username = user.Username, Password = user.Password };
            Users.Add(newUser);
            return newUser;
        }
    }
}

