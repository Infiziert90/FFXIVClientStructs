// <auto-generated/>
namespace FFXIVClientStructs.FFXIV.Component.Excel.Sheets;

[StructLayout(LayoutKind.Explicit, Size = 0x08)]
public unsafe partial struct BehaviorMove {
    [FieldOffset(0x00)] public float Unknown0;
    [FieldOffset(0x04)] public byte Unknown1;
    [FieldOffset(0x05)] public BitField05Flags BitField05;
    public bool Unknown2 => BitField05.HasFlag(BitField05Flags.Unknown2);
    public bool Unknown3 => BitField05.HasFlag(BitField05Flags.Unknown3);

    [Flags]
    public enum BitField05Flags : byte {
    	Unknown2 = 1 << 0,
    	Unknown3 = 1 << 1,
    }
}