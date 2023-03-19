﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Karaoke.Configuration;
using osu.Game.Rulesets.Karaoke.Edit.ChangeHandlers.Lyrics;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.CaretPosition;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.States;
using osu.Game.Screens.Edit;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.Settings.TimeTags
{
    public partial class RecordTimeTagActionReceiver : Component, IKeyBindingHandler<KaraokeEditAction>
    {
        [Resolved]
        private KaraokeRulesetLyricEditorConfigManager lyricEditorConfigManager { get; set; }

        [Resolved]
        private ILyricTimeTagsChangeHandler lyricTimeTagsChangeHandler { get; set; }

        [Resolved]
        private ILyricCaretState lyricCaretState { get; set; }

        [Resolved]
        private EditorClock editorClock { get; set; }

        public bool OnPressed(KeyBindingPressEvent<KaraokeEditAction> e)
        {
            var caretPosition = lyricCaretState.CaretPosition;
            if (caretPosition is not TimeTagCaretPosition timeTagCaretPosition)
                throw new NotSupportedException(caretPosition?.GetType().Name ?? "(null)");

            var currentTimeTag = timeTagCaretPosition.TimeTag;

            switch (e.Action)
            {
                case KaraokeEditAction.ClearTime:
                    lyricTimeTagsChangeHandler.ClearTimeTagTime(currentTimeTag);
                    return true;

                case KaraokeEditAction.SetTime:
                    double currentTime = editorClock.CurrentTime;
                    lyricTimeTagsChangeHandler.SetTimeTagTime(currentTimeTag, currentTime);

                    if (lyricEditorConfigManager.Get<bool>(KaraokeRulesetLyricEditorSetting.RecordingAutoMoveToNextTimeTag))
                        lyricCaretState.MoveCaret(MovingCaretAction.NextIndex);

                    return true;

                default:
                    return false;
            }
        }

        public void OnReleased(KeyBindingReleaseEvent<KaraokeEditAction> e)
        {
        }
    }
}
