using System.Runtime.CompilerServices;

namespace FFXIVClientStructs.FFXIV.Client.UI.Misc;

// ToDo: Wrap in RaptureHotbarModule partial struct for namespacing (API 10)
[GenerateInterop]
[StructLayout(LayoutKind.Explicit, Size = Size)]
public unsafe partial struct HotBar {
    public const int Size = HotBarSlot.Size * 16;

    [FieldOffset(0x00), FixedSizeArray] internal FixedSizeArray16<HotBarSlot> _slots;

    /// <summary>
    /// Helper method to return a pointer to a specific HotBarSlot, as certain APIs are much happier with a
    /// pointer rather than a fixed reference.
    /// </summary>
    /// <param name="id">A hotbar slot ID between 0 and 15.</param>
    /// <returns>Returns a pointer to a HotBarSlot, or null if an invalid ID was passed.</returns>
    public HotBarSlot* GetHotbarSlot(uint id) {
        if (id > 15) return null;

        return (HotBarSlot*)Unsafe.AsPointer(ref Slots[(int)id]);
    }
}
