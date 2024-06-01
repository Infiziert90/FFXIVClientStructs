using FFXIVClientStructs.FFXIV.Component.GUI;

namespace FFXIVClientStructs.FFXIV.Client.UI;

[Addon("GSInfoCardDeck")]
[GenerateInterop]
[Inherits<AtkUnitBase>]
[StructLayout(LayoutKind.Explicit, Size = 0x228)]
public unsafe partial struct AddonGSInfoCardDeck {
    [FieldOffset(0x220)] public AtkComponentList* DeckList;
}
