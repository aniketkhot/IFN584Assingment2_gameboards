namespace BoardGamesFramework
{
    public class NotaktoMove : Move
    {
        public int BoardIndex { get; set; }
        public int Position { get; set; }

        public NotaktoMove() { } 

        public NotaktoMove(int boardIndex, int position)
        {
            BoardIndex = boardIndex;
            Position = position;
        }
    }
}
