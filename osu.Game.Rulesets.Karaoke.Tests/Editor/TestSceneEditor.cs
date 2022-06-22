﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Karaoke.Tests.Beatmaps;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Karaoke.Tests.Editor
{
    [TestFixture]
    public class TestSceneEditor : EditorTestScene
    {
        protected override IBeatmap CreateBeatmap(RulesetInfo ruleset) => new TestKaraokeBeatmap(ruleset);

        protected override Ruleset CreateEditorRuleset() => new KaraokeRuleset();
    }
}
