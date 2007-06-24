using System;

namespace CS2.Model
{
    [Flags]
    public enum SearchScope
    {
        Namespace   =     1,
        Class       =     2,
        Property    =     4,
        Method      =     8,
        Field       =    16,
        Fulltext    =    32 
    }
}
