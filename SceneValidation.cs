namespace NoMoreIntro;

using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
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

    /// <summary> Checks if input is either an existing level (based on internal names) or a valid informal name for a level. </summary>
    public static bool IsSceneValid(string input) =>
        isNumLevel(input) || (input.ToLower() is "sandbox" or "museum" or "cybergrind" or "cg") || SceneNames.Contains(input);

    /// <summary> Converts the informal names of levels to the internal names. </summary>
    public static string ConvertToInternal(string input, string Default = "Main Menu")
    {
        // check if the input is like '1-4' and convert it to 'Level 1-4'
        if (isNumLevel(input))
        {
            input = "Level " + input[0] + '-' + input[2];
        }

        input = input.ToLower() switch
        {
            "sandbox" => "uk_construct",
            "museum" => "CreditsMuseum2",
            "cybergrind" or "cg" => "Endless",
            "tutorial" or "level 0-0" => "Tutorial",
            _ => input
        };

        // check if input exists
        if (SceneNames.Contains(input))
            return input;

        return Default;
    }

    /// <summary> Checks if inputs such as '1 4' should be turned into 'Level 1-4'. </summary>
    private static bool isNumLevel(string input)
    {
        if (input.Length == 3 && (input[1] is '-' or ' '))
        {
            char Layer = char.ToUpper(input[0]);
            char Level = char.ToUpper(input[2]);

            // `(uint)Level - '1' < 4` checks if Level is between 1 and 4 (or well 1, 2, 3, or 4)
            if ((Layer is '0' && ((uint)Level - '0' <= 5 || Level is 'S' or 'E'))

             || (Layer is '1' && ((uint)Level - '1' < 4 || Level is 'S' or 'E'))
             || (Layer is '2' && ((uint)Level - '1' < 4 || Level is 'S'))
             || (Layer is '3' && ((uint)Level - '1' < 2))

             || (Layer is '4' && ((uint)Level - '1' < 4 || Level is 'S'))
             || (Layer is '5' && ((uint)Level - '1' < 4 || Level is 'S'))
             || (Layer is '6' && ((uint)Level - '1' < 2))

             || (Layer is '7' && ((uint)Level - '1' < 4 || Level is 'S'))
             || (Layer is '8' && ((uint)Level - '1' < 4))

             || (Layer is 'P' && ((uint)Level - '1' < 2)))
            {
                return true;
            }
        }

        return false;
    }
}
