using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using System;

namespace Dennis.Tools.DialogueGraph.Data
{
    [Serializable]
    public class DialogueNodeData : BaseData
    {
        [SerializeReference]  public List<DialogueElementBase> AllDialogueElements = new List<DialogueElementBase>();

        public List<DialogueDataPort> DialogueDataPorts = new List<DialogueDataPort>();

        public List<DialogueBoxData> GetDialogueBoxes()
        {
            return AllDialogueElements.OfType<DialogueBoxData>().ToList();
        }

        public List<DialogueNameData> GetDialogueNames()
        {
            return AllDialogueElements.OfType<DialogueNameData>().ToList();
        }

        public List<DialogueImagesData> GetDialogueImages()
        {
            return AllDialogueElements.OfType<DialogueImagesData>().ToList();
        }
    }

    [Serializable]
    public abstract class DialogueElementBase
    {
        public int OrderIndex;
    }

    [Serializable]
    public class DialogueBoxData : DialogueElementBase
    {
        public string Text;
        public AudioClip AudioClip;
    }

    [Serializable]
    public class DialogueNameData : DialogueElementBase
    {
        public string Name;
    }

    [Serializable]
    public class DialogueImagesData : DialogueElementBase
    {
        public Sprite Sprite_Left;
        public Sprite Sprite_Right;

        public SpeakerType SpeakerTypeLeft;
        public SpeakerType SpeakerTypeRight;
    }

    [Serializable]
    public class DialogueDataPort
    {
        public string PortGuid;
        public string InputGuid;
        public string OutputGuid;
    }

    public enum SpeakerType
    {
        None,
        Speaking
    }
}
