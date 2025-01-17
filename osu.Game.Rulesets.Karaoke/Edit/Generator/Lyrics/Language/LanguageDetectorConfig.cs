﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Globalization;
using osu.Framework.Bindables;

namespace osu.Game.Rulesets.Karaoke.Edit.Generator.Lyrics.Language
{
    public class LanguageDetectorConfig : GeneratorConfig
    {
        [ConfigSource("Accept languages", "All accepted languages.")]
        public Bindable<CultureInfo[]> AcceptLanguages { get; } = new(Array.Empty<CultureInfo>());
    }
}
