using Engine.Models;

namespace Engine.EventArgs
{
    public class OnChallengeEventArgs : System.EventArgs
    {
        public Challenge Challenge { get; set; }

        public OnChallengeEventArgs(Challenge challenge)
        {
            Challenge = challenge;
        }
    }
}
