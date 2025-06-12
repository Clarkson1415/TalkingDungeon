using Assets.GracesScripts.Unit;
using UnityEngine;
#nullable enable

namespace Assets.GracesScripts
{
    [RequireComponent(typeof(Conversations))]
    public class Unit_NPC : DungeonUnit, IInteracble
    {
        public string unitName;

        public string SceneAfterWin;

        /// <summary>
        /// Because Humanoid NPC needs to flip. but the mimic chests don't and probably others.
        /// </summary>
        [SerializeField] private bool FlipBattleIdleAnimation;

        private DialogueTextBox? dialogueTextBox;
        private Conversations conversations;

        private bool _finishedInteraction;

        public bool FinishedInteraction { get => _finishedInteraction; set => _finishedInteraction = value; }

        private void StartBattleIdleAnimation()
        {
            MyGuard.IsNotNull(this.animatedLayers);
            this.animatedLayers.SetTriggers("StartFight");
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

        ///// <summary>
        ///// For talking in battle Scene TODO not setup yet
        ///// </summary>
        //public List<DialogueSlide> battleConversations;

        private void Start()
        {
            if (string.IsNullOrEmpty(unitName))
            {
                Debug.LogError($"this guy {this.gameObject.name} cannot have no Unitname on Unit.cs");
            }

            if (this.Abilities.Count == 0)
            {
                Debug.LogError($"this guy: {this.gameObject.name} cannot have no abilities at least have default Push ability assign in inspector");
            }

            conversations = this.GetComponent<Conversations>();
            var menuReferences = FindObjectOfType<MenuReferences>();
            this.dialogueTextBox = menuReferences.dialogueTextBox;
        }

        protected override void Die()
        {
            Debug.Log("ENEmy dies play player win sound and visuals and exit turn based");
        }

        public virtual void Interact()
        {
            this._finishedInteraction = false;
            MyGuard.IsNotNull(this.dialogueTextBox, "DialogueTextBox is null in Unit_NPC.Interact()");
            this.dialogueTextBox.gameObject.SetActive(true);
            this.dialogueTextBox.BeginDialogue(conversations.nextConversationDialogueSilde, this);
        }

        public virtual void EndInteract()
        {
            this._finishedInteraction = true;
        }
    }
}
