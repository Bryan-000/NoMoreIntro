namespace NoMoreIntro;

using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using PluginConfig;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using static BepInEx.BepInDependency;

/// <summary> miaow mrrrrp https://www.youtube.com/watch?v=p2_pHD230wY </summary>
[BepInDependency(PluginConfiguratorController.PLUGIN_GUID, DependencyFlags.SoftDependency)]
[BepInPlugin("Bryan_-000-.NoMoreIntro", "NoMoreIntro", "1.7.0")]
public class Plugin : BaseUnityPlugin
{
    public static ConfigEntry<string> StartScene;

    public void Awake()
    {
        StartScene = Config.Bind("Settings", "startscene", "Main Menu", "The level to load into when the game starts.");
        PConfigGUI.SafeLoad();

        Harmony.CreateAndPatchAll(GetType(), "Bryan_-000-.NoMoreIntro");
    }

    /// <summary> Load into the specified startscene instead of intro/tutorial cuz like meowmeowmeowmeowmeowmeowmmeow </summary>
    [HarmonyTranspiler] [HarmonyPatch(typeof(Bootstrap), "Start")]
    public static IEnumerable<CodeInstruction> LoadIntoMainMenu(IEnumerable<CodeInstruction> instructions)
    {
        MethodInfo LoadScene = AccessTools.Method(typeof(SceneHelper), "LoadScene");

        foreach (CodeInstruction instruction in instructions)
        {
            // replace parameters for SceneHelper.LoadScene()
            if (instruction.Calls(LoadScene))
            {
                yield return new(OpCodes.Pop);
                yield return new(OpCodes.Pop);


                yield return new(OpCodes.Ldstr, StartScene.Value);
                yield return new(OpCodes.Ldc_I4_0); // false
            }

            yield return instruction;
        }
    }
}