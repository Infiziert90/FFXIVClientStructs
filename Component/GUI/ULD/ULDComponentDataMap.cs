﻿using System.Runtime.InteropServices;

namespace FFXIVClientStructs.Component.GUI.ULD
{

    [StructLayout(LayoutKind.Explicit, Size = 0x34)]
    public unsafe struct ULDComponentDataMap
    {
        [FieldOffset(0x00)] public ULDComponentDataBase Base;
        [FieldOffset(0x0C)] public fixed uint Nodes[10];
    }
}
