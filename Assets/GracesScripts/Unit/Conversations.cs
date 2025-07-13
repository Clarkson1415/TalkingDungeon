using System.Collections.Generic;
using UnityEngine;

namespace Assets.GracesScripts.Unit
{
    public class Conversations : MonoBehaviour
    {
        /// <summary>
        /// Used for not in battle scene. Each is the first Dialogue slide for the start of a NEW conversation. i.e. when the player finished the interactino with the NPC then starts another conversation with him it will be the next conversation.
        /// </summary>
        [SerializeField] private List<DialogueSlide> firstSlideOfEachConvo = new();

        public DialogueSlide GetConversationStart()
        {
            if (ConversationIndex > firstSlideOfEachConvo.Count)
            {
                Debug.LogError("Grace forgot to add conversation to this guy.");
            }

            var slide = this.firstSlideOfEachConvo[ConversationIndex];
            ConversationIndex++;
            return slide;
        }

        public int ConversationIndex = 0;
    }
}
