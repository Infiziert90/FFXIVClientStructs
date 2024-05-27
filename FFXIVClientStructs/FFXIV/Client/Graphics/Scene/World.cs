namespace FFXIVClientStructs.FFXIV.Client.Graphics.Scene;

[StructLayout(LayoutKind.Explicit, Size = 0x160)]
[GenerateInterop]
[Inherits<Object>]
public unsafe partial struct World {
    [StaticAddress("48 8B 05 ?? ?? ?? ?? 48 8B 50 40", 3, isPointer: true)]
    public static partial World* Instance();
}
