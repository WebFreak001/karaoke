﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Timing;
using osu.Game.Skinning;
using osuTK.Input;

namespace osu.Game.Rulesets.Karaoke.Edit.Lyrics
{
    [Cached(typeof(ILyricEditorState))]
    public partial class LyricEditor : Container, ILyricEditorState, IKeyBindingHandler<KaraokeEditAction>
    {
        [Resolved(canBeNull: true)]
        private LyricManager lyricManager { get; set; }

        [Resolved(canBeNull: true)]
        private IFrameBasedClock framedClock { get; set; }

        private KaraokeLyricEditorSkin skin;
        private DrawableLyricEditList container;

        [BackgroundDependencyLoader]
        private void load()
        {
            Child = new SkinProvidingContainer(skin = new KaraokeLyricEditorSkin())
            {
                RelativeSizeAxes = Axes.Both,
                Child = container = new DrawableLyricEditList
                {
                    RelativeSizeAxes = Axes.Both,
                }
            };

            container.Items.BindTo(BindableLyrics);
            if (lyricManager != null)
                container.OnOrderChanged += lyricManager.ChangeLyricOrder;

            MoveCursor(MovingCursorAction.First);

            BindableMode.BindValueChanged(e =>
            {
                // display add new lyric only with edit mode.
                container.DisplayBottomDrawable = e.NewValue == Mode.EditMode;
            }, true);
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (lyricManager == null)
                return false;

            if (Mode != Mode.TypingMode)
                return false;

            var position = BindableCursorPosition.Value;

            switch (e.Key)
            {
                case Key.BackSpace:
                    // delete single character.
                    var deletedSuccess = lyricManager.DeleteLyricText(position);
                    if (deletedSuccess)
                        MoveCursor(MovingCursorAction.Left);
                    return deletedSuccess;

                default:
                    return false;
            }
        }

        public bool OnPressed(KaraokeEditAction action)
        {
            if (lyricManager == null)
                return false;

            var isMoving = HandleMovingEvent(action);
            if (isMoving)
                return true;

            switch (Mode)
            {
                case Mode.ViewMode:
                    return false;

                case Mode.EditMode:
                    return false;

                case Mode.TypingMode:
                    // will handle in OnKeyDown
                    return false;

                case Mode.RecordMode:
                    return HandleSetTimeEvent(action);

                case Mode.TimeTagEditMode:
                    return HandleCreateOrDeleterTimeTagEvent(action);

                default:
                    throw new IndexOutOfRangeException(nameof(Mode));
            }
        }

        public void OnReleased(KaraokeEditAction action)
        {
        }

        protected bool HandleMovingEvent(KaraokeEditAction action)
        {
            // moving cursor action
            switch (action)
            {
                case KaraokeEditAction.Up:
                    return MoveCursor(MovingCursorAction.Up);

                case KaraokeEditAction.Down:
                    return MoveCursor(MovingCursorAction.Down);

                case KaraokeEditAction.Left:
                    return MoveCursor(MovingCursorAction.Left);

                case KaraokeEditAction.Right:
                    return MoveCursor(MovingCursorAction.Right);

                case KaraokeEditAction.First:
                    return MoveCursor(MovingCursorAction.First);

                case KaraokeEditAction.Last:
                    return MoveCursor(MovingCursorAction.Last);

                default:
                    return false;
            }
        }

        protected bool HandleSetTimeEvent(KaraokeEditAction action)
        {
            if (lyricManager == null)
                return false;

            var cursorPosition = BindableCursorPosition.Value;
            if (cursorPosition.Mode != CursorMode.Recording)
                return false;

            var currentTimeTag = cursorPosition.TimeTag;

            switch (action)
            {
                case KaraokeEditAction.ClearTime:
                    return lyricManager.ClearTimeTagTime(currentTimeTag);

                case KaraokeEditAction.SetTime:
                    if (framedClock == null)
                        return false;

                    var currentTime = framedClock.CurrentTime;
                    var setTimeSuccess = lyricManager.SetTimeTagTime(currentTimeTag, currentTime);
                    if (setTimeSuccess)
                        MoveCursor(MovingCursorAction.Right);
                    return setTimeSuccess;

                default:
                    return false;
            }
        }

        protected bool HandleCreateOrDeleterTimeTagEvent(KaraokeEditAction action)
        {
            if (lyricManager == null)
                return false;

            var position = BindableCursorPosition.Value;

            switch (action)
            {
                case KaraokeEditAction.Create:
                    return lyricManager.AddTimeTagByPosition(position);

                case KaraokeEditAction.Remove:
                    return lyricManager.RemoveTimeTagByPosition(position);

                default:
                    return false;
            }
        }

        public float FontSize
        {
            get => skin.FontSize;
            set => skin.FontSize = value;
        }

        public Mode Mode
        {
            get => BindableMode.Value;
            set => SetMode(value);
        }

        public LyricFastEditMode LyricFastEditMode
        {
            get => BindableFastEditMode.Value;
            set => SetFastEditMode(value);
        }

        public RecordingMovingCursorMode RecordingMovingCursorMode
        {
            get => BindableRecordingMovingCursorMode.Value;
            set => SetRecordingMovingCursorMode(value);
        }

        public bool AutoFocusEditLyric
        {
            get => BindableAutoFocusEditLyric.Value;
            set => SetBindableAutoFocusEditLyric(value);
        }

        public int AutoFocusEditLyricSkipRows
        {
            get => BindableAutoFocusEditLyricSkipRows.Value;
            set => SetBindableAutoFocusEditLyricSkipRows(value);
        }
    }
}
