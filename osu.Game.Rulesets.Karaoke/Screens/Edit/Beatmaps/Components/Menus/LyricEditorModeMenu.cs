﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System.Collections.Generic;
using osu.Framework.Bindables;
using osu.Game.Rulesets.Karaoke.Edit.Components.Menus;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Components.Menus
{
    public class LyricEditorModeMenu : EnumMenu<LyricEditorMode>
    {
        public LyricEditorModeMenu(Bindable<LyricEditorMode> config, string text)
            : base(config, text)
        {
        }

        protected override IEnumerable<LyricEditorMode> ValidEnums => new[]
        {
            LyricEditorMode.View,
            LyricEditorMode.Texting,
            LyricEditorMode.Reference,
            LyricEditorMode.Language,
            LyricEditorMode.EditRuby,
            LyricEditorMode.EditRomaji,
            LyricEditorMode.EditTimeTag,
            LyricEditorMode.EditNote,
            LyricEditorMode.Singer,
        };
    }
}
