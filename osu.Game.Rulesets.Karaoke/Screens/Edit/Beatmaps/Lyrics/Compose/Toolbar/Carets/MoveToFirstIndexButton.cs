﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.States;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.Compose.Toolbar.Carets
{
    public partial class MoveToFirstIndexButton : MoveToCaretPositionButton
    {
        protected override KaraokeEditAction EditAction => KaraokeEditAction.MoveToFirstIndex;

        protected override MovingCaretAction AcceptAction => MovingCaretAction.FirstIndex;

        public MoveToFirstIndexButton()
        {
            SetIcon(FontAwesome.Solid.AngleDoubleLeft);
        }
    }
}
