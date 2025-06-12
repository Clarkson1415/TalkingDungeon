using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.GracesScripts.Unit
{
    public class Conversations : MonoBehaviour
    {
        /// <summary>
        /// Used for not in battle scene. Each is the first Dialogue slide for the start of a NEW conversation. i.e. when the player finished the interactino with the NPC then starts another conversation with him it will be the next conversation.
        /// </summary>
        [SerializeField] private List<DialogueSlide> firstSlideOfEachConvo;

        /// <summary>
        /// Gets the next conversation dialogue slide.
        /// </summary>
        public DialogueSlide nextConversationDialogueSilde => this.firstSlideOfEachConvo.First();
    }
}
