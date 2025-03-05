using UnityEngine;
using Dennis.Tools.DialogueGraph.Data;

namespace Dennis.Tools.DialogueGraph
{
    public static class ConditionProcessor
    {
        /// <summary>
        /// Checks if a condition is met based on the specified condition type
        /// </summary>
        /// <param name="currentValue">The current value of the variable</param>
        /// <param name="data">Condition data containing the condition type and target value</param>
        /// <returns>True if the condition is met, otherwise false</returns>
        public static bool Evaluate(float currentValue, ConditionData data)
        {
            switch (data.ConditionType)
            {
                case ConditionType.IsTrue:
                    return MathsUtility.IsTrue(currentValue);

                case ConditionType.IsFalse:
                    return MathsUtility.IsFalse(currentValue);

                case ConditionType.Equal:
                    return MathsUtility.IsEqual(currentValue, data.ComparisonValue);

                case ConditionType.GreaterOrEqual:
                    return MathsUtility.IsGreaterOrEqual(currentValue, data.ComparisonValue);

                case ConditionType.LessOrEqual:
                    return MathsUtility.IsLessOrEqual(currentValue, data.ComparisonValue);

                case ConditionType.Greater:
                    return MathsUtility.IsGreater(currentValue, data.ComparisonValue);

                case ConditionType.Less:
                    return MathsUtility.IsLess(currentValue, data.ComparisonValue);

                default:
#if UNITY_EDITOR
                    Debug.LogError($"[ConditionProcessor] Unsupported condition type: {data.ConditionType}");
#endif
                    return false;
            }
        }
    }
}
