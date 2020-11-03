﻿using System.Runtime.InteropServices;

namespace FFXIVClientStructs.Component.GUI.ULD
{

    [StructLayout(LayoutKind.Explicit, Size = 0x20)]
    public unsafe struct ULDComponentDataUnknownButton
    {
        [FieldOffset(0x00)] public ULDComponentDataBase Base;
        [FieldOffset(0x0C)] public fixed uint Nodes[4];
        [FieldOffset(0x1C)] public uint TextId;
    }
}
