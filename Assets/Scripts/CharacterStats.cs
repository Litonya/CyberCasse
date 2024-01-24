using System;
using System.Collections.Generic;

public class CharacterStats
{
    public float BaseValue;

    public float Value { 
        get {
            if (isDirty)
            {
                _value = CalculateFinalValue();
                isDirty = false;
            }
            return _value;
        } 
    }

    private bool isDirty = true;
    private float _value;

    private readonly List<StatsModifier> statsModifiers;

    public CharacterStats(float baseValue) //constructeur
    {
        BaseValue = baseValue;
        statsModifiers = new List<StatsModifier>();
    }

    public void addModifier(StatsModifier modifier) //ajouter un modifier à la liste
    {
        isDirty = true;
        statsModifiers.Add(modifier);
        statsModifiers.Sort(CompareModifierOrder);
    }

    private int CompareModifierOrder(StatsModifier modifier1, StatsModifier modifier2) //appliquer les modifiers Flat avant ceux en pourcentage
    {
        if (modifier1.Order < modifier2.Order)
            return -1;
        else if (modifier1.Order > modifier2.Order)
            return 1;
        return 0; // if (modifier1.Order == modifier2.Order) en gros

    }

    public bool removeModifier(StatsModifier modifier) //retirer un modifier de la liste
    {
        isDirty = true;
        return statsModifiers.Remove(modifier);
    }

    public float CalculateFinalValue()
    {
        float finalValue = BaseValue;

        for (int i = 0; i < statsModifiers.Count; i++)
        {
            StatsModifier modifier = statsModifiers[i];

            if(modifier.Type == StatModType.Flat)
                finalValue += modifier.Value;
            else if (modifier.Type == StatModType.Percent)
                finalValue *= 1 + modifier.Value;
        }

        return (float)Math.Round(finalValue, 4);
    }
}
