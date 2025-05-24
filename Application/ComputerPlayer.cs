namespace BoardGamesFramework
{
    public class ComputerPlayer : Player
    {
        private Random rand = new Random();

        public ComputerPlayer(string name)
        {
            Name = name;
            IsHuman = false;
        }

        public override Move GetMove(IGame game)
        {
            if (game is NumericalTicTacToe nt)
            {
                // Try immediate winning move
                for (int pos = 0; pos < 9; pos++)
                    for (int num = 1; num <= 9; num++)
                    {
                        var move = new NTTMove(pos, num);
                        if (!nt.IsValidMove(move))
                            continue;
                        var clone = (NumericalTicTacToe)nt.Clone();
                        clone.MakeMove(move);
                        if (clone.IsGameOver && clone.Winner == this)
                        {
                            Console.WriteLine($"{Name} plays winning move at pos {pos} number {num}");
                            return move;
                        }
                    }
                // Else random move
                List<NTTMove> validMoves = new List<NTTMove>();
                for (int pos = 0; pos < 9; pos++)
                    for (int num = 1; num <= 9; num++)
                    {
                        var move = new NTTMove(pos, num);
                        if (nt.IsValidMove(move))
                            validMoves.Add(move);
                    }
                if (validMoves.Count == 0) return null;
                var chosen = validMoves[rand.Next(validMoves.Count)];
                Console.WriteLine($"{Name} plays random move at pos {chosen.Position} number {chosen.NumberPlaced}");
                return chosen;
            }
            else if (game is Notakto ntk)
            {
                List<NotaktoMove> validMoves = new List<NotaktoMove>();
                for (int b = 0; b < 3; b++)
                    for (int pos = 0; pos < 9; pos++)
                    {
                        var move = new NotaktoMove(b, pos);
                        if (ntk.IsValidMove(move))
                            validMoves.Add(move);
                    }
                if (validMoves.Count == 0) return null;
                var chosen = validMoves[rand.Next(validMoves.Count)];
                Console.WriteLine($"{Name} plays move on board {chosen.BoardIndex} pos {chosen.Position}");
                return chosen;
            }
            else if (game is Gomoku gm)
            {
                List<GomokuMove> validMoves = new List<GomokuMove>();
                char piece = (game.CurrentPlayer == game.Player1) ? 'X' : 'O';
                for (int x = 0; x < gm.BoardSize; x++)
                    for (int y = 0; y < gm.BoardSize; y++)
                    {
                        var move = new GomokuMove(x, y, piece);
                        if (gm.IsValidMove(move))
                            validMoves.Add(move);
                    }
                if (validMoves.Count == 0) return null;
                var chosen = validMoves[rand.Next(validMoves.Count)];
                Console.WriteLine($"{Name} plays move at ({chosen.X},{chosen.Y})");
                return chosen;
            }
            else
            {
                throw new NotImplementedException("Unknown game type");
            }
        }
    }
}
