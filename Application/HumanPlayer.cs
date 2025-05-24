namespace BoardGamesFramework
{
    // ----- Player Implementations -----
    public class HumanPlayer : Player
    {
        public HumanPlayer(string name)
        {
            Name = name;
            IsHuman = true;
        }

        public override Move GetMove(IGame game)
        {
            if (game is NumericalTicTacToe nt)
            {
                Console.WriteLine($"{Name}, enter your move as: position(0-8) number(1-9)");
                while (true)
                {
                    Console.Write("> ");
                    var input = Console.ReadLine();
                    var parts = input.Split(' ');
                    if (parts.Length == 2 &&
                        int.TryParse(parts[0], out int pos) &&
                        int.TryParse(parts[1], out int num))
                    {
                        var move = new NTTMove(pos, num);
                        if (nt.IsValidMove(move))
                            return move;
                        Console.WriteLine("Invalid move, try again.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input format.");
                    }
                }
            }
            else if (game is Notakto ntk)
            {
                Console.WriteLine($"{Name}, enter your move as: boardIndex(0-2) position(0-8)");
                while (true)
                {
                    Console.Write("> ");
                    var input = Console.ReadLine();
                    var parts = input.Split(' ');
                    if (parts.Length == 2 &&
                        int.TryParse(parts[0], out int boardIndex) &&
                        int.TryParse(parts[1], out int pos))
                    {
                        var move = new NotaktoMove(boardIndex, pos);
                        if (ntk.IsValidMove(move))
                            return move;
                        Console.WriteLine("Invalid move, try again.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input format.");
                    }
                }
            }
            else if (game is Gomoku gm)
            {
                Console.WriteLine($"{Name}, enter your move as: x(0-{gm.BoardSize - 1}) y(0-{gm.BoardSize - 1})");
                while (true)
                {
                    Console.Write("> ");
                    var input = Console.ReadLine();
                    var parts = input.Split(' ');
                    if (parts.Length == 2 &&
                        int.TryParse(parts[0], out int x) &&
                        int.TryParse(parts[1], out int y))
                    {
                        char piece = (game.CurrentPlayer == game.Player1) ? 'X' : 'O';
                        var move = new GomokuMove(x, y, piece);
                        if (gm.IsValidMove(move))
                            return move;
                        Console.WriteLine("Invalid move, try again.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input format.");
                    }
                }
            }
            else
            {
                throw new NotImplementedException("Unknown game type");
            }
        }
    }
}
