﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Karaoke.Extensions;
using osu.Game.Rulesets.Karaoke.Objects;

namespace osu.Game.Rulesets.Karaoke.Edit.Lyrics.CaretPosition.Algorithms
{
    public class TimeTagCaretPositionAlgorithm : CaretPositionAlgorithm<TimeTagCaretPosition>
    {
        public MovingTimeTagCaretMode Mode { get; set; }

        public TimeTagCaretPositionAlgorithm(Lyric[] lyrics)
            : base(lyrics)
        {
        }

        public override bool PositionMovable(TimeTagCaretPosition position)
        {
            var lyric = position.Lyric;
            var timeTag = position.TimeTag;

            return lyric.TimeTags.Contains(timeTag)
                   && timeTagMovable(timeTag);
        }

        public override TimeTagCaretPosition? MoveUp(TimeTagCaretPosition currentPosition)
        {
            var currentTimeTag = currentPosition.TimeTag;

            // need to check is lyric in time-tag is valid.
            var currentLyric = timeTagInLyric(currentTimeTag);
            if (currentLyric != currentPosition.Lyric)
                throw new ArgumentException(nameof(currentPosition.Lyric));

            // get previous movable lyric.
            var previousLyric = Lyrics.GetPreviousMatch(currentLyric, l => l.TimeTags?.Any(timeTagMovable) ?? false);
            if (previousLyric == null)
                return null;

            var timeTags = previousLyric.TimeTags.Where(timeTagMovable).ToArray();
            var upTimeTag = timeTags.FirstOrDefault(x => x.Index >= currentTimeTag.Index)
                            ?? timeTags.LastOrDefault();
            return timeTagToPosition(upTimeTag);
        }

        public override TimeTagCaretPosition? MoveDown(TimeTagCaretPosition currentPosition)
        {
            var currentTimeTag = currentPosition.TimeTag;

            // need to check is lyric in time-tag is valid.
            var currentLyric = timeTagInLyric(currentTimeTag);
            if (currentLyric != currentPosition.Lyric)
                throw new ArgumentException(nameof(currentPosition.Lyric));

            // get next movable lyric.
            var nextLyric = Lyrics.GetNextMatch(currentLyric, l => l.TimeTags?.Any(timeTagMovable) ?? false);
            if (nextLyric == null)
                return null;

            var timeTags = nextLyric.TimeTags.Where(timeTagMovable).ToArray();
            var downTimeTag = timeTags.FirstOrDefault(x => x.Index >= currentTimeTag.Index)
                              ?? timeTags.LastOrDefault();
            return timeTagToPosition(downTimeTag);
        }

        public override TimeTagCaretPosition? MoveLeft(TimeTagCaretPosition currentPosition)
        {
            var timeTags = Lyrics.SelectMany(x => x.TimeTags ?? Array.Empty<TimeTag>()).ToArray();
            var previousTimeTag = timeTags.GetPreviousMatch(currentPosition.TimeTag, timeTagMovable);
            return timeTagToPosition(previousTimeTag);
        }

        public override TimeTagCaretPosition? MoveRight(TimeTagCaretPosition currentPosition)
        {
            var timeTags = Lyrics.SelectMany(x => x.TimeTags ?? Array.Empty<TimeTag>()).ToArray();
            var nextTimeTag = timeTags.GetNextMatch(currentPosition.TimeTag, timeTagMovable);
            return timeTagToPosition(nextTimeTag);
        }

        public override TimeTagCaretPosition? MoveToFirst()
        {
            var timeTags = Lyrics.SelectMany(x => x.TimeTags ?? Array.Empty<TimeTag>()).ToArray();
            var firstTimeTag = timeTags.FirstOrDefault(timeTagMovable);
            return timeTagToPosition(firstTimeTag);
        }

        public override TimeTagCaretPosition? MoveToLast()
        {
            var timeTags = Lyrics.SelectMany(x => x.TimeTags ?? Array.Empty<TimeTag>()).ToArray();
            var lastTag = timeTags.LastOrDefault(timeTagMovable);
            return timeTagToPosition(lastTag);
        }

        public override TimeTagCaretPosition? MoveToTarget(Lyric lyric)
        {
            var targetTimeTag = lyric.TimeTags?.FirstOrDefault(timeTagMovable);

            // should not move to lyric if contains no time-tag.
            return targetTimeTag == null ? null : new TimeTagCaretPosition(lyric, targetTimeTag);
        }

        private TimeTagCaretPosition? timeTagToPosition(TimeTag timeTag)
        {
            if (timeTag == null)
                return null;

            var lyric = timeTagInLyric(timeTag);
            if (lyric == null)
                return null;

            return new TimeTagCaretPosition(lyric, timeTag);
        }

        private Lyric? timeTagInLyric(TimeTag timeTag)
        {
            if (timeTag == null)
                return null;

            return Lyrics.FirstOrDefault(x => x.TimeTags?.Contains(timeTag) ?? false);
        }

        private bool timeTagMovable(TimeTag timeTag)
        {
            if (timeTag == null)
                return false;

            return Mode switch
            {
                MovingTimeTagCaretMode.None => true,
                MovingTimeTagCaretMode.OnlyStartTag => timeTag.Index.State == TextIndex.IndexState.Start,
                MovingTimeTagCaretMode.OnlyEndTag => timeTag.Index.State == TextIndex.IndexState.End,
                _ => throw new InvalidOperationException(nameof(MovingTimeTagCaretMode))
            };
        }
    }
}
