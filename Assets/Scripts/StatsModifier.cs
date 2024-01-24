public enum StatModType
{
    Flat,
    Percent,
}

public class StatsModifier
{
    public readonly float Value;
    public readonly StatModType Type;
    public readonly int Order;

    public StatsModifier(float value, StatModType type, int order)
    {
        Value = value;
        Type = type;
        Order = order;
    }

    public StatsModifier(float value, StatModType type) : this (value, type, (int)type)
    {
        
    }
}
