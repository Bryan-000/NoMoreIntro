namespace NoMoreIntro;

using BepInEx.Bootstrap;
using PluginConfig;
using PluginConfig.API;
using PluginConfig.API.Decorators;
using PluginConfig.API.Fields;
using System.IO;
using UnityEngine;

public static class PConfigGUI
{
    public static PluginConfigurator config;
    public static StringField StartScene;

    public static void SafeLoad()
    {
        try
        {
            if (Chainloader.PluginInfos.ContainsKey(PluginConfiguratorController.PLUGIN_GUID))
                LoadConfig();
        }
        catch { }
    }

    public static void LoadConfig()
    {
        config = PluginConfigurator.Create("NoMoreIntro", "Bryan_-000-.NoMoreIntro");
        config.presetButtonHidden = true; // fuck u
        config.image = GrabIcon();

        StartScene = new(config.rootPanel, "StartScene", "bryan000.nomoreintro.startScene", "Main Menu", false, false);
        StartScene.value = Plugin.StartScene.Value;

        ConfigHeader description = new(config.rootPanel, "The level to load into when the game starts.", 14);
        description.textColor = Color.gray;

        StartScene.onValueChange += data => Plugin.StartScene.Value = data.value;
    }

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
}