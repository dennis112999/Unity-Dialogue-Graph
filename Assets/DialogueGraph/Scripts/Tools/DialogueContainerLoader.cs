using UnityEngine;
using System.Collections.Generic;

using Dennis.Tools.DialogueGraph.Data;

namespace Dennis.Tools.DialogueGraph
{
    public static class DialogueContainerLoader
    {
        private static Dictionary<string, DialogueContainer> loadedContainers = new Dictionary<string, DialogueContainer>();

        // Define the base path for dialogue containers
        private const string DialogueResourcesPath = "DialogueGraphDatas/";

        /// <summary>
        /// Preloads all DialogueContainers from Resources/DialogueGraphDatas at startup
        /// </summary>
        public static void PreloadAllDialogueContainers()
        {
            DialogueContainer[] containers = Resources.LoadAll<DialogueContainer>(DialogueResourcesPath.TrimEnd('/'));

            foreach (var container in containers)
            {
                if (!loadedContainers.ContainsKey(container.name))
                {
                    loadedContainers[container.name] = container;
                }
            }
        }

        /// <summary>
        /// Loads a specific DialogueContainer by ID (uses cache if already loaded)
        /// </summary>
        public static DialogueContainer LoadDialogueContainer(string dialogueContainerId)
        {
            if (loadedContainers.TryGetValue(dialogueContainerId, out DialogueContainer container))
            {
                return container; // Return cached container if already loaded.
            }

            container = Resources.Load<DialogueContainer>($"{DialogueResourcesPath}{dialogueContainerId}");

            if (container != null)
            {
                loadedContainers[dialogueContainerId] = container; // Cache the loaded container.
                return container;
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning($"DialogueContainer '{dialogueContainerId}' not found in Resources/{DialogueResourcesPath}.");
#endif
                return null;
            }
        }
    }
}
