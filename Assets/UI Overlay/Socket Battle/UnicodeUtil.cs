using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity can't set unicode characters in its editor so this converts our syntax to the symbol we want
/// </summary>
public class UnicodeUtil
{
    private static Dictionary<string, string> unicodeMapping = new Dictionary<string, string>()
    {
        // Types
        { "ZZNormal", "\ue901" },
        { "ZZFire", "\ue917" },
        { "ZZWater", "\ue900" },
        { "ZZGrass", "\ue908" },
        { "ZZFlying", "\ue90e" },
        { "ZZFighting", "\ue913" },
        { "ZZDark", "\ue91e" },
        { "ZZPsychic", "\ue916" },
        { "ZZDragon", "\ue91a" },
        { "ZZGround", "\ue90b" },
        { "ZZRock", "\ue925" },
        { "ZZPoison", "\ue91b" },
        { "ZZGhost", "\ue91d" },
        { "ZZIce", "\ue91c" },
        { "ZZSteel", "\ue920" },
        { "ZZBug", "\ue919" },
        { "ZZElectric", "\ue909" },
        { "ZZFairy", "\ue918" },

        // Status Effects
        { "ZZAtk", "\ue904" },
        { "ZZDef", "\uF3ED" },
        { "ZZSpc", "\uF02D" },
        { "ZZEva", "\uF102" },

        { "ZZBlock", "\ue902" },
        { "ZZAttackMult", "\ue926" },
        { "ZZInvul", "\ue922" },
        { "ZZBlind", "\ue914" },
        { "ZZSleep", "\ue923" },

        { "ZZEnergy", "\uf10c" },
        { "ZZCard", "\uf005" }
    };

    /// <summary>
    /// Converts our made up symbol syntax into a unicode symbol.  Make sure you have fallback text on to support this.
    /// </summary>
    /// <param name="stringWithKeyword"></param>
    /// <returns></returns>
    public static string replaceWithUnicode(string stringWithKeyword)
    {
        var keys = unicodeMapping.Keys;
        foreach (KeyValuePair<string, string> entry in unicodeMapping)
        {
            stringWithKeyword = stringWithKeyword.Replace(entry.Key, entry.Value);
        }
        return stringWithKeyword;
    }
}
