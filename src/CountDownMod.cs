using UnityEngine;
using KSP.Game;
using SpaceWarp.API.Mods;
using Screen = UnityEngine.Screen;
using KSP.UI.Binding;
using KSP.Sim.impl;
using KSP.Sim;
using KSP.Sim.Definitions;
using KSP.OAB;
using KSP.Modules;
using Shapes;
using SpaceWarp.API.Assets;
using SpaceWarp;
using BepInEx;
using SpaceWarp.API.UI.Appbar;
using KSP.Messages;
using Steamworks;
using Unity.Mathematics;
using KSP.Messages.PropertyWatchers;
using KSP.Sim.State;
using KSP;
using System.IO.Ports;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace CountDown;
[BepInPlugin("com.shadowdev.countdown", "Count Down", "0.0.1")]
[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
public class SuicideBurnMod : BaseSpaceWarpPlugin
{
    private int windowWidth = 300;
    private int windowHeight = 700;
    private Rect windowRect;
    private static GUIStyle boxStyle;
    private bool showUI = false;
    private static string SetWindowWidthStr = "300";
    public static bool IsDev = false;
    private static bool ShowConfig = false;
    public static double Timer = 0.0;
    public static double StartTimer = 0.0;
    public override void OnInitialized()
    {
        Appbar.RegisterAppButton(
           "Countdown",
            "BTN-CD",
            AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/icon.png"), ToggleButton);
        GameManager.Instance.Game.Messages.Subscribe<PrelaunchSequenceInitiatedMessage>(CountDownstarted);
        GameManager.Instance.Game.Messages.Subscribe<LaunchSequenceCompleteMessage>(CountDownstopped);
    }
    void Awake()
    {
        windowRect = new Rect((Screen.width * 0.85f) - (windowWidth / 2), (Screen.height / 2) - (windowHeight / 2), 0, 0);
    }
    void Update()
    {
        if (!IsDev)
        {
            
        }
    }
    public void CountDownstarted(MessageCenterMessage message)
    {
        showUI = true;
        StartTimer = GameManager.Instance.Game.ViewController.universalTime + 13.6;
    }
    public void CountDownstopped(MessageCenterMessage message)
    {
        showUI = false;
        
    }
    void ToggleButton(bool toggle)
    {
        showUI = toggle;
        
        GameObject.Find("BTN-CD")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(toggle);
        
    }
    void OnGUI()
    {
        GUI.skin = SpaceWarp.API.UI.Skins.ConsoleSkin;
        if (showUI)
        {
            windowRect = GUILayout.Window(
                GUIUtility.GetControlID(FocusType.Passive),
                windowRect,
                FillWindow,
                "Count Down",
                GUILayout.Height(0),
                GUILayout.Width(windowWidth));
        }
    }
    private void FillWindow(int windowID)
    {
        Timer = (StartTimer - GameManager.Instance.Game.ViewController.universalTime);
        boxStyle = GUI.skin.GetStyle("Box");
        if(Timer < 0)
        {
            showUI = false;
            Timer = 0;
        }
        GUILayout.BeginVertical();
        if (GUI.Button(new Rect(windowRect.width - 23, 6, 18, 18), "<b>x</b>", new GUIStyle(GUI.skin.button) { fontSize = 10, }))
        {
            showUI = false;
        }
        GUILayout.BeginHorizontal();
        if(Timer > 5)
        {
            GUILayout.Label($"<color=green><b>T- {Timer}</b></color>", new GUIStyle(GUI.skin.button) { fontSize = 50, });
        }
        else
        {
            GUILayout.Label($"<color=red><b>T- {Timer}</b></color>", new GUIStyle(GUI.skin.button) { fontSize = 50, });
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUI.DragWindow(new Rect(0, 0, windowWidth, 700));
    }
}