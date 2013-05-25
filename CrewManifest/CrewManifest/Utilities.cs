using KSP.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CrewManifest
{
    public static class ManifestUtilities
    {
        public static String AppPath = KSPUtil.ApplicationRootPath.Replace("\\", "/");
        public static String PlugInPath = AppPath + "GameData/CrewManifest/Plugins/PluginData/crewmanifest/";
        public static Vector2 DebugScrollPosition = Vector2.zero;

        private static List<string> _errors = new List<string>();
        public static List<string> Errors
        {
            get { return _errors; }
        }

        public static void LoadTexture(ref Texture2D tex, String FileName)
        {
            LogMessage(String.Format("Loading Texture - file://{0}{1}", PlugInPath, FileName), "Info");
            WWW img1 = new WWW(String.Format("file://{0}{1}", PlugInPath, FileName));
            img1.LoadImageIntoTexture(tex);
        }

        public static void LogMessage(string error, string type)
        {
            _errors.Add(type + ": " + error);
        }
    }

    public static class Resources
    {
        public static Texture2D IconOff = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        public static Texture2D IconOn = new Texture2D(32, 32, TextureFormat.ARGB32, false);

        public static GUIStyle WindowStyle;
        public static GUIStyle IconStyle;
        public static GUIStyle ButtonToggledStyle;
        public static GUIStyle ButtonToggledRedStyle;
        public static GUIStyle ButtonStyle;
        public static GUIStyle ErrorLabelRedStyle;
        public static GUIStyle LabelStyle;
        public static GUIStyle LabelStyleRed;

        public static void SetupGUI()
        {
            GUI.skin = HighLogic.Skin;
            if (WindowStyle == null)
            {
                SetStyles();
            }
        }

        public static void LoadAssets()
        {
            ManifestUtilities.LoadTexture(ref IconOff, "IconOff.png");
            ManifestUtilities.LoadTexture(ref IconOn, "IconOn.png");
        }

        public static void SetStyles()
        {
            WindowStyle = new GUIStyle(GUI.skin.window);
            IconStyle = new GUIStyle();

            ButtonToggledStyle = new GUIStyle(GUI.skin.button);
            ButtonToggledStyle.normal.textColor = Color.green;
            ButtonToggledStyle.normal.background = ButtonToggledStyle.onActive.background;

            ButtonToggledRedStyle = new GUIStyle(ButtonToggledStyle);
            ButtonToggledRedStyle.normal.textColor = Color.red;

            ButtonStyle = new GUIStyle(GUI.skin.button);
            ButtonStyle.normal.textColor = Color.white;

            ErrorLabelRedStyle = new GUIStyle(GUI.skin.label);
            ErrorLabelRedStyle.normal.textColor = Color.red;
            ErrorLabelRedStyle.fontSize = 10;

            LabelStyle = new GUIStyle(GUI.skin.label);

            LabelStyleRed = new GUIStyle(LabelStyle);
            LabelStyleRed.normal.textColor = Color.red;
        }
    }

    public class SettingsManager
    {
        public Rect ManifestPosition;
        public Rect TransferPosition;
        public Rect RosterPosition;
        public Rect ButtonPosition;
        public Rect SettingsPosition;
        public bool EnablePermaDeath;

        public Rect PrevManifestPosition;
        public Rect PrevTransferPosition;
        public Rect PrevRosterPosition;
        public Rect PrevButtonPosition;
        public bool PrevEnablePermaDeath;

        public Rect DebuggerPosition;
        public bool ShowDebugger;

        public void Load()
        {
            ManifestUtilities.LogMessage("Settings load started...", "Info");

            try
            {
                KSP.IO.PluginConfiguration configfile = KSP.IO.PluginConfiguration.CreateForType<CrewManifestModule>();
                configfile.load();

                ManifestPosition = PrevManifestPosition = configfile.GetValue<Rect>("ManifestPosition");
                TransferPosition = PrevTransferPosition = configfile.GetValue<Rect>("TransferPosition");
                RosterPosition = PrevRosterPosition = configfile.GetValue<Rect>("RosterPosition");
                ButtonPosition = PrevButtonPosition = configfile.GetValue<Rect>("ButtonPosition");
                SettingsPosition = new Rect(ButtonPosition.xMin > Screen.width - 200 ? Screen.width - 200 : ButtonPosition.xMin, ButtonPosition.yMax + 5 > Screen.height - 200 ? Screen.height - 200 : ButtonPosition.yMax + 5, 200, 20);
                EnablePermaDeath = PrevEnablePermaDeath = configfile.GetValue<bool>("EnablePermaDeath");
                DebuggerPosition = configfile.GetValue<Rect>("DebuggerPosition");
                ShowDebugger = configfile.GetValue<bool>("ShowDebugger");

                ManifestUtilities.LogMessage(string.Format("ManifestPosition Loaded: {0}, {1}, {2}, {3}", ManifestPosition.xMin, ManifestPosition.xMax, ManifestPosition.yMin, ManifestPosition.yMax), "Info");
                ManifestUtilities.LogMessage(string.Format("TransferPosition Loaded: {0}, {1}, {2}, {3}", TransferPosition.xMin, TransferPosition.xMax, TransferPosition.yMin, TransferPosition.yMax), "Info");
                ManifestUtilities.LogMessage(string.Format("RosterPosition Loaded: {0}, {1}, {2}, {3}", RosterPosition.xMin, RosterPosition.xMax, RosterPosition.yMin, RosterPosition.yMax), "Info");
                ManifestUtilities.LogMessage(string.Format("ButtonPosition Loaded: {0}, {1}, {2}, {3}", ButtonPosition.xMin, ButtonPosition.xMax, ButtonPosition.yMin, ButtonPosition.yMax), "Info");
                ManifestUtilities.LogMessage(string.Format("DebuggerPosition Loaded: {0}, {1}, {2}, {3}", DebuggerPosition.xMin, DebuggerPosition.xMax, DebuggerPosition.yMin, DebuggerPosition.yMax), "Info");
                ManifestUtilities.LogMessage(string.Format("EnablePermaDeath Loaded: {0}", EnablePermaDeath.ToString()), "Info");
                ManifestUtilities.LogMessage(string.Format("ShowDebugger Loaded: {0}", ShowDebugger.ToString()), "Info");
            }
            catch(Exception e)
            {
                ManifestUtilities.LogMessage(string.Format("Failed to Load Settings: {0} \r\n\r\n{1}", e.Message, e.StackTrace), "Exception");
            }
        }

        public void Save()
        {
            try
            {
                KSP.IO.PluginConfiguration configfile = KSP.IO.PluginConfiguration.CreateForType<CrewManifestModule>();

                configfile.SetValue("ManifestPosition", ManifestPosition);
                configfile.SetValue("TransferPosition", TransferPosition);
                configfile.SetValue("RosterPosition", RosterPosition);
                configfile.SetValue("ButtonPosition", ButtonPosition);
                configfile.SetValue("DebuggerPosition", DebuggerPosition);
                configfile.SetValue("EnablePermaDeath", EnablePermaDeath);
                configfile.SetValue("ShowDebugger", ShowDebugger);

                configfile.save();

                PrevManifestPosition = ManifestPosition;
                PrevTransferPosition = TransferPosition;
                PrevRosterPosition = RosterPosition;
                PrevButtonPosition = ButtonPosition;
                PrevEnablePermaDeath = EnablePermaDeath;
                SettingsPosition = new Rect(ButtonPosition.xMin > Screen.width - 200 ? Screen.width - 200 : ButtonPosition.xMin, ButtonPosition.yMax + 5 > Screen.height - 200 ? Screen.height - 200 : ButtonPosition.yMax + 5, 200, 20);

                ManifestUtilities.LogMessage(string.Format("ManifestPosition Saved: {0}, {1}, {2}, {3}", ManifestPosition.xMin, ManifestPosition.xMax, ManifestPosition.yMin, ManifestPosition.yMax), "Info");
                ManifestUtilities.LogMessage(string.Format("TransferPosition Saved: {0}, {1}, {2}, {3}", TransferPosition.xMin, TransferPosition.xMax, TransferPosition.yMin, TransferPosition.yMax), "Info");
                ManifestUtilities.LogMessage(string.Format("RosterPosition Saved: {0}, {1}, {2}, {3}", RosterPosition.xMin, RosterPosition.xMax, RosterPosition.yMin, RosterPosition.yMax), "Info");
                ManifestUtilities.LogMessage(string.Format("ButtonPosition Saved: {0}, {1}, {2}, {3}", ButtonPosition.xMin, ButtonPosition.xMax, ButtonPosition.yMin, ButtonPosition.yMax), "Info");
                ManifestUtilities.LogMessage(string.Format("DebuggerPosition Saved: {0}, {1}, {2}, {3}", DebuggerPosition.xMin, DebuggerPosition.xMax, DebuggerPosition.yMin, DebuggerPosition.yMax), "Info");
                ManifestUtilities.LogMessage(string.Format("EnablePermaDeath Saved: {0}", EnablePermaDeath.ToString()), "Info");
                ManifestUtilities.LogMessage(string.Format("ShowDebugger Saved: {0}", ShowDebugger.ToString()), "Info");
            }
            catch (Exception e)
            {
                ManifestUtilities.LogMessage(string.Format("Failed to Save Settings: {0} \r\n\r\n{1}", e.Message, e.StackTrace), "Exception");
            }
        }

        public bool ShowSettings { get; set; }

        private float ButtonLeftPostition
        {
            get
            {
                return ButtonPosition.xMin;
            }
            set
            {
                ButtonPosition.xMin = value;
                ButtonPosition.xMax = value + 32;
            }
        }

        private float ButtonTopPostition
        {
            get
            {
                return ButtonPosition.yMin;
            }
            set
            {
                ButtonPosition.yMin = value;
                ButtonPosition.yMax = value + 32;
            }
        }

        public void DrawSettingsGUI()
        {
            SettingsPosition = GUILayout.Window(398544, SettingsPosition, SettingsWindow, "Crew Manifest Settings", GUILayout.MinHeight(20));
        }

        private void SettingsWindow(int windowId)
        {
            GUILayout.BeginVertical();
            GUILayout.Label(string.Format("Button Position ({0}, {1})", ButtonTopPostition, ButtonLeftPostition));

            GUILayout.BeginHorizontal();
            GUILayout.Label("Top", GUILayout.Width(25));
            ButtonTopPostition = GUILayout.HorizontalSlider(ButtonTopPostition, 0, Screen.height - 32);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
                GUILayout.Label("Left", GUILayout.Width(25));
                ButtonLeftPostition = GUILayout.HorizontalSlider(ButtonLeftPostition, 0, Screen.width - 32);
            GUILayout.EndHorizontal();

            string label = EnablePermaDeath ? "Permadeath Enabled" : "Permadeath Disabled";
            EnablePermaDeath = GUILayout.Toggle(EnablePermaDeath, label);

            label = ShowDebugger ? "Hide Debug Console" : "Show Debug Console";
            ShowDebugger = GUILayout.Toggle(ShowDebugger, label);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                Save();
                ShowSettings = false;
            }
            if (GUILayout.Button("Cancel"))
            {
                ManifestPosition = PrevManifestPosition;
                TransferPosition = PrevTransferPosition;
                RosterPosition = PrevRosterPosition;
                ButtonPosition = PrevButtonPosition;
                EnablePermaDeath = PrevEnablePermaDeath;
                ShowSettings = false;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, Screen.width, 30));
        }
    }
}
