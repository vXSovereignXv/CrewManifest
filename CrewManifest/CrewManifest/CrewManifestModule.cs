using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CrewManifest
{
    public class CrewManifestModule : PartModule
    {
        [KSPEvent(guiActive = true, guiName = "Destroy Part", active = true)]
        public void DestoryPart()
        {
            if (this.part != null)
                this.part.temperature = 5000;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if(this.part != null && part.name == "crewManifest")
                Events["DestoryPart"].active = true;
            else
                Events["DestoryPart"].active = false;
        }
    }

    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class ManifestBehaviour : MonoBehaviour
    {
        //Game object that keeps us running
        public static GameObject GameObjectInstance;
        public static SettingsManager Settings = new SettingsManager();
        private float interval = 30F;
        private float intervalCrewCheck = 0.5f;

        public void Awake()
        {
            DontDestroyOnLoad(this);
            Resources.LoadAssets();
            Settings.Load();
            InvokeRepeating("RunSave", interval, interval);
            InvokeRepeating("CrewCheck", intervalCrewCheck, intervalCrewCheck);
        }

        public void OnDestroy()
        {
            CancelInvoke("RunSave");
            CancelInvoke("CrewCheck");
        }

        public void OnGUI()
        {
            Resources.SetupGUI();
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                DrawButton();
                if (Settings.ShowSettings)
                    Settings.DrawSettingsGUI();
            }

            if(Settings.ShowDebugger)
                Settings.DebuggerPosition = GUILayout.Window(398643, Settings.DebuggerPosition, DrawDebugger, "Manifest Debug Console", GUILayout.MinHeight(20));
        }
        
        public void Update()
        {
            if (FlightGlobals.fetch != null && FlightGlobals.ActiveVessel != null)
            {
                if (HighLogic.LoadedScene == GameScenes.FLIGHT)
                {
                    //Instantiate the controller for the active vessel.
                    ManifestController.GetInstance(FlightGlobals.ActiveVessel).CanDrawButton = true;
                }
            }
        }

        public void CrewCheck()
        {
            if (Settings.EnablePermaDeath)
                Executekerbals();
            CheckKerbalInconsistency();
        }

        public void RunSave()
        {
            Save();
        }

        private void Save()
        {
            if (HighLogic.LoadedScene == GameScenes.FLIGHT && FlightGlobals.fetch != null && FlightGlobals.ActiveVessel != null)
            {
                ManifestUtilities.LogMessage("Saving Manifest Settings...", "Info");
                Settings.Save();
            }
        }

        private void Executekerbals()
        {
            //Persistence file doesn't look to be saved outside of the flight scene. Make changes there.
            if (HighLogic.LoadedScene == GameScenes.FLIGHT && FlightGlobals.fetch != null)
            {
                List<ProtoCrewMember> kerbalsToKill = new List<ProtoCrewMember>();

                for (int i = 0; i < KerbalCrewRoster.CrewRoster.Count; i++)
                {
                    var kerbal = KerbalCrewRoster.CrewRoster[i];
                    if (kerbal.rosterStatus == ProtoCrewMember.RosterStatus.RESPAWN) //Dead isn't used
                        kerbalsToKill.Add(kerbal);
                }

                for (int i = kerbalsToKill.Count - 1; i >= 0; i--)
                {
                    ManifestUtilities.LogMessage(string.Format("{0} is dead. removing from roster", kerbalsToKill[i].name), "Info");
                    KerbalCrewRoster.CrewRoster.Remove(kerbalsToKill[i]);
                }
            }
        }

        private void CheckKerbalInconsistency()
        {
            if (HighLogic.LoadedScene == GameScenes.FLIGHT && FlightGlobals.fetch != null && FlightGlobals.ActiveVessel != null)
            {
                var activeCrew = FlightGlobals.ActiveVessel.GetVesselCrew();
                foreach (var kerbal in activeCrew)
                {
                    //If kerbals are in the vessel they should be assigned. I've seen the state get messed up
                    //when restarting a flight.
                    if(kerbal.rosterStatus != ProtoCrewMember.RosterStatus.ASSIGNED)
                        kerbal.rosterStatus = ProtoCrewMember.RosterStatus.ASSIGNED;

                    if (!KerbalCrewRoster.CrewRoster.Contains(kerbal))
                    {
                        //Add kerbal back if they are in vessel but not roster
                        ManifestUtilities.LogMessage(string.Format("Could not find {0}. Adding back to roster...", kerbal.name), "Info");
                        KerbalCrewRoster.CrewRoster.Add(kerbal);
                    }
                }
            }
        }

        private void DrawButton()
        {
            var icon = Settings.ShowSettings ? Resources.IconOn : Resources.IconOff;
            if (GUI.Button(Settings.ButtonPosition, new GUIContent(icon, "Manifest Settings"), Resources.IconStyle))
            {
                Settings.ShowSettings = true;
            }
        }

        private void DrawDebugger(int windowId)
        {
            GUILayout.BeginVertical();

            ManifestUtilities.DebugScrollPosition = GUILayout.BeginScrollView(ManifestUtilities.DebugScrollPosition, GUILayout.Height(300), GUILayout.Width(500));
            GUILayout.BeginVertical();

            foreach(string error in ManifestUtilities.Errors)
                GUILayout.TextArea(error, GUILayout.Width(460));

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, Screen.width, 30));
        }
    }
}
