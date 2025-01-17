﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Karaoke.Objects;

namespace osu.Game.Rulesets.Karaoke.Edit.Generator.Lyrics.TimeTags
{
    public abstract class TimeTagGenerator<TConfig> : LyricPropertyGenerator<TimeTag[], TConfig>
        where TConfig : TimeTagGeneratorConfig, new()
    {
        protected TimeTagGenerator(TConfig config)
            : base(config)
        {
        }

        protected override LocalisableString? GetInvalidMessageFromItem(Lyric item)
        {
            if (string.IsNullOrEmpty(item.Text))
                return "Lyric should not be empty.";

            return null;
        }

        protected sealed override TimeTag[] GenerateFromItem(Lyric item)
        {
            var timeTags = new List<TimeTag>();
            string text = item.Text;

            if (string.IsNullOrEmpty(text))
                return timeTags.ToArray();

            if (string.IsNullOrWhiteSpace(text))
            {
                if (Config.CheckBlankLine.Value)
                    timeTags.Add(new TimeTag(new TextIndex(0)));

                return timeTags.ToArray();
            }

            // create tag at start of lyric
            timeTags.Add(new TimeTag(new TextIndex(0)));

            if (Config.CheckLineEndKeyUp.Value)
                timeTags.Add(new TimeTag(new TextIndex(text.Length - 1, TextIndex.IndexState.End)));

            TimeTagLogic(item, timeTags);

            return timeTags.OrderBy(x => x.Index).ToArray();
        }

        protected abstract void TimeTagLogic(Lyric lyric, List<TimeTag> timeTags);
    }
}
