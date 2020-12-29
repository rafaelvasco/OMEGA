namespace OMEGA
{
    public struct CharRange
    {
        public int Start;
        public int End;

        public static readonly CharRange Latin = new CharRange(32, 126);
        public static readonly CharRange LatinSupplement = new CharRange(160, 255);

        public CharRange(int start, int end)
        {
            Start = start;
            End = end;
        }
    }
}
