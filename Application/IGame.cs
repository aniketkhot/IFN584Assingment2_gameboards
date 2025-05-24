namespace BoardGamesFramework
{
    // Interface for Games
    public interface IGame
    {
        Player Player1 { get; set; }
        Player Player2 { get; set; }
        Player CurrentPlayer { get; set; }

        bool IsGameOver { get; }
        Player Winner { get; }

        bool IsValidMove(Move move);
        void MakeMove(Move move);
        void Undo();
        void Redo();
        void DisplayBoard();
        void Save(string filePath);
        bool Load(string filePath);
        IGame Clone();
        string GameName { get; }
    }
}
