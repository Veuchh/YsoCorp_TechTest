using System;

[Flags]
public enum CardAttributes
{
    Movement = 1 << 0,
    Attack = 1 << 1,
    Wait = 1 << 2,
}
