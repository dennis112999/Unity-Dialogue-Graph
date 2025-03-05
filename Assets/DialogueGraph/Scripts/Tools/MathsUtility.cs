using UnityEngine;

namespace Dennis.Tools
{
    public static class MathsUtility
    {
        public static float Add(float a, float b) => a + b;

        public static float Subtract(float a, float b) => a - b;

        public static float Multiply(float a, float b) => a * b;

        public static float Divide(float a, float b, string variableName)
        {
            if (b != 0)
                return a / b;

#if UNITY_EDITOR
            Debug.LogError($"[MathsUtility] Attempted division by zero: Variable '{variableName}'");
#endif
            return a;
        }

        public static bool IsTrue(float value) => value == 1;

        public static bool IsFalse(float value) => value == 0;

        public static bool IsEqual(float value, float target) => value == target;

        public static bool IsGreaterOrEqual(float value, float target) => value >= target;

        public static bool IsLessOrEqual(float value, float target) => value <= target;

        public static bool IsGreater(float value, float target) => value > target;

        public static bool IsLess(float value, float target) => value < target;
    }
}
