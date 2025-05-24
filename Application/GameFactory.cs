using BoardGamesFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class GameFactory
    {
        public static IGame Create(string choice, Player p1, Player p2)
        {
            return choice switch
            {
                "1" => new NumericalTicTacToe(p1, p2),
                "2" => new Notakto(p1, p2),
                "3" => new Gomoku(p1, p2),
                _ => throw new ArgumentException("Invalid game type")
            };
        }
    }

}
