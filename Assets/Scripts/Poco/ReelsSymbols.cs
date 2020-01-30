using System;

namespace Poco
{
    [Serializable]
    public class ReelsSymbols
    {
        public int[][] Reels;
        public Prize[][] Prizes;
    }
    
    [Serializable]
    public class Prize
    {
        public int[] Combination;
        public int PrizeValue;
    }
}