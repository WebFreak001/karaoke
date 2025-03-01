// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Rulesets.Karaoke.Edit.Utils;
using osu.Game.Rulesets.Karaoke.Objects;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.Settings.Notes;
using osu.Game.Rulesets.Karaoke.Utils;
using osu.Game.Rulesets.Objects;
using osu.Game.Screens.Edit;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.States.Modes
{
    public partial class EditNoteModeState : ModeStateWithBlueprintContainer<Note>, IEditNoteModeState
    {
        private readonly Bindable<NoteEditMode> bindableEditMode = new();
        private readonly BindableList<HitObject> selectedHitObjects = new();

        [Resolved, AllowNull]
        private EditorBeatmap editorBeatmap { get; set; }

        public IBindable<NoteEditMode> BindableEditMode => bindableEditMode;

        public void ChangeEditMode(NoteEditMode mode)
            => bindableEditMode.Value = mode;

        public Bindable<NoteEditModeSpecialAction> BindableSpecialAction { get; } = new();

        public Bindable<NoteEditPropertyMode> NoteEditPropertyMode { get; } = new();

        [BackgroundDependencyLoader]
        private void load()
        {
            BindablesUtils.Sync(SelectedItems, selectedHitObjects);
            selectedHitObjects.BindTo(editorBeatmap.SelectedHitObjects);
        }

        protected override bool IsWriteLyricPropertyLocked(Lyric lyric)
            => HitObjectWritableUtils.IsCreateOrRemoveNoteLocked(lyric);

        protected override bool SelectFirstProperty(Lyric lyric)
            => BindableEditMode.Value == NoteEditMode.Edit;

        protected override IEnumerable<Note> SelectableProperties(Lyric lyric)
            => EditorBeatmapUtils.GetNotesByLyric(editorBeatmap, lyric);
    }
}
