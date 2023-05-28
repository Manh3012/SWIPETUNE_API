using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BusinessObject.Sub_Model
{
    public class RegisterAccountModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime DOB { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }

    }
}
