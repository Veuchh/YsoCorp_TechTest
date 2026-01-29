using System;

[Flags]
public enum CardAttributes
{
    Movement = 1 << 0,
    Attack = 1 << 1,
    Wait = 1 << 2,
    Tutorial_Move = 1 << 3,
    Tutorial_Attack = 1 << 4
}
