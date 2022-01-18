using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Interfaces
{
    interface IAuthService
    {
        string GetToken(string username);
        string GetNameFromToken(string token);
    }
}
