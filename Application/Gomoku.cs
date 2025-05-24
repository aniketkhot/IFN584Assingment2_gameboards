using System.Text.Json;


namespace BoardGamesFramework
{
    public class Gomoku : GameBase
    {
        public override string GameName => "Gomoku";

        public int BoardSize;
        private char[,] Board;
        private bool gameOverFlag = false;
        private Player winner = null;

        public Gomoku(Player p1, Player p2, int boardSize = 15)
        {
            BoardSize = boardSize;
            Board = new char[BoardSize, BoardSize];
            Player1 = p1;
            Player2 = p2;
            CurrentPlayer = Player1;
            Reset();
        }

        public void Reset()
        {
            for (int i = 0; i < BoardSize; i++)
                for (int j = 0; j < BoardSize; j++)
                    Board[i, j] = '.';
            UndoStack.Clear();
            RedoStack.Clear();
            gameOverFlag = false;
            winner = null;
            CurrentPlayer = Player1;
        }

        public override bool IsGameOver => gameOverFlag;
        public override Player Winner => winner;

        public override bool IsValidMove(Move move)
        {
            if (!(move is GomokuMove m))
                return false;
            if (m.X < 0 || m.X >= BoardSize || m.Y < 0 || m.Y >= BoardSize)
                return false;
            if (Board[m.X, m.Y] != '.')
                return false;
            if (m.Piece != 'X' && m.Piece != 'O')
                return false;
            if (CurrentPlayer == Player1 && m.Piece != 'X') return false;
            if (CurrentPlayer == Player2 && m.Piece != 'O') return false;
            return true;
        }

        public override void MakeMove(Move move)
        {
            if (!(move is GomokuMove m))
                throw new ArgumentException("Invalid move type");
            if (!IsValidMove(m))
                throw new InvalidOperationException("Invalid move");

            Board[m.X, m.Y] = m.Piece;
            UndoStack.Push(move);
            RedoStack.Clear();
            CheckGameOver(m.X, m.Y);
            if (!gameOverFlag)
                CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
        }

        public override void Undo()
        {
            if (UndoStack.Count == 0)
            {
                Console.WriteLine("Nothing to undo.");
                return;
            }
            var lastMove = (GomokuMove)UndoStack.Pop();
            Board[lastMove.X, lastMove.Y] = '.';
            RedoStack.Push(lastMove);
            gameOverFlag = false;
            winner = null;
            CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
            Console.WriteLine($"Undo move at ({lastMove.X},{lastMove.Y}) by {CurrentPlayer.Name}");
        }

        public override void Redo()
        {
            if (RedoStack.Count == 0)
            {
                Console.WriteLine("Nothing to redo.");
                return;
            }
            var move = (GomokuMove)RedoStack.Pop();
            Board[move.X, move.Y] = move.Piece;
            UndoStack.Push(move);
            CheckGameOver(move.X, move.Y);
            CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
            Console.WriteLine($"Redo move at ({move.X},{move.Y}) by {CurrentPlayer.Name}");
        }

        private void CheckGameOver(int x, int y)
        {
            char p = Board[x, y];
            if (p == '.') return;

            int[][] directions = new int[][]
            {
                new int[]{1,0}, new int[]{0,1}, new int[]{1,1}, new int[]{1,-1}
            };

            foreach (var dir in directions)
            {
                int count = 1;
                // check forward
                count += CountInDirection(x, y, dir[0], dir[1], p);
                // check backward
                count += CountInDirection(x, y, -dir[0], -dir[1], p);

                if (count >= 5)
                {
                    gameOverFlag = true;
                    winner = CurrentPlayer;
                    return;
                }
            }
            // Check for draw: no empty spots
            bool draw = true;
            for (int i = 0; i < BoardSize; i++)
                for (int j = 0; j < BoardSize; j++)
                    if (Board[i, j] == '.')
                        draw = false;
            if (draw)
            {
                gameOverFlag = true;
                winner = null;
            }
        }

        private int CountInDirection(int x, int y, int dx, int dy, char p)
        {
            int count = 0;
            int nx = x + dx;
            int ny = y + dy;
            while (nx >= 0 && nx < BoardSize && ny >= 0 && ny < BoardSize && Board[nx, ny] == p)
            {
                count++;
                nx += dx;
                ny += dy;
            }
            return count;
        }

        public override void DisplayBoard()
        {
            Console.WriteLine();
            Console.Write("   ");
            for (int i = 0; i < BoardSize; i++)
                Console.Write(i.ToString().PadLeft(2) + " ");
            Console.WriteLine();
            for (int y = 0; y < BoardSize; y++)
            {
                Console.Write(y.ToString().PadLeft(2) + " ");
                for (int x = 0; x < BoardSize; x++)
                {
                    Console.Write($" {Board[x, y]} ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public override void Save(string filePath)
        {
            var data = new GomokuSave()
            {
                BoardSize = this.BoardSize,
                Board = this.Board,
                CurrentPlayerName = CurrentPlayer.Name,
                Player1Name = Player1.Name,
                Player2Name = Player2.Name,
                Player1IsHuman = Player1.IsHuman,
                Player2IsHuman = Player2.IsHuman,
                UndoMoves = UndoStack.Cast<GomokuMove>().ToList(),
                RedoMoves = RedoStack.Cast<GomokuMove>().ToList()
            };
            var json = JsonSerializer.Serialize(data);
            File.WriteAllText(filePath, json);
            Console.WriteLine($"Game saved to {filePath}");
        }

        public override bool Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Save file not found.");
                return false;
            }
            try
            {
                var json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<GomokuSave>(json);
                if (data == null) return false;
                BoardSize = data.BoardSize;
                Board = data.Board;
                Player1.Name = data.Player1Name;
                Player2.Name = data.Player2Name;
                Player1.IsHuman = data.Player1IsHuman;
                Player2.IsHuman = data.Player2IsHuman;
                CurrentPlayer = data.CurrentPlayerName == Player1.Name ? Player1 : Player2;
                UndoStack.Clear();
                RedoStack.Clear();
                foreach (var m in data.UndoMoves)
                    UndoStack.Push(m);
                foreach (var m in data.RedoMoves)
                    RedoStack.Push(m);
                gameOverFlag = false;
                winner = null;
                // No checkGameOver here, assume consistent save
                Console.WriteLine($"Game loaded from {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading file: " + ex.Message);
                return false;
            }
        }

        public override IGame Clone()
        {
            var clone = new Gomoku(Player1, Player2, BoardSize);
            clone.Board = (char[,])this.Board.Clone();
            clone.CurrentPlayer = this.CurrentPlayer;
            clone.gameOverFlag = this.gameOverFlag;
            clone.winner = this.winner;
            return clone;
        }

        private class GomokuSave
        {
            public int BoardSize { get; set; }
            public char[,] Board { get; set; }
            public string CurrentPlayerName { get; set; }
            public string Player1Name { get; set; }
            public string Player2Name { get; set; }
            public bool Player1IsHuman { get; set; }
            public bool Player2IsHuman { get; set; }
            public List<GomokuMove> UndoMoves { get; set; }
            public List<GomokuMove> RedoMoves { get; set; }
        }
    }
}
