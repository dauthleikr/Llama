namespace Llama.PE
{
    internal static class Round
    {
        public static uint AlwaysUp(uint number, uint blockSize) => number + blockSize - number % blockSize;
        public static uint Up(uint number, uint blockSize) => number % blockSize == 0 ? number : AlwaysUp(number, blockSize);
        public static int AlwaysUp(int number, int blockSize) => number + blockSize - number % blockSize;
        public static int Up(int number, int blockSize) => number % blockSize == 0 ? number : AlwaysUp(number, blockSize);
        public static ulong AlwaysUp(ulong number, ulong blockSize) => number + blockSize - number % blockSize;
        public static ulong Up(ulong number, ulong blockSize) => number % blockSize == 0 ? number : AlwaysUp(number, blockSize);
        public static long AlwaysUp(long number, long blockSize) => number + blockSize - number % blockSize;
        public static long Up(long number, long blockSize) => number % blockSize == 0 ? number : AlwaysUp(number, blockSize);
    }
}