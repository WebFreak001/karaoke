// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Karaoke.Configuration;
using osu.Game.Rulesets.Karaoke.Edit.Generator.Languages;
using osu.Game.Rulesets.Karaoke.Edit.Generator.Notes;
using osu.Game.Rulesets.Karaoke.Edit.Generator.RomajiTags;
using osu.Game.Rulesets.Karaoke.Edit.Generator.RubyTags;
using osu.Game.Rulesets.Karaoke.Edit.Generator.TimeTags;
using osu.Game.Rulesets.Karaoke.Edit.Generator.Types;
using osu.Game.Rulesets.Karaoke.Objects;
using osu.Game.Screens.Edit;

namespace osu.Game.Rulesets.Karaoke.Edit.ChangeHandlers.Lyrics
{
    public class LyricAutoGenerateChangeHandler : HitObjectChangeHandler<Lyric>, ILyricAutoGenerateChangeHandler
    {
        [Resolved, AllowNull]
        private KaraokeRulesetEditGeneratorConfigManager generatorConfigManager { get; set; }

        [Resolved, AllowNull]
        private EditorBeatmap beatmap { get; set; }

        public bool CanGenerate(LyricAutoGenerateProperty autoGenerateProperty)
        {
            switch (autoGenerateProperty)
            {
                case LyricAutoGenerateProperty.DetectLanguage:
                    var languageDetector = createLyricDetector<CultureInfo>();
                    return canDetect(languageDetector);

                case LyricAutoGenerateProperty.AutoGenerateRubyTags:
                    var rubyGenerator = createLyricGenerator<RubyTag[]>();
                    return canGenerate(rubyGenerator);

                case LyricAutoGenerateProperty.AutoGenerateRomajiTags:
                    var romajiGenerator = createLyricGenerator<RomajiTag[]>();
                    return canGenerate(romajiGenerator);

                case LyricAutoGenerateProperty.AutoGenerateTimeTags:
                    var timeTagGenerator = createLyricGenerator<TimeTag[]>();
                    return canGenerate(timeTagGenerator);

                case LyricAutoGenerateProperty.AutoGenerateNotes:
                    var noteGenerator = createLyricGenerator<Note[]>();
                    return canGenerate(noteGenerator);

                default:
                    throw new ArgumentOutOfRangeException(nameof(autoGenerateProperty));
            }

            bool canDetect<T>(ILyricPropertyDetector<T> detector)
                => HitObjects.Any(detector.CanDetect);

            bool canGenerate<T>(ILyricPropertyGenerator<T> generator)
                => HitObjects.Any(generator.CanGenerate);
        }

        public IDictionary<Lyric, LocalisableString> GetNotGeneratableLyrics(LyricAutoGenerateProperty autoGenerateProperty)
        {
            switch (autoGenerateProperty)
            {
                case LyricAutoGenerateProperty.DetectLanguage:
                    var languageDetector = createLyricDetector<CultureInfo>();
                    return getInvalidMessageFromDetector(languageDetector);

                case LyricAutoGenerateProperty.AutoGenerateRubyTags:
                    var rubyGenerator = createLyricGenerator<RubyTag[]>();
                    return getInvalidMessageFromGenerator(rubyGenerator);

                case LyricAutoGenerateProperty.AutoGenerateRomajiTags:
                    var romajiGenerator = createLyricGenerator<RomajiTag[]>();
                    return getInvalidMessageFromGenerator(romajiGenerator);

                case LyricAutoGenerateProperty.AutoGenerateTimeTags:
                    var timeTagGenerator = createLyricGenerator<TimeTag[]>();
                    return getInvalidMessageFromGenerator(timeTagGenerator);

                case LyricAutoGenerateProperty.AutoGenerateNotes:
                    var noteGenerator = createLyricGenerator<Note[]>();
                    return getInvalidMessageFromGenerator(noteGenerator);

                default:
                    throw new ArgumentOutOfRangeException(nameof(autoGenerateProperty));
            }

            IDictionary<Lyric, LocalisableString> getInvalidMessageFromDetector<T>(ILyricPropertyDetector<T> detector)
                => HitObjects.Select(x => new KeyValuePair<Lyric, LocalisableString?>(x, detector.GetInvalidMessage(x)))
                             .Where(x => x.Value != null)
                             .ToDictionary(k => k.Key, v => v.Value!.Value);

            IDictionary<Lyric, LocalisableString> getInvalidMessageFromGenerator<T>(ILyricPropertyGenerator<T> generator)
                => HitObjects.Select(x => new KeyValuePair<Lyric, LocalisableString?>(x, generator.GetInvalidMessage(x)))
                             .Where(x => x.Value != null)
                             .ToDictionary(k => k.Key, v => v.Value!.Value);
        }

