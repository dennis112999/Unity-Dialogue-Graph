using System.Collections.Generic;
using UnityEngine;

namespace Dennis.Tools.DialogueGraph.Data
{
    [System.Serializable]
    public class DialogueNodeData : BaseData
    {
        public List<DialogueElementBase> AllDialogueElements = new List<DialogueElementBase>();

        public List<DialogueBoxData> DialogueBoxes = new List<DialogueBoxData>();
        public List<DialogueImagesData> DialogueImagesDatas = new List<DialogueImagesData>();
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
}
