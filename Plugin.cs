namespace NoIntro;

using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

/// <summary> miaow mrrrrp https://www.youtube.com/watch?v=p2_pHD230wY </summary>
[BepInIncompatibility(PluginConfig.PluginInfo.PLUGIN_GUID)]
[BepInPlugin("Bryan_-000-.NoMoreIntro", "NoMoreIntro", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    public void Awake() =>
        Harmony.CreateAndPatchAll(GetType(), "Bryan_-000-.NoMoreIntro");

    /// <summary> Load into the main menu instead of tutorial cuz like meowmeowmeowmeowmeowmeowmmeow </summary>
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


                yield return new(OpCodes.Ldstr, "Main Menu");
                yield return new(OpCodes.Ldc_I4_0); // false
            }

            yield return instruction;
        }
    }
}