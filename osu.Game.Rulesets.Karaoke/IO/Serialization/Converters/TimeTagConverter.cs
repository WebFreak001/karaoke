﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using osu.Framework.Graphics.Sprites;
using osu.Game.Rulesets.Karaoke.Extensions;
using osu.Game.Rulesets.Karaoke.Objects;
using osu.Game.Rulesets.Karaoke.Utils;

namespace osu.Game.Rulesets.Karaoke.IO.Serialization.Converters
{
    public class TimeTagConverter : JsonConverter<TimeTag>
    {
        public override TimeTag ReadJson(JsonReader reader, Type objectType, TimeTag? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JToken.Load(reader);
            string? value = obj.Value<string>();

            if (string.IsNullOrEmpty(value))
                return new TimeTag(new TextIndex());

            var regex = new Regex("(?<index>[-0-9]+),(?<state>start|end)]:(?<time>[-0-9]+|s*|)");
            var result = regex.Match(value);
            if (!result.Success)
                return new TimeTag(new TextIndex());

            int index = result.GetGroupValue<int>("index");
            var state = result.GetGroupValue<string>("state") == "start" ? TextIndex.IndexState.Start : TextIndex.IndexState.End;
            int? time = result.GetGroupValue<int?>("time");

            return new TimeTag(new TextIndex(index, state), time);
        }

        public override void WriteJson(JsonWriter writer, TimeTag? value, JsonSerializer serializer)
        {
            ArgumentNullException.ThrowIfNull(value);

            var index = value.Index;
            string state = TextIndexUtils.GetValueByState(index, "start", "end");
            double? time = value.Time;

            string str = $"[{index.Index},{state}]:{time}";
            writer.WriteValue(str);
        }
    }
}
