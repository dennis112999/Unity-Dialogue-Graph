using System.Collections.Generic;
using UnityEngine;

namespace Dennis.Tools.DialogueGraph.Data
{
    [System.Serializable]
    public class DialogueNodeData : BaseData
    {
        public List<DialogueBoxData> DialogueBoxes = new List<DialogueBoxData>();
        public List<DialogueImagesData> DialogueImagesDatas = new List<DialogueImagesData>();
    }

    [System.Serializable]
    public class DialogueBoxData
    {
        public string Text;

        public AudioClip AudioClip;

        public DialogueBoxData(string text = "default", AudioClip audioClip = null)
        {
            Text = text;
            AudioClip = audioClip;
        }
    }

    [System.Serializable]
    public class DialogueImagesData
    {
        public int ID;

        public Sprite Sprite_Left;
        public Sprite Sprite_Right;
    }
}
