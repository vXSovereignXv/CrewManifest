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

    public class CrewManifest: KSP.Testing.UnitTest
    {
        public CrewManifest()
            : base()
        {
            if (ManifestBehaviour.GameObjectInstance == null)
                ManifestBehaviour.GameObjectInstance = GameObject.Find("ManifestBehaviour") ?? new GameObject("ManifestBehaviour", typeof(ManifestBehaviour));
        }
    }

    public class ManifestBehaviour : MonoBehaviour
    {
        //Game object that keeps us running
        public static GameObject GameObjectInstance;
        public static Settings Settings = new Settings();
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
                        KerbalCrewRoster.CrewRoster.Add(kerbal);
                    }
                }
            }
        }
    }
}
