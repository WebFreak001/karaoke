﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps.Formats;
using osu.Game.IO;
using osu.Game.Rulesets.Karaoke.Beatmaps.Formats;
using osu.Game.Rulesets.Karaoke.Tests.Resources;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Karaoke.Tests.Beatmaps.Formats
{
    [TestFixture]
    public class NicoKaraDecoderTest
    {
        public NicoKaraDecoderTest()
        {
            // It's a tricky to let osu! to read karaoke testing beatmap
            NicoKaraDecoder.Register();
        }

        [Test]
        public void TestDecodeNicoKara()
        {
            using (var resStream = TestResources.OpenNicoKaraResource("default"))
            using (var stream = new LineBufferedReader(resStream))
            {
                var decoder = Decoder.GetDecoder<NicoKaraSkin>(stream);
                var skin = decoder.Decode(stream);

                // Testing layout
                var firstLayout = skin.Layouts.FirstOrDefault()!;
                Assert.IsNotNull(firstLayout);
                Assert.AreEqual("下-1", firstLayout.Name);
                Assert.AreEqual(Anchor.BottomRight, firstLayout.Alignment);
                Assert.AreEqual(30, firstLayout.HorizontalMargin);
                Assert.AreEqual(45, firstLayout.VerticalMargin);

                // Testing style
                var firstFont = skin.LyricStyles.FirstOrDefault()!;
                Assert.IsNotNull(firstFont);
                Assert.AreEqual("標準配色", firstFont.Name);

                // Because some property has been converted into shader, so should test shader property.
                var shaders = firstFont.LeftLyricTextShaders;
                Assert.IsNotNull(shaders);

                // Test outline shader.
                var outlineShader = (OutlineShader)shaders.FirstOrDefault()!;
                Assert.IsNotNull(outlineShader);
                Assert.AreEqual(new Color4(255, 255, 255, 255), outlineShader.OutlineColour);
                Assert.AreEqual(3, outlineShader.Radius);

                // Test shader convert result.
                var shadowShader = (ShadowShader)shaders.LastOrDefault()!;
                Assert.IsNotNull(shadowShader);
                Assert.AreEqual(new Vector2(3), shadowShader.ShadowOffset);

                // test lyric config
                var defaultLyricFontInfo = skin.DefaultLyricFontInfo;
                Assert.IsNotNull(defaultLyricFontInfo);
                Assert.AreEqual(KaraokeTextSmartHorizon.Multi, defaultLyricFontInfo.SmartHorizon);
                Assert.AreEqual(4, defaultLyricFontInfo.LyricsInterval);
                Assert.AreEqual(2, defaultLyricFontInfo.RubyInterval);
                Assert.AreEqual(LyricTextAlignment.Auto, defaultLyricFontInfo.RubyAlignment);
                Assert.AreEqual(4, defaultLyricFontInfo.RubyMargin);

                // Test main text font
                var mainTextFontInfo = defaultLyricFontInfo.MainTextFont;
                Assert.AreEqual("游明朝 Demibold", mainTextFontInfo.Family);
                Assert.AreEqual("Bold", mainTextFontInfo.Weight);
                Assert.AreEqual(40, mainTextFontInfo.Size);
                Assert.AreEqual(false, mainTextFontInfo.FixedWidth);
            }
        }
    }
}
