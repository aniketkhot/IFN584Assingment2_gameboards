namespace BoardGamesFramework
{
    public class ComputerPlayer : Player
    {
        public ComputerPlayer(string name)
        {
            Name = name;
            IsHuman = false;
        }

        public override Move GetMove(IGame game)
        {
            if (game is NumericalTicTacToe nt)
            {
                for (int pos = 0; pos < 9; pos++)
                {
                    for (int num = 1; num <= 9; num++)
                    {
                        if (nt.CurrentPlayer == nt.Player1 && num % 2 == 0) continue;
                        if (nt.CurrentPlayer == nt.Player2 && num % 2 == 1) continue;
                        var move = new NTTMove(pos, num);
                        if (!nt.IsValidMove(move)) continue;
                        var clone = (NumericalTicTacToe)nt.Clone();
                        clone.MakeMove(move);
                        if (clone.IsGameOver && clone.Winner == this)
                            return move;
                    }
                }

                var validMoves = new List<NTTMove>();
                for (int pos = 0; pos < 9; pos++)
                {
                    for (int num = 1; num <= 9; num++)
                    {
                        if (nt.CurrentPlayer == nt.Player1 && num % 2 == 0) continue;
                        if (nt.CurrentPlayer == nt.Player2 && num % 2 == 1) continue;
                        var move = new NTTMove(pos, num);
                        if (nt.IsValidMove(move)) validMoves.Add(move);
                    }
                }

                if (validMoves.Count == 0) return null;
                var rnd = new Random();
                return validMoves[rnd.Next(validMoves.Count)];
            }

            else if (game is Notakto ntk)
            {
                var validMoves = new List<NotaktoMove>();
                for (int b = 0; b < 3; b++)
                {
                    for (int pos = 0; pos < 9; pos++)
                    {
                        var move = new NotaktoMove(b, pos);
                        if (ntk.IsValidMove(move)) validMoves.Add(move);
                    }
                }

                if (validMoves.Count == 0) return null;
                var rnd = new Random();
                return validMoves[rnd.Next(validMoves.Count)];
            }

            else if (game is Gomoku gm)
            {
                var validMoves = new List<GomokuMove>();
                for (int x = 0; x < gm.BoardSize; x++)
                {
                    for (int y = 0; y < gm.BoardSize; y++)
                    {
                        var move = new GomokuMove(x, y, gm.CurrentPlayer == gm.Player1 ? 'X' : 'O');
                        if (gm.IsValidMove(move)) validMoves.Add(move);
                    }
                }

                if (validMoves.Count == 0) return null;
                var rnd = new Random();
                return validMoves[rnd.Next(validMoves.Count)];
            }

            Console.WriteLine("Unsupported game type.");
            return null;
        }
    }
}
