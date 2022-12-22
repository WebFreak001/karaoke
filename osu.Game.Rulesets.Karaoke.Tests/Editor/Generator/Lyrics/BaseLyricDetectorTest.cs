// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using NUnit.Framework;
using osu.Game.Rulesets.Karaoke.Edit.Generator;
using osu.Game.Rulesets.Karaoke.Edit.Generator.Lyrics;
using osu.Game.Rulesets.Karaoke.Objects;

namespace osu.Game.Rulesets.Karaoke.Tests.Editor.Generator.Lyrics
{
    public abstract class BaseLyricDetectorTest<TDetector, TObject, TConfig>
        : BaseGeneratorTest<TConfig>
        where TDetector : class, ILyricPropertyDetector<TObject> where TConfig : IHasConfig<TConfig>, new()
    {
        protected static TDetector GenerateDetector(TConfig config)
        {
            if (Activator.CreateInstance(typeof(TDetector), config) is not TDetector detector)
                throw new ArgumentNullException(nameof(detector));

            return detector;
        }

        protected static void CheckCanDetect(Lyric lyric, bool canDetect, TConfig config)
        {
            var detector = GenerateDetector(config);

            CheckCanDetect(lyric, canDetect, detector);
        }

        protected static void CheckCanDetect(Lyric lyric, bool canDetect, TDetector detector)
        {
            bool actual = detector.CanDetect(lyric);
            Assert.AreEqual(canDetect, actual);
        }

        protected void CheckDetectResult(Lyric lyric, TObject expected, TConfig config)
        {
            var detector = GenerateDetector(config);

            CheckDetectResult(lyric, expected, detector);
        }

        protected void CheckDetectResult(Lyric lyric, TObject expected, TDetector detector)
        {
            // create time tag and actually time tag.
            var actual = detector.Detect(lyric);
            AssertEqual(expected, actual);
        }

        protected abstract void AssertEqual(TObject expected, TObject actual);
    }
}