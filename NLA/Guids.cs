// Guids.cs
// MUST match guids.h
using System;

namespace None.NLA
{
    static class GuidList
    {
        public const string guidNLAPkgString = "30bb9f85-6d65-49e9-8978-fee60d61040b";
        public const string guidNLACmdSetString = "92872fff-a67e-4952-96df-6b34e009a2bf";

        public static readonly Guid guidNLACmdSet = new Guid(guidNLACmdSetString);
    };
}