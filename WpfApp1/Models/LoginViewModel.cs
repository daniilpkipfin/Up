using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class LoginViewModel
    {
        public Users Login(string login, string password)
        {
            return Core.Context.Users
                .Include("Roles")
                .FirstOrDefault(u => u.Login == login && u.Password == password);
        }
    }
}
