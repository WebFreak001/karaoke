﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Edit.Checks.Components;
using osu.Game.Rulesets.Karaoke.Edit.Checks;

namespace osu.Game.Rulesets.Karaoke.Edit
{
    public class KaraokeBeatmapVerifier : IBeatmapVerifier
    {
        private readonly List<ICheck> checks = new()
        {
            new CheckBeatmapAvailableTranslates(),
            new CheckBeatmapClassicStageInfo(),
            new CheckBeatmapNoteInfo(),
            new CheckBeatmapPageInfo(),
            new CheckLyricLanguage(),
            new CheckLyricReferenceLyric(),
            new CheckLyricRomajiTag(),
            new CheckLyricRubyTag(),
            new CheckLyricSinger(),
            new CheckLyricText(),
            new CheckLyricTime(),
            new CheckLyricTimeTag(),
            new CheckLyricTranslate(),
            new CheckNoteReferenceLyric(),
            new CheckNoteText(),
        };

        public IEnumerable<Issue> Run(BeatmapVerifierContext context) => checks.SelectMany(check => check.Run(context));
    }
}
