namespace Scripts.Utils
{
    public static class FormatHelper
    {
        public static string FormatTime(float time)
        {
            return $"{time / 60f:00}:{time % 60f:00.000}";
        }
    }
}