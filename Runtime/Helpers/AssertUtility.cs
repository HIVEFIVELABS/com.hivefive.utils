using UnityEngine.Assertions;

namespace HiveFive.Utils
{
    public static class AssertUtility
    {
        public static void AssertIsNotNullOrEmpty(this string s)
        {
            Assert.IsNotNull(s, "s != null");
            Assert.IsTrue(s.Length > 0, "s.Length > 0");
        }
    }
}