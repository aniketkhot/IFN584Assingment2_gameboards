namespace BoardGamesFramework
{
    // ----- Numerical Tic Tac Toe Implementation -----

    public class NTTMove : Move
    {
        public int Position { get; set; }
        public int NumberPlaced { get; set; }
        public NTTMove(int pos, int num)
        {
            Position = pos;
            NumberPlaced = num;
        }
    }
}
