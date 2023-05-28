using System;
using Engine.Models;
using Engine.ViewModels;

namespace StoryRPG
{
    class Program
    {
        public enum GameStates { Start, Run, Quit};
        public static GameStates GameState = GameStates.Start;
        private static bool Run = true;

        static void Main(string[] args)
        {
            while(Run)
            {
                switch(GameState)
                {
                    case GameStates.Start:
                        StartState();
                        break;
                    case GameStates.Run:
                        RunState();
                        break;
                    case GameStates.Quit:
                        QuitState();
                        break;
                }
            }
        }

        private static void StartState()
        {

        }

        private static void RunState()
        {

        }
        private static void QuitState()
        {
            Run = false;
        }
    }

}