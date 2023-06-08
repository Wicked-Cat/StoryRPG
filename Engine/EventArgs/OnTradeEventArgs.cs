using Engine.Models;

namespace Engine.EventArgs
{
    public class OnTradeEventArgs : System.EventArgs
    {
        public Trade Trade { get; set; }
        public OnTradeEventArgs(Trade trade)
        {
            Trade = trade;
        }
    }
}
