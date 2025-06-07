using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.GracesScripts
{
    public class Unit_NPC : Unit, IInteracble, IHasDialogue
    {
        public string unitName;

        public string SceneAfterWin;

        /// <summary>
        /// Because Humanoid NPC needs to flip. but the mimic chests don't and probably others.
        /// </summary>
        [SerializeField] private bool FlipBattleIdleAnimation;

        private void StartBattleIdleAnimation()
        {
            animatedLayers.SetTriggers("StartFight");
        }

        public void SetupUnitForBattle()
        {
            Debug.Log("EVERYTHING BELOW GETFIRSTDIALOGUE SLIDE TO GO IN UNITFORBATTEL.CS an extension of this class for the turn based battle. NAH maybe not? thogh animate enemy health in here is conveintint because dont have to setup UI on the new object when battle starts");
            MyGuard.IsNotNull(this.healthBarFill);
            MyGuard.IsNotNull(this.HealthBarObject);
            this.healthBarFill.fillAmount = this.currentHealth / this.maxHealth;
            this.StartBattleIdleAnimation();

            if (FlipBattleIdleAnimation)
            {
                this.transform.localScale = new Vector3(this.transform.localScale.x * -1, this.transform.localScale.y, this.transform.localScale.z);
            }
        }

        /// <summary>
        /// Used for not in battle scene. Each is the first Dialogue slide for the start of a NEW conversation. i.e. when the player finished the interactino with the NPC then starts another conversation with him it will be the next conversation.
        /// </summary>
        public List<DialogueSlide> Conversations;

        public DialogueSlide nextConversationDialogueSilde => Conversations.First();

        /// <summary>
        /// For talking in battle Scene TODO not setup yet
        /// </summary>
        public List<DialogueSlide> battleConversations;

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
            MyGuard.IsNotNull(this.nextConversationDialogueSilde, $"Must assign dialogue slide to {this.unitName}");
            return this.nextConversationDialogueSilde;
        }
    }
}
