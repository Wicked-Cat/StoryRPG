using System;

namespace Engine
{
    public static class RandomNumberGenerator
    {
        private static readonly Random _generator = new Random();

        public static int NumberBetween(int min, int max)
        {
            return _generator.Next(min, max +1);
        }
    }
}
