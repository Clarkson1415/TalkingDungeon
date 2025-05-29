using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.GracesScripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "DungeonItems/Weapon", order = 1)]
    public class Weapon : DungeonItem
    {
        public List<Ability> Abilities = new();

        /// <summary>
        /// Multiplies power of all abilities this weapon can do. Even the defence Abilities also get boosted by this.
        /// </summary>
        public int PowerStat;

        /// <summary>
        /// Defence stat to boost player defence by when equipped.
        /// </summary>
        public int DefenceStat;

        public override string Path { get => $"Items/Weapon/{this.Name}"; set => throw new NotImplementedException("we never need to set this"); }
    }
}
