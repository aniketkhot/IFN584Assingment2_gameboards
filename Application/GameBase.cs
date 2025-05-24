namespace BoardGamesFramework
{
    // Abstract base class for common Undo/Redo and state management
    public abstract class GameBase : IGame
    {
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Player CurrentPlayer { get; set; }
        public abstract bool IsGameOver { get; }
        public abstract Player Winner { get; }
        public abstract string GameName { get; }

        protected Stack<Move> UndoStack = new Stack<Move>();
        protected Stack<Move> RedoStack = new Stack<Move>();

        public abstract bool IsValidMove(Move move);
        public abstract void MakeMove(Move move);
        public abstract void Undo();
        public abstract void Redo();
        public abstract void DisplayBoard();
        public abstract void Save(string filePath);
        public abstract bool Load(string filePath);
        public abstract IGame Clone();
    }
}
