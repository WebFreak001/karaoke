﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osu.Game.Rulesets.Karaoke.Edit.ImportLyric;

namespace osu.Game.Rulesets.Karaoke.Edit.Components.Menus
{
    public class ImportLyricMenu : MenuItem
    {
        public ImportLyricMenu(IScreen screen, string text)
            : base(text, () => openLyricImporter(screen))
        {
        }

        private static void openLyricImporter(IScreen screen)
        {
            screen?.Push(new LyricImporter());
        }
    }
}