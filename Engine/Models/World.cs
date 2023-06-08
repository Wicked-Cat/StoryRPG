using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class World
    {
        private List<Location> _locations = new List<Location>();

        internal void AddLocation(Location location)
        {
            _locations.Add(location);
        }

        public Location LocationAt(int x, int y, int z)
        {
            foreach(Location loc  in _locations)
            {
                if(loc.XCoordinate == x  && loc.YCoordinate == y && loc.ZCoordinate == z)
                    return loc;
            }
            return null;
        }
    }
}
