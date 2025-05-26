using Assets.GracesScripts.ScriptableObjects;
using System;

namespace Assets.GracesScripts.UI
{
    public class ItemPages : PageWithSlots
    {
        public override Type TypeInPageSlots { get => typeof(SpecialItem); set => throw new NotImplementedException(); }

    }
}
