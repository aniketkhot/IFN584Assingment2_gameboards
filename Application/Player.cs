namespace BoardGamesFramework
{
    // Abstract Player base class
    public abstract class Player
    {
        public string Name { get; set; }
        public bool IsHuman { get; set; }
        public abstract Move GetMove(IGame game);
    }
}
