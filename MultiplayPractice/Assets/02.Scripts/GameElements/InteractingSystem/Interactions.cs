using System;

namespace MP.GameElements.InteractingSystem
{
    [Flags]
    public enum Interactions
    {
        None,
        Grab,
        Ungrab,
        Throw,
        Use
    }
}
