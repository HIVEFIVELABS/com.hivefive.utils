using UnityEngine.UIElements;

namespace Hivefive.Utils
{
    public static class VisualElementUtility
    {
        public static float ToSeconds(this TimeValue timeValue)
        {
            switch (timeValue.unit) {
                case TimeUnit.Millisecond: return timeValue.value * 0.001f;
                case TimeUnit.Second:
                default: return timeValue.value;
            }
        }

        public static long ToMilliseconds(this TimeValue timeValue)
        {
            switch (timeValue.unit) {
                case TimeUnit.Second: return (long)(timeValue.value * 1000L);
                case TimeUnit.Millisecond:
                default: return (long)timeValue.value;
            }
        }
    }
}