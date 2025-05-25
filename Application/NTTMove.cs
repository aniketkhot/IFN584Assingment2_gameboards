namespace BoardGamesFramework
{
    public class NTTMove : Move
    {
        public int Position { get; set; }
        public int NumberPlaced { get; set; }

        public NTTMove() { } 

        public NTTMove(int position, int numberPlaced)
        {
            Position = position;
            NumberPlaced = numberPlaced;
        }
    }
}
