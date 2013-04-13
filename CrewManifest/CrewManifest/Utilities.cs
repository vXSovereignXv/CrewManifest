using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CrewManifest
{
    public class ManifestUtilities
    {
        public static String AppPath = KSPUtil.ApplicationRootPath.Replace("\\", "/");
        public static String PlugInPath = AppPath + "PluginData/CrewManifest/";

        public static void LoadTexture(ref Texture2D tex, String FileName)
        {
            WWW img1 = new WWW(String.Format("file://{0}{1}", PlugInPath, FileName));
            img1.LoadImageIntoTexture(tex);
        }
    }

    public class Resources
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

    public class Settings
    {
        public Rect ManifestPosition;
        public Rect TransferPosition;
        public Rect RosterPosition;
        public Rect ButtonPosition;
        public bool EnablePermaDeath;

        public void Load()
        {
            try
            {
                KSP.IO.PluginConfiguration configfile = KSP.IO.PluginConfiguration.CreateForType<CrewManifestModule>();
                configfile.load();

                ManifestPosition = configfile.GetValue<Rect>("ManifestPosition");
                TransferPosition = configfile.GetValue<Rect>("TransferPosition");
                RosterPosition = configfile.GetValue<Rect>("RosterPosition");
                ButtonPosition = configfile.GetValue<Rect>("ButtonPosition");
                EnablePermaDeath = configfile.GetValue<bool>("EnablePermaDeath");
            }
            catch
            {

            }
        }

        public void Save()
        {
            try
            {
                KSP.IO.PluginConfiguration configfile = KSP.IO.PluginConfiguration.CreateForType<CrewManifestModule>();

                configfile.SetValue("ManifestPosition", this.ManifestPosition);
                configfile.SetValue("TransferPosition", this.TransferPosition);
                configfile.SetValue("RosterPosition", this.RosterPosition);
                configfile.SetValue("ButtonPosition", this.ButtonPosition);
                configfile.SetValue("EnablePermaDeath", this.EnablePermaDeath);

                configfile.save();
            }
            catch { }
        }
    }
}
