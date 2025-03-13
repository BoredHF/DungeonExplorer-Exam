using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonExplorer.Managers.Game;

namespace DungeonExplorer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Single instance of the game, and will most likely be used to create a character creation menu

            Game game = new Game();
            game.Start();
        }
    }
}
