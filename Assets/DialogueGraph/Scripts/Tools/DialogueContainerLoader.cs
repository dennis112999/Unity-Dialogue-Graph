using Dennis.Tools.DialogueGraph.Data;
using UnityEngine;

namespace Dennis.Tools.DialogueGraph
{
    public static class DialogueContainerLoader
    {
        private static DialogueContainerDatabase database;

        public static void Initialize(DialogueContainerDatabase db)
        {
            database = db;
        }

        public static DialogueContainer LoadDialogueContainer(string dialogueContainerId)
        {
            if (database == null)
            {
#if UNITY_EDITOR
                Debug.LogError("DialogueContainerDatabase not initialized.");
#endif
                return null;
            }

            return database.GetContainerById(dialogueContainerId);
        }
    }
}
