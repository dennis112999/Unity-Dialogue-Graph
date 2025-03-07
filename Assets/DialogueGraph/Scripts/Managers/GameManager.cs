using UnityEngine;
using Dennis.Tools.DialogueGraph.Data;

namespace Dennis.Tools.DialogueGraph.Sample
{
    /// <summary>
    /// Sample Game Manager
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] VariableData _variableData;

        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            Initialize();
        }

        private void Initialize()
        {
            VariableManager.Initialize(_variableData);
        }

        private void OnDestroy()
        {
            VariableManager.Cleanup();
        }

#if UNITY_EDITOR

        private void OnGUI()
        {
            if (!Application.isPlaying || _variableData == null) return;

            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 30,
                normal = { textColor = Color.white }
            };

            GUILayout.BeginArea(new Rect(Screen.width - 300, 20, 600, 400));
            GUILayout.Label("Variable Debugger", style);

            var variables = _variableData.GetAllVariables();
            foreach (var kvp in variables)
            {
                GUILayout.Label($"{kvp.Key}: {kvp.Value}", style);
            }

            GUILayout.EndArea();
        }

#endif

    }
}
