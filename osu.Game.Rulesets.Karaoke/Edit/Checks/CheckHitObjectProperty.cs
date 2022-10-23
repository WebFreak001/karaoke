﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Checks.Components;
using osu.Game.Rulesets.Karaoke.Objects;

namespace osu.Game.Rulesets.Karaoke.Edit.Checks
{
    public abstract class CheckHitObjectProperty<THitObject> : ICheck where THitObject : KaraokeHitObject
    {
        public CheckMetadata Metadata => new(CheckCategory.HitObjects, Description);

        protected abstract string Description { get; }

        public abstract IEnumerable<IssueTemplate> PossibleTemplates { get; }

        public IEnumerable<Issue> Run(BeatmapVerifierContext context)
            => context.Beatmap.HitObjects.OfType<THitObject>().Select(Check).SelectMany(x => x);

        public abstract IEnumerable<Issue> Check(THitObject hitObject);
    }
}
