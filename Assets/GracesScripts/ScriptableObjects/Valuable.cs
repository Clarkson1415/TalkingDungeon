using UnityEngine;

namespace Assets.GracesScripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Valuable", menuName = "DungeonItems/Valuable", order = 1)]
    public class Valuable : DungeonItem
    {
        public override string Path { get => $"Items/Valuable/{this.Name}"; set => throw new System.NotImplementedException(); }
    }
}
