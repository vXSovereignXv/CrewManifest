using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CrewManifest
{
    public class CrewManifestModule : PartModule
    {
        [KSPEvent(guiActive = true, guiName = "View Manifest", active = true)]
        public void ShowWindow()
        {
            ManifestController.GetInstance(this.vessel).ShowWindow = true;
        }

        [KSPEvent(guiActive = true, guiName = "Hide Manifest", active = true)]
        public void HideWindow()
        {
            ManifestController.GetInstance(this.vessel).ShowWindow = false;
            ManifestController.GetInstance(this.vessel).SelectedPart = null;
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            bool windowActive = ManifestController.GetInstance(this.vessel).ShowWindow;

            Events["ShowWindow"].active = !windowActive;
            Events["HideWindow"].active = windowActive;
        }
    }
}
