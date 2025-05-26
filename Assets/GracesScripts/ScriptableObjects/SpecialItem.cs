using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.GracesScripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "DungeonItems/SpecialItem", order = 1)]
    public class SpecialItem : DungeonItem
    {
        public override string Path { get => $"Items/SpecialItem/{this.Name}"; set => throw new NotImplementedException("we never need to set this"); }
        public List<AbilityEffect> Effects { get; set; }
    }
}
