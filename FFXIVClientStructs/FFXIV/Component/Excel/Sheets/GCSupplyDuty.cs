// <auto-generated/>
namespace FFXIVClientStructs.FFXIV.Component.Excel.Sheets;

[GenerateInterop]
[StructLayout(LayoutKind.Explicit, Size = 0xB0)]
public unsafe partial struct GCSupplyDuty {
    [FieldOffset(0x00), FixedSizeArray] internal FixedSizeArray11<SupplyDataStruct> _supplyData;

    [GenerateInterop]
    [StructLayout(LayoutKind.Explicit, Size = 0x10)]
    public partial struct SupplyDataStruct {
        [FieldOffset(0x00), FixedSizeArray] internal FixedSizeArray3<int> _item;
        [FieldOffset(0x0C), FixedSizeArray] internal FixedSizeArray3<byte> _itemCount;
    }
}