using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Dennis.Tools.DialogueGraph.Data
{
    [System.Serializable]
    public class DialogueNodeData : BaseData
    {
        public List<DialogueElementBase> AllDialogueElements = new List<DialogueElementBase>();

        public List<DialogueBoxData> DialogueBoxes = new List<DialogueBoxData>();
        public List<DialogueImagesData> DialogueImagesDatas = new List<DialogueImagesData>();

        public List<DialogueDataPort> DialogueDataPorts = new List<DialogueDataPort>();
    }

    [System.Serializable]
    public abstract class DialogueElementBase
    {
        public int OrderIndex;
    }

    [System.Serializable]
    public class DialogueBoxData : DialogueElementBase
    {
        public string Text;
        public AudioClip AudioClip;
    }

    [System.Serializable]
    public class DialogueImagesData : DialogueElementBase
    {
        public Sprite Sprite_Left;
        public Sprite Sprite_Right;
    }

    [System.Serializable]
    public class DialogueDataPort
    {
        public string PortGuid;
        public string InputGuid;
        public string OutputGuid;
    }
}
