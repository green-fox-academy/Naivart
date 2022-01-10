using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Interfaces
{
    public interface IKingdomRepository : IRepository<Kingdom>
    {
        List<Kingdom> GetAll();
        Kingdom RenameKingdom(long kingdomId, string newKingdomName);
    }
}
