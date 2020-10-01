﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Karaoke.Beatmaps;
using osu.Game.Rulesets.Karaoke.Skinning.Components;
using osu.Game.Rulesets.Karaoke.Statistics;
using osu.Game.Rulesets.Karaoke.Tests.Beatmaps;
using osu.Game.Scoring;
using osu.Game.Tests.Visual;
using osuTK;
using osuTK.Graphics;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Karaoke.Tests.Ranking
{
    public class TestSceneBeatmapMetadataGraph : OsuTestScene
    {
        [Test]
        public void TestBeatmapMetadataGraph()
        {
            var ruleset = new KaraokeRuleset().RulesetInfo;
            var originBeatmap = new TestKaraokeBeatmap(ruleset);
            var karaokeBeatmap = new KaraokeBeatmapConverter(originBeatmap, new KaraokeRuleset()).Convert() as KaraokeBeatmap;
            karaokeBeatmap.Singers = createDefaultSinger();
            createTest(new ScoreInfo(), karaokeBeatmap);
        }

        private void createTest(ScoreInfo score, IBeatmap beatmap) => AddStep("create test", () =>
        {
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4Extensions.FromHex("#333")
                },
                new BeatmapMetadataGraph(beatmap)
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = new Vector2(600, 200)
                }
            };
        });

        private IDictionary<int, Singer> createDefaultSinger()
        {
            return new Dictionary<int, Singer>
            {
                {
                    0,
                    new Singer
                    {
                        Name = "Singer 001",
                        Romaji = "",
                        EnglishName = "",
                        Color = Color4.Blue
                    }
                },
                {
                    1,
                    new Singer
                    {
                        Name = "Singer 002",
                        Romaji = "",
                        EnglishName = "",
                        Color = Color4.Red
                    }
                },
                {
                    2,
                    new Singer
                    {
                        Name = "Singer 003",
                        Romaji = "",
                        EnglishName = "",
                        Color = Color4.Yellow
                    }
                }
            };
        }
    }
}