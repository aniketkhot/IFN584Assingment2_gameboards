namespace BoardGamesFramework
{
    public class HumanPlayer : Player
    {
        public HumanPlayer(string name)
        {
            Name = name;
            IsHuman = true;
        }

        public override Move GetMove(IGame game)
        {
            Console.WriteLine($"It's your turn, {Name}.");

            if (game is NumericalTicTacToe nt)
            {
                Console.Write($"{Name}, enter your move as: position(0-8) number(1-9): ");
                var parts = Console.ReadLine()?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts == null || parts.Length <= 1) return null;
                if (!int.TryParse(parts[0], out int pos) || !int.TryParse(parts[1], out int num))
                {
                    Console.WriteLine("Invalid input.");
                    return null;
                }
                return new NTTMove(pos, num);
            }

            else if (game is Notakto ntk)
            {
                Console.Write($"{Name}, enter your move as: boardIndex(0-2) position(0-8): ");
                var parts = Console.ReadLine()?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts == null || parts.Length <= 1) return null;
                if (!int.TryParse(parts[0], out int boardIndex) || !int.TryParse(parts[1], out int pos))
                {
                    Console.WriteLine("Invalid input.");
                    return null;
                }
                return new NotaktoMove(boardIndex, pos);
            }

            else if (game is Gomoku gm)
            {
                Console.Write($"{Name}, enter your move as: x(0-{gm.BoardSize - 1}) y(0-{gm.BoardSize - 1}): ");
                var parts = Console.ReadLine()?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts == null || parts.Length <= 1) return null;
                if (!int.TryParse(parts[0], out int x) || !int.TryParse(parts[1], out int y))
                {
                    Console.WriteLine("Invalid input.");
                    return null;
                }
                char piece = game.CurrentPlayer == game.Player1 ? 'X' : 'O';
                return new GomokuMove(x, y, piece);
            }

            Console.WriteLine("Unsupported game type.");
            return null;
        }
    }
}
