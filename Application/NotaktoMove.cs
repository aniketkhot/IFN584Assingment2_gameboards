namespace BoardGamesFramework
{
    // ----- Notakto Implementation -----
    public class NotaktoMove : Move
    {
        public int BoardIndex { get; set; }  // 0,1,2 for which 3x3 board
        public int Position { get; set; }    // 0-8 position on board
        public NotaktoMove(int boardIndex, int position)
        {
            BoardIndex = boardIndex;
            Position = position;
        }
    }
}
