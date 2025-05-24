using System.Text.Json;


namespace BoardGamesFramework
{
    public class Notakto : GameBase
    {
        public override string GameName => "Notakto";

        // Three 3x3 boards, each position bool (true if X placed)
        private bool[][] Boards = new bool[3][]
        {
            new bool[9], new bool[9], new bool[9]
        };

        private bool gameOverFlag = false;
        private Player loser = null;

        public Notakto(Player p1, Player p2)
        {
            Player1 = p1;
            Player2 = p2;
            CurrentPlayer = Player1;
            Reset();
        }

        public void Reset()
        {
            for (int i = 0; i < 3; i++)
                Boards[i] = new bool[9];
            UndoStack.Clear();
            RedoStack.Clear();
            gameOverFlag = false;
            loser = null;
            CurrentPlayer = Player1;
        }

        public override bool IsGameOver => gameOverFlag;

        public override Player Winner => gameOverFlag ? (loser == Player1 ? Player2 : Player1) : null;

        public override bool IsValidMove(Move move)
        {
            if (!(move is NotaktoMove m))
                return false;
            if (m.BoardIndex < 0 || m.BoardIndex > 2) return false;
            if (m.Position < 0 || m.Position >= 9) return false;
            if (Boards[m.BoardIndex][m.Position]) return false; // Already taken
            return true;
        }

        public override void MakeMove(Move move)
        {
            if (!(move is NotaktoMove m))
                throw new ArgumentException("Invalid move type");
            if (!IsValidMove(m))
                throw new InvalidOperationException("Invalid move");

            Boards[m.BoardIndex][m.Position] = true;
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
            var lastMove = (NotaktoMove)UndoStack.Pop();
            Boards[lastMove.BoardIndex][lastMove.Position] = false;
            RedoStack.Push(lastMove);
            gameOverFlag = false;
            loser = null;
            CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
            Console.WriteLine($"Undo move on board {lastMove.BoardIndex} position {lastMove.Position} by {CurrentPlayer.Name}");
        }

        public override void Redo()
        {
            if (RedoStack.Count == 0)
            {
                Console.WriteLine("Nothing to redo.");
                return;
            }
            var move = (NotaktoMove)RedoStack.Pop();
            Boards[move.BoardIndex][move.Position] = true;
            UndoStack.Push(move);
            CheckGameOver();
            CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
            Console.WriteLine($"Redo move on board {move.BoardIndex} position {move.Position} by {CurrentPlayer.Name}");
        }

        private static int[][] winningLines = new int[][]
        {
            new int[] {0,1,2}, new int[] {3,4,5}, new int[] {6,7,8},
            new int[] {0,3,6}, new int[] {1,4,7}, new int[] {2,5,8},
            new int[] {0,4,8}, new int[] {2,4,6}
        };

        private bool HasThreeInRow(bool[] board)
        {
            foreach (var line in winningLines)
            {
                if (board[line[0]] && board[line[1]] && board[line[2]])
                    return true;
            }
            return false;
        }

        private void CheckGameOver()
        {
            // Game ends when all three boards contain a three-in-a-row of X
            if (Boards.All(b => HasThreeInRow(b)))
            {
                gameOverFlag = true;
                loser = CurrentPlayer; // Last move player loses
            }
        }

        public override void DisplayBoard()
        {
            Console.WriteLine();
            for (int b = 0; b < 3; b++)
            {
                Console.WriteLine($"Board {b}:");
                for (int i = 0; i < 9; i++)
                {
                    Console.Write(Boards[b][i] ? " X" : " .");
                    if ((i + 1) % 3 == 0)
                        Console.WriteLine();
                }
                Console.WriteLine();
            }
        }

        public override void Save(string filePath)
        {
            var data = new NotaktoSave()
            {
                Boards = Boards,
                CurrentPlayerName = CurrentPlayer.Name,
                Player1Name = Player1.Name,
                Player2Name = Player2.Name,
                Player1IsHuman = Player1.IsHuman,
                Player2IsHuman = Player2.IsHuman,
                UndoMoves = UndoStack.Cast<NotaktoMove>().ToList(),
                RedoMoves = RedoStack.Cast<NotaktoMove>().ToList()
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
                var data = JsonSerializer.Deserialize<NotaktoSave>(json);
                if (data == null) return false;
                Boards = data.Boards;
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
                loser = null;
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
            var clone = new Notakto(Player1, Player2);
            for (int i = 0; i < 3; i++)
            {
                clone.Boards[i] = (bool[])this.Boards[i].Clone();
            }
            clone.CurrentPlayer = this.CurrentPlayer;
            clone.gameOverFlag = this.gameOverFlag;
            clone.loser = this.loser;
            return clone;
        }

        private class NotaktoSave
        {
            public bool[][] Boards { get; set; }
            public string CurrentPlayerName { get; set; }
            public string Player1Name { get; set; }
            public string Player2Name { get; set; }
            public bool Player1IsHuman { get; set; }
            public bool Player2IsHuman { get; set; }
            public List<NotaktoMove> UndoMoves { get; set; }
            public List<NotaktoMove> RedoMoves { get; set; }
        }
    }
}
