// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Diagnostics.CodeAnalysis;
using osu.Framework.Allocation;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Overlays;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Components.Markdown;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Stages.Classic.Stage.Settings;

public partial class StageEditorEditModeSection : EditModeSection<StageEditorEditMode>
{
    [OpenTabletDriver.Plugin.DependencyInjection.Resolved, AllowNull]
    private IStageEditorStateProvider stageEditorStateProvider { get; set; }

    private readonly StageEditorEditCategory category;

    public StageEditorEditModeSection(StageEditorEditCategory category)
    {
        this.category = category;
    }

    protected override StageEditorEditMode DefaultMode()
        => stageEditorStateProvider.EditMode;

    internal sealed override void UpdateEditMode(StageEditorEditMode mode)
    {
        stageEditorStateProvider.ChangeEditMode(mode);

        base.UpdateEditMode(mode);
    }

    protected override OverlayColourScheme CreateColourScheme()
        => OverlayColourScheme.Green;

    protected override Selection CreateSelection(StageEditorEditMode mode) =>
        mode switch
        {
            StageEditorEditMode.Edit => new Selection(),
            StageEditorEditMode.Verify => new StageEditorVerifySelection(category),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

    protected override LocalisableString GetSelectionText(StageEditorEditMode mode) =>
        mode switch
        {
            StageEditorEditMode.Edit => "Edit",
            StageEditorEditMode.Verify => "Verify",
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

    protected override Color4 GetSelectionColour(OsuColour colours, StageEditorEditMode mode, bool active) =>
        mode switch
        {
            StageEditorEditMode.Edit => active ? colours.Red : colours.RedDarker,
            StageEditorEditMode.Verify => active ? colours.Yellow : colours.YellowDarker,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

    protected override DescriptionFormat GetSelectionDescription(StageEditorEditMode mode) =>
        mode switch
        {
            StageEditorEditMode.Edit => "Edit the stage property in here.",
            StageEditorEditMode.Verify => "Check if have any stage issues.",
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

    private partial class StageEditorVerifySelection : VerifySelection
    {
        private readonly StageEditorEditCategory category;

        public StageEditorVerifySelection(StageEditorEditCategory category)
        {
            this.category = category;
        }

        [BackgroundDependencyLoader]
        private void load(IStageEditorVerifier stageEditorVerifier)
        {
            Issues.BindTo(stageEditorVerifier.GetIssueByType(category));
        }
    }
}
