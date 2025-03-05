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
    }
}
