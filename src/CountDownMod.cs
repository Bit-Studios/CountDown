using UnityEngine;
using KSP.Game;
using SpaceWarp.API.Mods;
using UnityEngine.UIElements;
using SpaceWarp;
using BepInEx;
using KSP.Messages;
using ShadowUtilityLIB;
using ShadowUtilityLIB.UI;
using UitkForKsp2.API;
using Logger = ShadowUtilityLIB.logging.Logger;
using System.Collections;
using Window = UitkForKsp2.API.Window;
using CountDown.UI;
using DragManipulator = CountDown.UI.DragManipulator;

namespace CountDown;
[BepInPlugin("com.shadowdev.countdown", "Count Down", "0.0.3")]
[BepInDependency(ShadowUtilityLIBMod.ModId, ShadowUtilityLIBMod.ModVersion)]
[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
public class CountDownMod : BaseSpaceWarpPlugin
{

    public static string ModId = "com.shadowdev.countdown";
    public static string ModName = "Count Down";
    public static string ModVersion = "0.0.3";

    private Logger logger = new Logger(ModName, ModVersion);

    public static Manager manager;

    private bool showUI = false;
    public static bool IsDev = true;
    private static bool ShowConfig = false;

    public static double Timer = 0.0;

    public static double StartTimer = 0.0;
    public override void OnInitialized()
    {
        logger.Log("Starting");
        try
        {
            manager = new Manager();
            GenerateCountdownWindow();
            GameManager.Instance.Game.Messages.Subscribe<PrelaunchSequenceInitiatedMessage>(CountDownstarted);
            GameManager.Instance.Game.Messages.Subscribe<LaunchSequenceCompleteMessage>(CountDownstopped);
        }
        catch (Exception e)
        {
            logger.Error($"{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}");
        }
        logger.Log("Initialized");
    }
    void Awake()
    {
        if (IsDev)
        {
            logger.Log("CountDown dev mode");
            ShadowUtilityLIBMod.EnableDebugMode();
        }
    }
    public void GenerateCountdownWindow()
    {
        logger.Log("CountDown generating UI");
        try
        {
            VisualElement CountDownClock = Element.Root("CountDownClock");
            CountDownClock.AddManipulator(new DragManipulator());
            CountDownClock.style.width = 300;
            CountDownClock.style.height = 120;
            CountDownClock.style.backgroundColor = new StyleColor(new Color32(0, 0, 0, 255));
            CountDownClock.style.borderRightColor = new StyleColor(new Color32(0, 0, 6, 255));
            CountDownClock.style.borderLeftColor = new StyleColor(new Color32(0, 0, 6, 255));
            CountDownClock.style.borderTopColor = new StyleColor(new Color32(0, 0, 6, 255));
            CountDownClock.style.borderBottomColor = new StyleColor(new Color32(0, 0, 6, 255));
            CountDownClock.style.left = Screen.width / 2;
            CountDownClock.style.top = Screen.height / 5;

            Label CountDownClockTitle = Element.Label("CountDownClockTitle", $"Launch countdown");
            CountDownClockTitle.style.fontSize = 25;
            CountDownClockTitle.style.color = new StyleColor(new Color32(255, 255, 255, 255));
            CountDownClockTitle.style.unityTextAlign = TextAnchor.MiddleCenter;
            CountDownClock.Add(CountDownClockTitle);

            VisualElement CountDownClockSeperator = new VisualElement();
            CountDownClockSeperator.style.paddingBottom = 0;
            CountDownClockSeperator.style.paddingLeft = 0;
            CountDownClockSeperator.style.paddingRight = 0;
            CountDownClockSeperator.style.paddingTop = 0;
            CountDownClockSeperator.style.marginLeft = 0;
            CountDownClockSeperator.style.marginRight = 0;
            CountDownClockSeperator.style.height = 3;
            CountDownClockSeperator.style.width = 300;
            CountDownClockSeperator.style.backgroundColor = new StyleColor(new Color32(0, 0, 0, 255));
            CountDownClock.Add(CountDownClockSeperator);

            Label CountDownClockTimer = Element.Label("CountDownClockTimer", $"T- not initilised, this is a bug");
            CountDownClockTimer.style.fontSize = 40;
            CountDownClockTimer.style.color = new StyleColor(new Color32(255, 0, 0, 255));
            CountDownClockTimer.style.unityTextAlign = TextAnchor.MiddleCenter;
            CountDownClock.Add(CountDownClockTimer);

            UIDocument window = Window.CreateFromElement(CountDownClock);
            manager.Add("CountDownClock", window);
            manager.Set("CountDownClock", showUI);
        }
        catch (Exception e)
        {
            logger.Error($"{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}");
        }
    }
    public void CountDownstarted(MessageCenterMessage message)
    {
        try
        {
            showUI = true;
            manager.Set("CountDownClock", showUI);
            ShadowUtilityLIBMod.RunCr(FillWindow());
            StartTimer = GameManager.Instance.Game.ViewController.universalTime + 13.53;
        }
        catch (Exception e)
        {
            logger.Error($"{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}");
        }
    }
    public void CountDownstopped(MessageCenterMessage message)
    {
        try
        {
            showUI = false;
            manager.Set("CountDownClock", showUI);
        }
        catch (Exception e)
        {
            logger.Error($"{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}");
        }
    }
    public IEnumerator FillWindow()
    {
        while (showUI)
        {
            yield return new WaitForSeconds(0.1f);
            try
            {
                Timer = (StartTimer - GameManager.Instance.Game.ViewController.universalTime);
                Timer = double.Parse(Timer.ToString("F3"));
                if (Timer < -3)
                {
                    showUI = false;
                    manager.Set("CountDownClock", showUI);
                    Timer = 0;
                }
                else
                {
                    if (Timer > 5)
                    {
                        manager.Get("CountDownClock").rootVisualElement.Q<Label>("CountDownClockTimer").text = $"T- {Timer}";
                    }
                    if (Timer > 0 && Timer < 5)
                    {
                        manager.Get("CountDownClock").rootVisualElement.Q<Label>("CountDownClockTimer").text = $"T- {Timer}";
                    }
                    if (Timer < 0)
                    {
                        manager.Get("CountDownClock").rootVisualElement.Q<Label>("CountDownClockTimer").text = $"T+ {-Timer}";
                    }
                    
                }
                
                /*GUILayout.BeginVertical();
                if (GUI.Button(new Rect(windowRect.width - 23, 6, 18, 18), "<b>x</b>", new GUIStyle(GUI.skin.button) { fontSize = 10, }))
                {
                    showUI = false;
                }
                GUILayout.BeginHorizontal();
                if (Timer > 5)
                {
                    GUILayout.Label($"<color=green><b>T- {Timer}</b></color>", new GUIStyle(GUI.skin.button) { fontSize = 50, });
                }
                if (Timer > 0 && Timer < 5)
                {
                    GUILayout.Label($"<color=red><b>T- {Timer}</b></color>", new GUIStyle(GUI.skin.button) { fontSize = 50, });
                }
                if (Timer < 0)
                {
                    GUILayout.Label($"<color=red><b>T+ {-Timer}</b></color>", new GUIStyle(GUI.skin.button) { fontSize = 50, });
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUI.DragWindow(new Rect(0, 0, windowWidth, 700));*/

            }
            catch (Exception e)
            {
                logger.Error($"{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}");
            }
            
        }
    }
}