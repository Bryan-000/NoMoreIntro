namespace NoMoreIntro;

using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using PluginConfig;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceProviders;
using static BepInEx.BepInDependency;

/// <summary> miaow mrrrrp https://www.youtube.com/watch?v=p2_pHD230wY </summary>
[BepInDependency(PluginConfiguratorController.PLUGIN_GUID, DependencyFlags.SoftDependency)]
[BepInPlugin("Bryan_-000-.NoMoreIntro", "NoMoreIntro", "2.0.0")]
public class Plugin : BaseUnityPlugin
{
    public static HashSet<string> ValidSceneNames
    {
        get
        {
            if (field == null)
            {
                IResourceLocator MainAddressablesLocator = Addressables.ResourceLocators
                    .FirstOrDefault(loc => loc.LocatorId == "AddressablesMainContentCatalog");

                field = [..
                    from key in MainAddressablesLocator.Keys
                    where MainAddressablesLocator.Locate(key, typeof(SceneInstance), out _)
                    select key.ToString()
                ];
            }

            return field;
        }
    }

    public static ConfigEntry<string> StartScene;

    public void Awake()
    {
        StartScene = Config.Bind("Settings", "startscene", "Main Menu", "The level to load into when the game starts.");
        if (!ValidSceneNames.Contains(StartScene.Value))
            StartScene.Value = "Main Menu";

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