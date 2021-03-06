  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// How the effectiveness is calculated
/// </summary>
public static class TypeChart
{
    const float superEffectiveMult = 1.5f;

    const float notEffectiveMult = 0.667f;

    const float superEffectiveMultMax = 2f;

    const float notEffectiveMultMin = 0.5f;

    /// <summary>
    /// 
    /// Created from https://upload.wikimedia.org/wikipedia/commons/9/97/Pokemon_Type_Chart.svg
    /// </summary>
    private static Dictionary<string, Dictionary<string, float>> typeChart = new Dictionary<string, Dictionary<string, float>>
    {
        { EnergyType.NORMAL, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.GHOST, 0f }, // Defending Type
                { EnergyType.ROCK, notEffectiveMult },
                { EnergyType.STEEL, notEffectiveMult }
            }
        },
        { EnergyType.FIRE, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.GRASS, superEffectiveMult },
                { EnergyType.ICE, superEffectiveMult },
                { EnergyType.BUG, superEffectiveMult },
                { EnergyType.STEEL, superEffectiveMult },

                { EnergyType.FIRE, notEffectiveMult },
                { EnergyType.WATER, notEffectiveMult },
                { EnergyType.ROCK, notEffectiveMult },
                { EnergyType.DRAGON, notEffectiveMult }
            }
        },
        { EnergyType.WATER, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.FIRE, superEffectiveMult },
                { EnergyType.GROUND, superEffectiveMult },
                { EnergyType.ROCK, superEffectiveMult },

                { EnergyType.WATER, notEffectiveMult },
                { EnergyType.GRASS, notEffectiveMult },
                { EnergyType.DRAGON, notEffectiveMult }
            }
        },
        { EnergyType.GRASS, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.GROUND, superEffectiveMult },
                { EnergyType.ROCK, superEffectiveMult },

                { EnergyType.FIRE, notEffectiveMult },
                { EnergyType.GRASS, notEffectiveMult },
                { EnergyType.POISON, notEffectiveMult },
                { EnergyType.FLYING, notEffectiveMult },
                { EnergyType.BUG, notEffectiveMult },
                { EnergyType.DRAGON, notEffectiveMult },
                { EnergyType.STEEL, notEffectiveMult }
            }
        },
        { EnergyType.ELECTRIC, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.WATER, superEffectiveMult },
                { EnergyType.GHOST, superEffectiveMult },

                { EnergyType.GROUND, 0f },

                { EnergyType.GRASS, notEffectiveMult },
                { EnergyType.ELECTRIC, notEffectiveMult },
                { EnergyType.DRAGON, notEffectiveMult }
            }
        },
        { EnergyType.ICE, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.GRASS, superEffectiveMult },
                { EnergyType.GROUND, superEffectiveMult },
                { EnergyType.FLYING, superEffectiveMult },
                { EnergyType.DRAGON, superEffectiveMult },

                { EnergyType.FIRE, notEffectiveMult },
                { EnergyType.WATER, notEffectiveMult },
                { EnergyType.ICE, notEffectiveMult },
                { EnergyType.STEEL, notEffectiveMult }
            }
        },
        { EnergyType.FIGHTING, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.NORMAL, superEffectiveMult },
                { EnergyType.ICE, superEffectiveMult },
                { EnergyType.ROCK, superEffectiveMult },
                { EnergyType.DARK, superEffectiveMult },
                { EnergyType.STEEL, superEffectiveMult },

                { EnergyType.GHOST, 0f },

                { EnergyType.POISON, notEffectiveMult },
                { EnergyType.FLYING, notEffectiveMult },
                { EnergyType.PSYCHIC, notEffectiveMult },
                { EnergyType.BUG, notEffectiveMult },
                { EnergyType.FAIRY, notEffectiveMult }
            }
        },
        { EnergyType.POISON, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.GRASS, superEffectiveMult },
                { EnergyType.FAIRY, superEffectiveMult },

                { EnergyType.STEEL, 0f },

                { EnergyType.POISON, notEffectiveMult },
                { EnergyType.GROUND, notEffectiveMult },
                { EnergyType.ROCK, notEffectiveMult },
                { EnergyType.GHOST, notEffectiveMult }
            }
        },
        { EnergyType.GROUND, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.FIRE, superEffectiveMult },
                { EnergyType.ELECTRIC, superEffectiveMult },
                { EnergyType.POISON, superEffectiveMult },
                { EnergyType.ROCK, superEffectiveMult },
                { EnergyType.STEEL, superEffectiveMult },

                { EnergyType.FLYING, 0f },

                { EnergyType.GRASS, notEffectiveMult },
                { EnergyType.BUG, notEffectiveMult }
            }
        },
        { EnergyType.FLYING, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.GRASS, superEffectiveMult },
                { EnergyType.FIGHTING, superEffectiveMult },
                { EnergyType.BUG, superEffectiveMult },

                { EnergyType.ELECTRIC, notEffectiveMult },
                { EnergyType.ROCK, notEffectiveMult },
                { EnergyType.STEEL, notEffectiveMult }
            }
        },
        { EnergyType.PSYCHIC, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.FIGHTING, superEffectiveMult },
                { EnergyType.POISON, superEffectiveMult },

                { EnergyType.DARK, 0f },

                { EnergyType.PSYCHIC, notEffectiveMult },
                { EnergyType.STEEL, notEffectiveMult }
            }
        },
        { EnergyType.BUG, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.GRASS, superEffectiveMult },
                { EnergyType.PSYCHIC, superEffectiveMult },
                { EnergyType.DARK, superEffectiveMult },

                { EnergyType.FIRE, notEffectiveMult },
                { EnergyType.FIGHTING, notEffectiveMult },
                { EnergyType.POISON, notEffectiveMult },
                { EnergyType.FLYING, notEffectiveMult },
                { EnergyType.GHOST, notEffectiveMult },
                { EnergyType.STEEL, notEffectiveMult },
                { EnergyType.FAIRY, notEffectiveMult }
            }
        },
        { EnergyType.ROCK, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.FIRE, superEffectiveMult },
                { EnergyType.ICE, superEffectiveMult },
                { EnergyType.FLYING, superEffectiveMult },
                { EnergyType.BUG, superEffectiveMult },

                { EnergyType.FIGHTING, notEffectiveMult },
                { EnergyType.GROUND, notEffectiveMult },
                { EnergyType.STEEL, notEffectiveMult }
            }
        },
        { EnergyType.GHOST, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.GRASS, superEffectiveMult },
                { EnergyType.PSYCHIC, superEffectiveMult },
                { EnergyType.DARK, superEffectiveMult },

                { EnergyType.FIRE, notEffectiveMult },
                { EnergyType.FIGHTING, notEffectiveMult },
                { EnergyType.POISON, notEffectiveMult },
                { EnergyType.FLYING, notEffectiveMult },
                { EnergyType.GHOST, notEffectiveMult },
                { EnergyType.STEEL, notEffectiveMult },
                { EnergyType.FAIRY, notEffectiveMult }
            }
        },
        { EnergyType.DRAGON, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.DRAGON, superEffectiveMult },

                { EnergyType.FAIRY, 0f },

                { EnergyType.STEEL, notEffectiveMult }
            }
        },
        { EnergyType.DARK, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.PSYCHIC, superEffectiveMult },
                { EnergyType.GHOST, superEffectiveMult },

                { EnergyType.FIGHTING, notEffectiveMult },
                { EnergyType.DARK, notEffectiveMult },
                { EnergyType.FAIRY, notEffectiveMult }
            }
        },
        { EnergyType.STEEL, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.ICE, superEffectiveMult },
                { EnergyType.ROCK, superEffectiveMult },
                { EnergyType.FAIRY, superEffectiveMult },

                { EnergyType.FIRE, notEffectiveMult },
                { EnergyType.WATER, notEffectiveMult },
                { EnergyType.ELECTRIC, notEffectiveMult },
                { EnergyType.STEEL, notEffectiveMult }
            }
        },
        { EnergyType.FAIRY, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.FIGHTING, superEffectiveMult },
                { EnergyType.DRAGON, superEffectiveMult },
                { EnergyType.DARK, superEffectiveMult },

                { EnergyType.FIRE, notEffectiveMult },
                { EnergyType.POISON, notEffectiveMult },
                { EnergyType.STEEL, notEffectiveMult }
            }
        },
    };
    
    /// <summary>
    /// Returns the effectiveness multiplier. So super effective returns 2, not very effective returns .5
    /// </summary>
    /// <param name="move"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static float getEffectivenessMultiplier(string move, string target)
    {
        float effectiveness = 1;
        var hasValue = typeChart[move].TryGetValue(target, out effectiveness);
        if (!hasValue)
        {
            return 1;
        }
        return effectiveness;
    }

    /// <summary>
    /// Returns the move effectiveness. For the move on the target. 
    /// @assumes the target has 1 or two types
    /// </summary>
    /// <param name="move"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static float getEffectiveness(Card move, Pokemon target)
    {
        if (target.pokemonTypes.Count == 1)
        {
            return getEffectivenessMultiplier(move.damageType, target.pokemonTypes[0]);
        }
        else if (target.pokemonTypes.Count == 2)
        {
            var firstEff = getEffectivenessMultiplier(move.damageType, target.pokemonTypes[0]);
            var secondEff = getEffectivenessMultiplier(move.damageType, target.pokemonTypes[1]);
            return firstEff * secondEff;
        }
        else
        {
            throw new System.Exception("Invalid number of types on target");
        }
    }

    /// <summary>
    /// Returns the move effectiveness. For the move on the target. 
    /// @assumes the target has 1 or two types
    /// </summary>
    /// <param name="move"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static float getEffectiveness(IOpponentMove move, Pokemon target)
    {
        if (target.pokemonTypes.Count == 1)
        {
            return getEffectivenessMultiplier(move.damageEnergy, target.pokemonTypes[0]);
        }
        else if (target.pokemonTypes.Count == 2)
        {
            var firstEff = getEffectivenessMultiplier(move.damageEnergy, target.pokemonTypes[0]);
            var secondEff = getEffectivenessMultiplier(move.damageEnergy, target.pokemonTypes[1]);
            return Mathf.Max(Mathf.Min(firstEff * secondEff, superEffectiveMultMax), notEffectiveMultMin);
        }
        else
        {
            throw new System.Exception("Invalid number of types on target");
        }
    }
}
