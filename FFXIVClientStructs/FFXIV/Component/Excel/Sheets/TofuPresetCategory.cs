// <auto-generated/>
namespace FFXIVClientStructs.FFXIV.Component.Excel.Sheets;

[StructLayout(LayoutKind.Explicit, Size = 0x08)]
public unsafe partial struct TofuPresetCategory {
    [FieldOffset(0x00)] public int Unknown0_Offset;
    [FieldOffset(0x04)] public BitField04Flags BitField04;
    public bool Unknown1 => BitField04.HasFlag(BitField04Flags.Unknown1);
    public bool Unknown2 => BitField04.HasFlag(BitField04Flags.Unknown2);

    [Flags]
    public enum BitField04Flags : byte {
    	Unknown1 = 1 << 0,
    	Unknown2 = 1 << 1,
    }
}