namespace Extensions
{
    public static class SpeedExtensions
    {
        public static float MSToKmH(this int speedMS) => 
            speedMS / 1000 * 3.6f;
    }
}