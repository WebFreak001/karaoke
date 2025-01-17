﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Karaoke.Edit.Generator.Lyrics.TimeTags;
using osu.Game.Rulesets.Karaoke.Objects;
using osu.Game.Rulesets.Karaoke.Tests.Asserts;
using osu.Game.Rulesets.Karaoke.Tests.Helper;

namespace osu.Game.Rulesets.Karaoke.Tests.Editor.Generator.Lyrics.TimeTags
{
    public abstract class BaseTimeTagGeneratorTest<TTimeTagGenerator, TConfig> : BaseLyricGeneratorTest<TTimeTagGenerator, TimeTag[], TConfig>
        where TTimeTagGenerator : TimeTagGenerator<TConfig> where TConfig : TimeTagGeneratorConfig, new()
    {
        protected static void CheckCanGenerate(string text, bool canGenerate, TConfig config)
        {
            var lyric = new Lyric { Text = text };
            CheckCanGenerate(lyric, canGenerate, config);
        }

        protected void CheckGenerateResult(string text, string[] expectedTimeTags, TConfig config)
        {
            var expected = TestCaseTagHelper.ParseTimeTags(expectedTimeTags);
            var lyric = new Lyric { Text = text };
            CheckGenerateResult(lyric, expected, config);
        }

        protected void CheckGenerateResult(Lyric lyric, string[] expectedTimeTags, TConfig config)
        {
            var expected = TestCaseTagHelper.ParseTimeTags(expectedTimeTags);
            CheckGenerateResult(lyric, expected, config);
        }

        protected override void AssertEqual(TimeTag[] expected, TimeTag[] actual)
        {
            TimeTagAssert.ArePropertyEqual(expected, actual);
        }
    }
}
