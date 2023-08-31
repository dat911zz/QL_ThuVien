using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_ThuVien.Models
{
    public class TestUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public TestUser()
        {

        }
        public TestUser(string name, string email, string pass)
        {
            Name = name;
            Email = email;
            Pass = pass;
        }
    }
}