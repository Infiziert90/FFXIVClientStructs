using FFXIVClientStructs.FFXIV.Component.GUI;

namespace FFXIVClientStructs.FFXIV.Client.UI;

// Client::UI::AddonJournalDetail
//   Component::GUI::AtkUnitBase
//     Component::GUI::AtkEventListener
[Addon("JournalDetail")]
[GenerateInterop]
[Inherits<AtkUnitBase>]
[StructLayout(LayoutKind.Explicit, Size = 0x370)]
public unsafe partial struct AddonJournalDetail {
    [FieldOffset(0x240)] public AtkComponentScrollBar* ScrollBarNode;
    [FieldOffset(0x248)] public AtkComponentGuildLeveCard* GuildLeveCardNode;
    [FieldOffset(0x250)] public AtkTextNode* DutyNameTextNode;
    [FieldOffset(0x258)] public AtkTextNode* DutyLevelTextNode;
    [FieldOffset(0x260)] public AtkImageNode* DutyCategoryImageNode;
    [FieldOffset(0x268)] public AtkImageNode* DutyCategoryBackgroundImageNode;
    [FieldOffset(0x270)] public AtkImageNode* GuildLeveCardBackgroundImageNode;
    [FieldOffset(0x278)] public AtkResNode* RewardsReceivedResNode;
    [FieldOffset(0x280)] public AtkTextNode* RewardsReceivedTextNode;
    [FieldOffset(0x288)] public AtkResNode* ButtonsResNode;
    [FieldOffset(0x290)] public AtkComponentButton* AcceptMapButton;
    [FieldOffset(0x298)] public AtkComponentButton* InitiateButton;
    [FieldOffset(0x2A0)] public AtkComponentButton* AbandonDeclineButton;
    [FieldOffset(0x2A8)] public AtkResNode* AtkResNode298; // Seems unused? // Res Node: 44
    [FieldOffset(0x2B0)] public AtkImageNode* QuestImageNode;
    [FieldOffset(0x2B8)] public AtkTextNode* AtkTextNode2A8; // Seems unused? // TestNode: 46
    [FieldOffset(0x2C0)] public AtkTextNode* RequirementsNotMetLabelTextNode;
    [FieldOffset(0x2C8)] public AtkTextNode* RequirementsNotMetTextNode; // The actual requirements that are not being met
    [FieldOffset(0x2D0)] public AtkTextNode* AtkTextNode2C0; // Seems unused? // TextNode: 35
    [FieldOffset(0x2D8)] public AtkComponentButton* AtkComponentButton2C8; // Seems Unused? // Button Component Node: 53
    [FieldOffset(0x2E0)] public AtkComponentJournalCanvas* JournalCanvasNode;
}
