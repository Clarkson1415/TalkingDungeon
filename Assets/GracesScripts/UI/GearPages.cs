using Assets.GracesScripts.ScriptableObjects;
using System;

namespace Assets.GracesScripts.UI
{
    public class GearPages : PageWithSlots
    {
        public override Type TypeInPageSlots { get => typeof(Weapon); set => throw new NotImplementedException(); }
    }
}
        