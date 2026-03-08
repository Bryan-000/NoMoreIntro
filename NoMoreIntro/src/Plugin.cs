namespace NoMoreIntro;

using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

/// <summary> miaow mrrrrp https://www.youtube.com/watch?v=p2_pHD230wY </summary>
[BepInPlugin("Bryan_-000-.NoMoreIntro", "NoMoreIntro", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    /// <summary> Do patch all :P </summary>
    public void Awake() =>
        new Harmony("miaow rawr").PatchAll(GetType());

    /// <summary> Load into the main menu instead of tutorial cuz like meowmeowmeowmeowmeowmeowmmeow </summary>
    [HarmonyTranspiler] [HarmonyPatch(typeof(Bootstrap), "Start")]
    public static IEnumerable<CodeInstruction> LoadIntoMainMenu(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.Is(OpCodes.Ldstr, "Intro"))
                instruction.operand = "Main Menu"; // replace "Intro" with "Main Menu"
            
            yield return instruction;
        }
    }
}