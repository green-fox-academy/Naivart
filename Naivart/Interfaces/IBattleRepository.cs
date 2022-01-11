using Naivart.Models.Entities;
using System.Collections.Generic;

namespace Naivart.Interfaces
{
    public interface IBattleRepository: IRepository<Battle>
    {
        List<Battle> Battles(long kingdomId);
        Kingdom Defender(long defenderId);
        Kingdom Attacker(long attackerId);
        void UpdateTroops(List<Troop> troops);
    }
}
