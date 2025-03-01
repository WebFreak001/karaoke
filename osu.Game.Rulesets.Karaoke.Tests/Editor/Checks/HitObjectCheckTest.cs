﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Checks.Components;
using osu.Game.Rulesets.Objects;
using osu.Game.Tests.Beatmaps;

namespace osu.Game.Rulesets.Karaoke.Tests.Editor.Checks
{
    public abstract class HitObjectCheckTest<THitObject, TCheck> : BaseCheckTest<TCheck> where TCheck : class, ICheck, new()
    {
        protected void AssertOk(HitObject hitObject)
        {
            AssertOk(new[] { hitObject });
        }

        protected void AssertOk(IEnumerable<HitObject> hitObjects)
        {
            AssertOk(getContext(hitObjects));
        }

        protected void AssertNotOk<TIssue, TIssueTemplate>(HitObject hitObject)
            where TIssue : Issue
            where TIssueTemplate : IssueTemplate
        {
            AssertNotOk<TIssue, TIssueTemplate>(new[] { hitObject });
        }

        protected void AssertNotOk<TIssue, TIssueTemplate>(IEnumerable<HitObject> hitObjects)
            where TIssue : Issue
            where TIssueTemplate : IssueTemplate
        {
            AssertNotOk<TIssue, TIssueTemplate>(getContext(hitObjects));
        }

        private BeatmapVerifierContext getContext(IEnumerable<HitObject> hitObjects)
        {
            var beatmap = new Beatmap<HitObject> { HitObjects = hitObjects.ToList() };

            return new BeatmapVerifierContext(beatmap, new TestWorkingBeatmap(beatmap));
        }
    }
}
