using System.Text.Json;


namespace BoardGamesFramework
{
    public class NumericalTicTacToe : GameBase
    {
        public override string GameName => "Numerical Tic Tac Toe";
        private int?[] Board = new int?[9];
        private HashSet<int> UsedNumbers = new HashSet<int>();

        private static int[][] winningLines = new int[][]
        {
            new int[] {0,1,2}, new int[] {3,4,5}, new int[] {6,7,8},
            new int[] {0,3,6}, new int[] {1,4,7}, new int[] {2,5,8},
            new int[] {0,4,8}, new int[] {2,4,6}
        };

        private bool gameOverFlag = false;
        private Player winner = null;

        public NumericalTicTacToe(Player p1, Player p2)
        {
            Player1 = p1;
            Player2 = p2;
            CurrentPlayer = Player1;
            Reset();
        }

        public void Reset()
        {
            for (int i = 0; i < 9; i++)
                Board[i] = null;
            UsedNumbers.Clear();
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
            if (!(move is NTTMove m))
                return false;
            if (m.Position < 0 || m.Position >= 9)
                return false;
            if (Board[m.Position].HasValue)
                return false;
            if (UsedNumbers.Contains(m.NumberPlaced))
                return false;
            if (CurrentPlayer == Player1 && m.NumberPlaced % 2 == 0)
                return false;
            if (CurrentPlayer == Player2 && m.NumberPlaced % 2 == 1)
                return false;
            return true;
        }

        public override void MakeMove(Move move)
        {
            if (!(move is NTTMove m))
                throw new ArgumentException("Invalid move type");
            if (!IsValidMove(m))
                throw new InvalidOperationException("Invalid move");

            Board[m.Position] = m.NumberPlaced;
            UsedNumbers.Add(m.NumberPlaced);
            UndoStack.Push(move);
            RedoStack.Clear();
            CheckGameOver();
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
            var lastMove = (NTTMove)UndoStack.Pop();
            Board[lastMove.Position] = null;
            UsedNumbers.Remove(lastMove.NumberPlaced);
            RedoStack.Push(lastMove);
            gameOverFlag = false;
            winner = null;
            CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
            Console.WriteLine($"Undo move at position {lastMove.Position} by {CurrentPlayer.Name}");
        }

        public override void Redo()
        {
            if (RedoStack.Count == 0)
            {
                Console.WriteLine("Nothing to redo.");
                return;
            }
            var move = (NTTMove)RedoStack.Pop();
            Board[move.Position] = move.NumberPlaced;
            UsedNumbers.Add(move.NumberPlaced);
            UndoStack.Push(move);
            CheckGameOver();
            CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
            Console.WriteLine($"Redo move at position {move.Position} by {CurrentPlayer.Name}");
        }

        private void CheckGameOver()
        {
            foreach (var line in winningLines)
            {
                if (Board[line[0]].HasValue && Board[line[1]].HasValue && Board[line[2]].HasValue)
                {
                    int sum = Board[line[0]].Value + Board[line[1]].Value + Board[line[2]].Value;
                    if (sum == 15)
                    {
                        gameOverFlag = true;
                        winner = CurrentPlayer;
                        return;
                    }
                }
            }
            // Check draw
            if (Board.All(p => p.HasValue))
            {
                gameOverFlag = true;
                winner = null;
            }
        }

        public override void DisplayBoard()
        {
            Console.WriteLine();
            for (int i = 0; i < 9; i++)
            {
                if (Board[i].HasValue)
                    Console.Write(Board[i].Value.ToString().PadLeft(2));
                else
                    Console.Write(" .");
                if ((i + 1) % 3 == 0)
                    Console.WriteLine();
            }
            Console.WriteLine();
        }

        public override void Save(string filePath)
        {
            var data = new NumericalTicTacToeSave()
            {
                Board = this.Board,
                UsedNumbers = this.UsedNumbers.ToList(),
                CurrentPlayerName = CurrentPlayer.Name,
                Player1Name = Player1.Name,
                Player2Name = Player2.Name,
                Player1IsHuman = Player1.IsHuman,
                Player2IsHuman = Player2.IsHuman,
                UndoMoves = UndoStack.Cast<NTTMove>().ToList(),
                RedoMoves = RedoStack.Cast<NTTMove>().ToList()
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
                var data = JsonSerializer.Deserialize<NumericalTicTacToeSave>(json);
                if (data == null) return false;
                Board = data.Board;
                UsedNumbers = new HashSet<int>(data.UsedNumbers);
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
                CheckGameOver();
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
            var clone = new NumericalTicTacToe(Player1, Player2);
            clone.Board = (int?[])this.Board.Clone();
            clone.UsedNumbers = new HashSet<int>(this.UsedNumbers);
            clone.CurrentPlayer = this.CurrentPlayer;
            clone.gameOverFlag = this.gameOverFlag;
            clone.winner = this.winner;
            // Note: Undo/Redo stacks not cloned here
            return clone;
        }

        // Helper class for serialization
        private class NumericalTicTacToeSave
        {
            public int?[] Board { get; set; }
            public List<int> UsedNumbers { get; set; }
            public string CurrentPlayerName { get; set; }
            public string Player1Name { get; set; }
            public string Player2Name { get; set; }
            public bool Player1IsHuman { get; set; }
            public bool Player2IsHuman { get; set; }
            public List<NTTMove> UndoMoves { get; set; }
            public List<NTTMove> RedoMoves { get; set; }
        }
    }
}
