namespace FFXIVClientStructs.FFXIV.Client.Graphics.Render;

// Client::Graphics::Render::OffscreenRenderingManager
// renderer responsible for UI character models - tryon, inspect, etc
[StructLayout(LayoutKind.Explicit, Size = 0x190)]
[GenerateInterop]
public unsafe partial struct OffscreenRenderingManager {
    [FieldOffset(0xC8)] [FixedSizeArray] internal FixedSizeArray4<Pointer<Camera>> _cameras;
}