        public void AutoGenerate(LyricAutoGenerateProperty autoGenerateProperty)
        {
            switch (autoGenerateProperty)
            {
                case LyricAutoGenerateProperty.DetectLanguage:
                    var languageDetector = createLyricDetector<CultureInfo>();
                    PerformOnSelection(lyric =>
                    {
                        var detectedLanguage = languageDetector.Detect(lyric);
                        lyric.Language = detectedLanguage;
                    });
                    break;

                case LyricAutoGenerateProperty.AutoGenerateRubyTags:
                    var rubyGenerator = createLyricGenerator<RubyTag[]>();
                    PerformOnSelection(lyric =>
                    {
                        lyric.RubyTags = rubyGenerator.Generate(lyric);
                    });
                    break;

                case LyricAutoGenerateProperty.AutoGenerateRomajiTags:
                    var romajiGenerator = createLyricGenerator<RomajiTag[]>();
                    PerformOnSelection(lyric =>
                    {
                        lyric.RomajiTags = romajiGenerator.Generate(lyric);
                    });
                    break;

                case LyricAutoGenerateProperty.AutoGenerateTimeTags:
                    var timeTagGenerator = createLyricGenerator<TimeTag[]>();
                    PerformOnSelection(lyric =>
                    {
                        lyric.TimeTags = timeTagGenerator.Generate(lyric);
                    });
                    break;

                case LyricAutoGenerateProperty.AutoGenerateNotes:
                    var noteGenerator = createLyricGenerator<Note[]>();
                    PerformOnSelection(lyric =>
                    {
                        // clear exist notes if from those
                        var matchedNotes = beatmap.HitObjects.OfType<Note>().Where(x => x.ParentLyric == lyric).ToArray();
                        RemoveRange(matchedNotes);

                        var notes = noteGenerator.Generate(lyric);
                        AddRange(notes);
                    });
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(autoGenerateProperty));
            }
        }

        private ILyricPropertyDetector<T> createLyricDetector<T>()
        {
            switch (typeof(T))
            {
                case Type t when t == typeof(CultureInfo):
                    var config = generatorConfigManager.Get<LanguageDetectorConfig>(KaraokeRulesetEditGeneratorSetting.LanguageDetectorConfig);
                    return (ILyricPropertyDetector<T>)new LanguageDetector(config);

                default:
                    throw new NotSupportedException();
            }
        }

        private ILyricPropertyGenerator<T> createLyricGenerator<T>()
        {
            switch (typeof(T))
            {
                case Type t when t == typeof(RubyTag[]):
                    return (ILyricPropertyGenerator<T>)new RubyTagGeneratorSelector(generatorConfigManager);

                case Type t when t == typeof(RomajiTag[]):
                    return (ILyricPropertyGenerator<T>)new RomajiTagGeneratorSelector(generatorConfigManager);

                case Type t when t == typeof(TimeTag[]):
                    return (ILyricPropertyGenerator<T>)new TimeTagGeneratorSelector(generatorConfigManager);

                case Type t when t == typeof(Note[]):
                    var config = generatorConfigManager.Get<NoteGeneratorConfig>(KaraokeRulesetEditGeneratorSetting.NoteGeneratorConfig);
                    return (ILyricPropertyGenerator<T>)new NoteGenerator(config);

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
