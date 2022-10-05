﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Karaoke.Edit.Lyrics.Settings.Texting;

namespace osu.Game.Rulesets.Karaoke.Edit.Lyrics.Settings
{
    public class TextingSettings : LyricEditorSettings
    {
        public override SettingsDirection Direction => SettingsDirection.Right;

        public override float SettingsWidth => 300;

        protected override IReadOnlyList<Drawable> CreateSections() => new Drawable[]
        {
            new TextingEditModeSection(),
            new TextingSwitchSpecialActionSection()
        };
    }
}