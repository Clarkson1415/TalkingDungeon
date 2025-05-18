using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.GracesScripts.UI
{
    class SaveMenuPages : BookPage
    {
        public override void TogglePageComponents(bool OnOff)
        {
            base.ToggleChildComponents(OnOff);
        }
    }
}
