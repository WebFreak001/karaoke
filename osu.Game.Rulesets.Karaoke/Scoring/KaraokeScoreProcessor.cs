﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Karaoke.Scoring
{
    internal partial class KaraokeScoreProcessor : ScoreProcessor
    {
        public KaraokeScoreProcessor()
            : base(new KaraokeRuleset())
        {
        }
    }
}
