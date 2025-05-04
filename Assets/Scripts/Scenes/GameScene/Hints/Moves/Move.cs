namespace Core.Hints.Moves
{
    public struct Move
    {
        public int From { get; private set; }
        public int To { get; private set; }
        public int TransferAmount { get; private set; }

        public Move(int from, int to, int transferAmount)
        {
            From = from;
            To = to;
            TransferAmount = transferAmount;
        }
    }
}
