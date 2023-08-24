using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class LocationItems
    {
        public int ID { get; set; }
        public int Percentage { get; set; }
        public int Quantity { get; set; }
        public bool Respawns { get; set; }
        public bool HasBeenCollected { get; set; }

        public LocationItems(int iD, int percentage, int quantity, bool respawns, bool collected)
        {
            ID = iD;
            Percentage = percentage;
            Quantity = quantity;
            Respawns = respawns;
            HasBeenCollected = collected;
        }
    }
}
