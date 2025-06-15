using System.Collections.Generic;
using UnityEngine;

namespace Assets.GracesScripts.Unit
{
    public class Conversations : MonoBehaviour
    {
        /// <summary>
        /// Used for not in battle scene. Each is the first Dialogue slide for the start of a NEW conversation. i.e. when the player finished the interactino with the NPC then starts another conversation with him it will be the next conversation.
        /// </summary>
        [SerializeField] private List<DialogueSlide> firstSlideOfEachConvo;

        public DialogueSlide GetConversationStart()
        {
            var slide = this.firstSlideOfEachConvo[ConversationIndex];
            ConversationIndex++;
            return slide;
        }

        public int ConversationIndex = 0;
    }
}
