using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[Serializable]
public class CharacterStats
{
    public float BaseValue;

    public float Value { 
        get {
            if (isDirty || BaseValue != lastBaseValue)
            {
                lastBaseValue = BaseValue;
                _value = CalculateFinalValue();
                isDirty = false;
            }
            return _value;
        } 
    }

    protected bool isDirty = true;
    protected float _value;
    protected float lastBaseValue = float.MinValue;

    protected readonly List<StatsModifier> statsModifiers;
    public readonly ReadOnlyCollection<StatsModifier> StatsModifiers;

    public CharacterStats()
    {
        statsModifiers = new List<StatsModifier>();
        StatsModifiers = statsModifiers.AsReadOnly();
    }
    public CharacterStats(float baseValue) : this() //constructeur
    {
        BaseValue = baseValue;
    }

    public virtual void addModifier(StatsModifier modifier) //ajouter un modifier à la liste
    {
        isDirty = true;
        statsModifiers.Add(modifier);
        statsModifiers.Sort(CompareModifierOrder);
    }

    protected virtual int CompareModifierOrder(StatsModifier modifier1, StatsModifier modifier2) //appliquer les modifiers Flat avant ceux en pourcentage
    {
        if (modifier1.Order < modifier2.Order)
            return -1;
        else if (modifier1.Order > modifier2.Order)
            return 1;
        return 0; // if (modifier1.Order == modifier2.Order) en gros
    }

    public virtual bool removeModifier(StatsModifier modifier) //retirer un modifier de la liste
    {
        if(statsModifiers.Remove(modifier))
        {
            isDirty = true;
            return true;
        }
        return false;
    }

    public bool RemoveAllModifiersFromSource(object source)
    {
        bool didRemove = false;

        for (int i = statsModifiers.Count - 1; i >= 0; i--)
        {
            if (statsModifiers[i].Source == source)
            {
                isDirty = true;
                didRemove = true;
                statsModifiers.RemoveAt(i);
            }
        }

        return didRemove;
    }

    protected virtual float CalculateFinalValue()
    {
        float finalValue = BaseValue;
        float sumPercentAdd = 0;

        for (int i = 0; i < statsModifiers.Count; i++)
        {
            StatsModifier modifier = statsModifiers[i];

            if(modifier.Type == StatModType.Flat)
                finalValue += modifier.Value;
            else if(modifier.Type == StatModType.PercentAdd)
            {
                sumPercentAdd += modifier.Value;

                if (i + 1 >= statsModifiers.Count || statsModifiers[i + 1].Type != StatModType.PercentAdd)
                {
                    finalValue *= 1 + sumPercentAdd;
                    sumPercentAdd = 0;
                }
            }
            else if(modifier.Type == StatModType.PercentMult)
                finalValue *= 1 + modifier.Value;
        }

        return (float)Math.Round(finalValue, 4);
    }
}
