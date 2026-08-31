namespace NoIntro;

using BepInEx;
using HarmonyLib;
using PluginConfig.API;
using PluginConfig.API.Fields;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

/// <summary> miaow mrrrrp https://www.youtube.com/watch?v=p2_pHD230wY </summary>
[BepInIncompatibility(PluginConfig.PluginInfo.PLUGIN_GUID)]
[BepInPlugin("Bryan_-000-.NoIntro", "NoIntro", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    public static PluginConfigurator config;
    public static StringField StartScene;

    public static Sprite GrabIcon()
    {
        using Stream stream = typeof(Plugin).Assembly.GetManifestResourceStream("icon.png");
        byte[] data = new byte[stream.Length];
        stream.Read(data, 0, data.Length);

        Texture2D icon_tex = new(0, 0);
        if (icon_tex.LoadImage(data))
            return Sprite.Create(icon_tex, new(0, 0, icon_tex.width, icon_tex.height), new(0.5f, 0.5f));

        return null;
    }

    public void Awake()
    {
        config = PluginConfigurator.Create("NoIntro", "Bryan_-000-.NoIntro");
        config.presetButtonHidden = true; // fuck u
        config.image = GrabIcon();
        StartScene = new(config.rootPanel, "Start Scene", "startscene", "Main Menu");

        Harmony.CreateAndPatchAll(GetType(), "Bryan_-000-.NoIntro");
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


                yield return new(OpCodes.Ldstr, StartScene.value);
                yield return new(OpCodes.Ldc_I4_0); // false
            }

            yield return instruction;
        }
    }
}