﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Edit.Checks.Components;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Components.Issues;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Karaoke.Tests.Screens.Edit.Components.Issues
{
    [TestFixture]
    public partial class TestSceneIssuesToolTip : OsuTestScene
    {
        private IssuesToolTip toolTip = null!;

        [SetUp]
        public void SetUp() => Schedule(() =>
        {
            Child = toolTip = new IssuesToolTip
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            };
            toolTip.Show();
        });

        [Test]
        public void TestValidLyric()
        {
            setTooltip("valid lyric");
        }

        private void setTooltip(string testName, params Issue[] issues)
        {
            AddStep(testName, () =>
            {
                toolTip.SetContent(issues);
            });
        }
    }
}
