namespace BoardGamesFramework
{
    // ----- Gomoku Implementation -----

    public class GomokuMove : Move
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char Piece { get; set; }  // 'X' or 'O'
        public GomokuMove(int x, int y, char piece)
        {
            X = x;
            Y = y;
            Piece = piece;
        }
    }
}
