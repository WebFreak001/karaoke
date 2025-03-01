﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.Settings.Notes;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.States.Modes;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.Settings
{
    public partial class NoteSettings : LyricEditorSettings
    {
        public override SettingsDirection Direction => SettingsDirection.Right;

        public override float SettingsWidth => 300;

        private readonly IBindable<NoteEditMode> bindableMode = new Bindable<NoteEditMode>();

        [BackgroundDependencyLoader]
        private void load(IEditNoteModeState editNoteModeState)
        {
            bindableMode.BindTo(editNoteModeState.BindableEditMode);
            bindableMode.BindValueChanged(e =>
            {
                ReloadSections();
            }, true);
        }

        protected override IReadOnlyList<Drawable> CreateSections() => bindableMode.Value switch
        {
            NoteEditMode.Generate => new Drawable[]
            {
                new NoteEditModeSection(),
                new NoteConfigSection(),
                new NoteSwitchSpecialActionSection(),
            },
            NoteEditMode.Edit => new Drawable[]
            {
                new NoteEditModeSection(),
                new NoteEditPropertyModeSection(),
                new NoteEditPropertySection(),
            },
            NoteEditMode.Verify => new Drawable[]
            {
                new NoteEditModeSection(),
                new NoteIssueSection()
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
