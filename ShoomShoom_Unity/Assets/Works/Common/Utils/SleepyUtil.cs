using UnityEngine;

namespace SleepyUtil
{
    public static class RangeUtil
    {
        public static string GetRangeName(Transform lRange, Transform rRange)
        {
            return $"{lRange.name}_to_{rRange.name}";
        }
    }
}