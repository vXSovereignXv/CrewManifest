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

        public override void OnAwake()
        {
            if (ManifestBehaviour.GameObjectInstance == null)
                ManifestBehaviour.GameObjectInstance = GameObject.Find("ManifestBehaviour") ?? new GameObject("ManifestBehaviour", typeof(ManifestBehaviour));
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

    public class ManifestBehaviour : MonoBehaviour
    {
        //Game object that keeps us running
        public static GameObject GameObjectInstance;
        public static Settings Settings = new Settings();
        private bool inQueue = false;
        private float interval = 30F;
        private bool ShouldDrawUI = false;

        public void Awake()
        {
            DontDestroyOnLoad(this);
            Resources.LoadAssets();
            Settings.Load();
            InvokeRepeating("RunTasks", interval, interval);
        }
        
        public void Update()
        {
            ShouldDrawUI = FlightGlobals.fetch != null && FlightGlobals.ActiveVessel != null && HighLogic.LoadedScene == GameScenes.FLIGHT;
        }

        public void OnGUI()
        {
            if (ShouldDrawUI != inQueue)
            {
                if (ShouldDrawUI && !inQueue)
                {
                    Debug.Log(string.Format("Manifest Added to queue."));
                    RenderingManager.AddToPostDrawQueue(6, DrawGUI);
                    inQueue = true;
                }
                else
                {
                    Debug.Log(string.Format("Manifest Removed from queue."));
                    RenderingManager.RemoveFromPostDrawQueue(6, DrawGUI);
                    inQueue = false;
                }
            }
        }

        public void DrawGUI()
        {
            try
            {
                SetupGUI();

                if (FlightGlobals.fetch != null && FlightGlobals.ActiveVessel != null && !MapView.MapIsEnabled && !PauseMenu.isOpen)
                {
                    var controller = ManifestController.GetInstance(FlightGlobals.ActiveVessel);
                    var icon = controller.ShowWindow ? Resources.IconOn : Resources.IconOff;
                    if (GUI.Button(Settings.ButtonPosition, new GUIContent(icon, "Click to Show Manifest"), Resources.IconStyle))
                    {
                        controller.ShowWindow = !controller.ShowWindow;
                        if (!controller.ShowWindow)
                            controller.HideAllWindows();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
            }
        }

        public void RunTasks()
        {
            if (Settings.EnablePermaDeath)
                Executekerbals();
            Save();
        }

        private void Save()
        {
            if (HighLogic.LoadedScene == GameScenes.FLIGHT && FlightGlobals.fetch != null && FlightGlobals.ActiveVessel != null)
            {
                Settings.Save();
            }
        }

        private void Executekerbals()
        {
            if (HighLogic.LoadedScene == GameScenes.FLIGHT && FlightGlobals.fetch != null && FlightGlobals.ActiveVessel != null)
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
                    KerbalCrewRoster.CrewRoster.Remove(kerbalsToKill[i]);
                }
            }
        }

        private void SetupGUI()
        {
            GUI.skin = HighLogic.Skin;
            if (Resources.WindowStyle == null)
            {
                Resources.SetStyles();
            }
        }
    }
}
