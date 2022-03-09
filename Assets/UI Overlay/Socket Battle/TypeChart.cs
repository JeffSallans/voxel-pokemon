  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// How the effectiveness is calculated
/// </summary>
public static class TypeChart
{
    /// <summary>
    /// 
    /// Created from https://upload.wikimedia.org/wikipedia/commons/9/97/Pokemon_Type_Chart.svg
    /// </summary>
    private static Dictionary<string, Dictionary<string, float>> typeChart = new Dictionary<string, Dictionary<string, float>>
    {
        { EnergyType.NORMAL, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.GHOST, 0f }, // Defending Type
                { EnergyType.ROCK, .5f },
                { EnergyType.STEEL, .5f }
            }
        },
        { EnergyType.FIRE, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.GRASS, 2f },
                { EnergyType.ICE, 2f },
                { EnergyType.BUG, 2f },
                { EnergyType.STEEL, 2f },

                { EnergyType.FIRE, .5f },
                { EnergyType.WATER, .5f },
                { EnergyType.ROCK, .5f },
                { EnergyType.DRAGON, .5f }
            }
        },
        { EnergyType.WATER, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.FIRE, 2f },
                { EnergyType.GROUND, 2f },
                { EnergyType.ROCK, 2f },

                { EnergyType.WATER, .5f },
                { EnergyType.GRASS, .5f },
                { EnergyType.DRAGON, .5f }
            }
        },
        { EnergyType.GRASS, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.GROUND, 2f },
                { EnergyType.ROCK, 2f },

                { EnergyType.FIRE, .5f },
                { EnergyType.GRASS, .5f },
                { EnergyType.POISON, .5f },
                { EnergyType.FLYING, .5f },
                { EnergyType.BUG, .5f },
                { EnergyType.DRAGON, .5f },
                { EnergyType.STEEL, .5f }
            }
        },
        { EnergyType.ELECTRIC, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.WATER, 2f },
                { EnergyType.GHOST, 2f },

                { EnergyType.GROUND, 0f },

                { EnergyType.GRASS, .5f },
                { EnergyType.ELECTRIC, .5f },
                { EnergyType.DRAGON, .5f }
            }
        },
        { EnergyType.ICE, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.GRASS, 2f },
                { EnergyType.GROUND, 2f },
                { EnergyType.FLYING, 2f },
                { EnergyType.DRAGON, 2f },

                { EnergyType.FIRE, .5f },
                { EnergyType.WATER, .5f },
                { EnergyType.ICE, .5f },
                { EnergyType.STEEL, .5f }
            }
        },
        { EnergyType.FIGHTING, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.NORMAL, 2f },
                { EnergyType.ICE, 2f },
                { EnergyType.ROCK, 2f },
                { EnergyType.DARK, 2f },
                { EnergyType.STEEL, 2f },

                { EnergyType.GHOST, 0f },

                { EnergyType.POISON, .5f },
                { EnergyType.FLYING, .5f },
                { EnergyType.PSYCHIC, .5f },
                { EnergyType.BUG, .5f },
                { EnergyType.FAIRY, .5f }
            }
        },
        { EnergyType.POISON, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.GRASS, 2f },
                { EnergyType.FAIRY, 2f },

                { EnergyType.STEEL, 0f },

                { EnergyType.POISON, .5f },
                { EnergyType.GROUND, .5f },
                { EnergyType.ROCK, .5f },
                { EnergyType.GHOST, .5f }
            }
        },
        { EnergyType.GROUND, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.FIRE, 2f },
                { EnergyType.ELECTRIC, 2f },
                { EnergyType.POISON, 2f },
                { EnergyType.ROCK, 2f },
                { EnergyType.STEEL, 2f },

                { EnergyType.FLYING, 0f },

                { EnergyType.GRASS, .5f },
                { EnergyType.BUG, .5f }
            }
        },
        { EnergyType.FLYING, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.GRASS, 2f },
                { EnergyType.FIGHTING, 2f },
                { EnergyType.BUG, 2f },

                { EnergyType.ELECTRIC, .5f },
                { EnergyType.ROCK, .5f },
                { EnergyType.STEEL, .5f }
            }
        },
        { EnergyType.PSYCHIC, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.FIGHTING, 2f },
                { EnergyType.POISON, 2f },

                { EnergyType.DARK, 0f },

                { EnergyType.PSYCHIC, .5f },
                { EnergyType.STEEL, .5f }
            }
        },
        { EnergyType.BUG, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.GRASS, 2f },
                { EnergyType.PSYCHIC, 2f },
                { EnergyType.DARK, 2f },

                { EnergyType.FIRE, .5f },
                { EnergyType.FIGHTING, .5f },
                { EnergyType.POISON, .5f },
                { EnergyType.FLYING, .5f },
                { EnergyType.GHOST, .5f },
                { EnergyType.STEEL, .5f },
                { EnergyType.FAIRY, .5f }
            }
        },
        { EnergyType.ROCK, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.FIRE, 2f },
                { EnergyType.ICE, 2f },
                { EnergyType.FLYING, 2f },
                { EnergyType.BUG, 2f },

                { EnergyType.FIGHTING, .5f },
                { EnergyType.GROUND, .5f },
                { EnergyType.STEEL, .5f }
            }
        },
        { EnergyType.GHOST, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.GRASS, 2f },
                { EnergyType.PSYCHIC, 2f },
                { EnergyType.DARK, 2f },

                { EnergyType.FIRE, .5f },
                { EnergyType.FIGHTING, .5f },
                { EnergyType.POISON, .5f },
                { EnergyType.FLYING, .5f },
                { EnergyType.GHOST, .5f },
                { EnergyType.STEEL, .5f },
                { EnergyType.FAIRY, .5f }
            }
        },
        { EnergyType.DRAGON, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.DRAGON, 2f },

                { EnergyType.FAIRY, 0f },

                { EnergyType.STEEL, .5f }
            }
        },
        { EnergyType.DARK, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.PSYCHIC, 2f },
                { EnergyType.GHOST, 2f },

                { EnergyType.FIGHTING, .5f },
                { EnergyType.DARK, .5f },
                { EnergyType.FAIRY, .5f }
            }
        },
        { EnergyType.STEEL, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.ICE, 2f },
                { EnergyType.ROCK, 2f },
                { EnergyType.FAIRY, 2f },

                { EnergyType.FIRE, .5f },
                { EnergyType.WATER, .5f },
                { EnergyType.ELECTRIC, .5f },
                { EnergyType.STEEL, .5f }
            }
        },
        { EnergyType.FAIRY, // Attacking Type
            new Dictionary<string, float>{
                { EnergyType.FIGHTING, 2f },
                { EnergyType.DRAGON, 2f },
                { EnergyType.DARK, 2f },

                { EnergyType.FIRE, .5f },
                { EnergyType.POISON, .5f },
                { EnergyType.STEEL, .5f }
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
            return firstEff * secondEff;
        }
        else
        {
            throw new System.Exception("Invalid number of types on target");
        }
    }
}
