using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Models.APIModels
{
    public class RegisterRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public string kingdomName { get; set; }
    }
}
