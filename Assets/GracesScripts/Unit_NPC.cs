using UnityEngine;

namespace Assets.GracesScripts
{
    public class Unit_NPC : Unit, IInteracble, IHasDialogue
    {
        public string unitName;

        public string SceneAfterWin;

        /// <summary>
        /// Used for not in battle scene.
        /// </summary>
        public DialogueSlide firstDialogueSlide;

        /// <summary>
        /// For talking in battle Scene TODO not setup yet
        /// </summary>
        public DialogueSlide battleSceneDialogueSlide;

        private void Start()
        {
            if (this.unitName == null)
            {
                Debug.LogError($"this guy {this.gameObject.name} cannot have no Unitname on Unit.cs");
            }

            if (this.Abilities.Count == 0)
            {
                Debug.LogError($"this guy: {this.gameObject.name} cannot have no abilities at least have default Push ability assign in inspector");
            }
        }

        protected override void Die()
        {
            Debug.Log("ENEmy dies play player win sound and visuals and exit turn based");
        }

        public DialogueSlide GetFirstDialogueSlide()
        {
            MyGuard.IsNotNull(this.firstDialogueSlide, $"Must assign dialogue slide to {this.unitName}");
            return this.firstDialogueSlide;
        }
    }
}
