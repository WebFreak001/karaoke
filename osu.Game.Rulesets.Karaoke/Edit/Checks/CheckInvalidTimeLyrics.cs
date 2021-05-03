﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Edit.Checks.Components;
using osu.Game.Rulesets.Karaoke.Edit.Checks.Components;
using osu.Game.Rulesets.Karaoke.Edit.Checks.Configs;
using osu.Game.Rulesets.Karaoke.Objects;
using osu.Game.Rulesets.Karaoke.Utils;

namespace osu.Game.Rulesets.Karaoke.Edit.Checks
{
    public class CheckInvalidTimeLyrics : ICheck
    {
        private readonly LyricCheckerConfig config;

        public CheckMetadata Metadata => new CheckMetadata(CheckCategory.HitObjects, "Lyrics with invalid time issue.");

        public IEnumerable<IssueTemplate> PossibleTemplates => new IssueTemplate[]
        {
            new IssueTemplateInvalidLyricTime(this),
            new IssueTemplateInvalidTimeTag(this),
        };

        public CheckInvalidTimeLyrics(LyricCheckerConfig config)
        {
            this.config = config;
        }

        public IEnumerable<Issue> Run(IBeatmap playableBeatmap, IWorkingBeatmap workingBeatmap)
        {
            foreach (var lyric in playableBeatmap.HitObjects.OfType<Lyric>())
            {
                var invalidLyricTime = checkInvalidLyricTime(lyric);
                if (invalidLyricTime.Any())
                    yield return new IssueTemplateInvalidLyricTime(this).Create(lyric, invalidLyricTime);

                var invalidTimeTags = checkInvalidTimeTags(lyric);
                if (invalidTimeTags.Any())
                    yield return new IssueTemplateInvalidTimeTag(this).Create(lyric, invalidTimeTags);
            }
        }

        private TimeInvalid[] checkInvalidLyricTime(Lyric lyric)
        {
            var result = new List<TimeInvalid>();

            if (LyricUtils.CheckIsTimeOverlapping(lyric))
                result.Add(TimeInvalid.Overlapping);

            if (LyricUtils.CheckIsStartTimeInvalid(lyric))
                result.Add(TimeInvalid.StartTimeInvalid);

            if (LyricUtils.CheckIsEndTimeInvalid(lyric))
                result.Add(TimeInvalid.EndTimeInvalid);

            return result.ToArray();
        }

        private Dictionary<TimeTagInvalid, TimeTag[]> checkInvalidTimeTags(Lyric lyric)
        {
            var result = new Dictionary<TimeTagInvalid, TimeTag[]>();

            // todo : check out of range.
            var outOfRangeTags = TimeTagsUtils.FindOutOfRange(lyric.TimeTags, lyric.Text);
            if (outOfRangeTags?.Length > 0)
                result.Add(TimeTagInvalid.OutOfRange, outOfRangeTags);

            // Check overlapping.
            var groupCheck = config.TimeTagTimeGroupCheck;
            var selfCheck = config.TimeTagTimeSelfCheck;
            var invalidTimeTags = TimeTagsUtils.FindInvalid(lyric.TimeTags, groupCheck, selfCheck);
            if (invalidTimeTags?.Length > 0)
                result.Add(TimeTagInvalid.Overlapping, invalidTimeTags);

            return result;
        }

        public class IssueTemplateInvalidLyricTime : IssueTemplate
        {
            public IssueTemplateInvalidLyricTime(ICheck check)
                : base(check, IssueType.Problem, "This lyric contains invalid time.")
            {
            }

            public Issue Create(Lyric lyric, TimeInvalid[] invalidTime)
                => new LyricTimeIssue(lyric, this, invalidTime);
        }

        public class IssueTemplateInvalidTimeTag : IssueTemplate
        {
            public IssueTemplateInvalidTimeTag(ICheck check)
                : base(check, IssueType.Problem, "This lyric contains invalid time-tag.")
            {
            }

            public Issue Create(Lyric lyric, Dictionary<TimeTagInvalid, TimeTag[]> invalidTimeTags)
                => new TimeTagIssue(lyric, this, invalidTimeTags);
        }
    }
}