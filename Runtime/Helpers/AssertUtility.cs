using UnityEngine.Assertions;

namespace Hivefive.Utils
{
    public static class AssertUtility
    {
        public static void AssertIsNotNullOrEmpty(this string s, string userMessage = null)
        {
            switch (s) {
                case null:
                    Fail("String is null. A non-null string was expected.", userMessage);
                    break;
                case "":
                    Fail("String is empty. A non-empty string was expected.", userMessage);
                    break;
            }
        }

        private static void Fail(string message, string userMessage)
        {
            throw new AssertionException(message, userMessage);
        }
    }
}