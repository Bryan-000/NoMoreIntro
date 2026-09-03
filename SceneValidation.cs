namespace NoMoreIntro;

using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

public static class SceneValidation
{
    /// <summary> Uses addressables to get all the internal names of every scene in the game. </summary>
    public static HashSet<string> SceneNames
    {
        get
        {
            if (field == null)
            {
                field = [..
                    Addressables.ResourceLocators.SelectMany(locator =>
                        from key in locator.Keys
                        where locator.Locate(key, typeof(SceneInstance), out _)
                        select key.ToString()
                    )
                ];
            }

            return field;
        }
    }

    /// <summary> Checks if input is either an existing level (based on internal names) or a valid informal name for a level. </summary>
    public static bool IsSceneValid(string input) =>
        isNumLevel(input) || (input.ToLower() is "sandbox" or "museum" or "credits" or "cybergrind" or "cg") || SceneNames.Contains(input);

    /// <summary> Converts the informal names of levels to the internal names. </summary>
    public static string ConvertToInternal(string input, string Default = "Main Menu")
    {
        // check if the input is like '1-4' and convert it to 'Level 1-4'
        if (isNumLevel(input))
            input = "Level " + (input[0] + "-" + input[2]).ToUpper();

        input = input.ToLower() switch
        {
            "sandbox" => "uk_construct",
            "cybergrind" or "cg" => "Endless",
            "museum" or "credits" => "CreditsMuseum2",
            "tutorial" or "level 0-0" => "Tutorial",
            _ => input
        };

        // check if input exists
        if (SceneNames.Contains(input))
            return input;

        return Default;
    }

    public static readonly Dictionary<char, HashSet<char>> LevelNums = new()
    {
        ['0'] = ['0', '1', '2', '3', '4', '5', 'S', 'E'],

        ['1'] = ['1', '2', '3', '4', 'S', 'E'],
        ['2'] = ['1', '2', '3', '4', 'S'],
        ['3'] = ['1', '2'],

        ['4'] = ['1', '2', '3', '4', 'S'],
        ['5'] = ['1', '2', '3', '4', 'S'],
        ['6'] = ['1', '2'],

        ['7'] = ['1', '2', '3', '4', 'S'],
        ['8'] = ['1', '2', '3', '4'],

        ['P'] = ['1', '2'],
    };

    /// <summary> Checks if inputs such as '1 4' should be turned into 'Level 1-4'. </summary>
    public static bool isNumLevel(string input)
    {
        if (input.Length == 3 && (input[1] is '-' or ' '))
        {
            char Layer = char.ToUpper(input[0]);
            char Level = char.ToUpper(input[2]);

            if (LevelNums.TryGetValue(Layer, out var ValidLevels) && ValidLevels.Contains(Level))
                return true;
        }

        return false;
    }
}
