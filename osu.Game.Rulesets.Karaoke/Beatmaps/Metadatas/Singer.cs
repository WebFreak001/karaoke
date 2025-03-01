﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using Newtonsoft.Json;
using osu.Framework.Bindables;
using osu.Game.Rulesets.Karaoke.Beatmaps.Metadatas.Types;

namespace osu.Game.Rulesets.Karaoke.Beatmaps.Metadatas
{
    public class Singer : ISinger
    {
        public Singer()
        {
        }

        public Singer(int id)
        {
            ID = id;
        }

        public int ID { get; protected set; }

        [JsonIgnore]
        public readonly Bindable<int> OrderBindable = new();

        /// <summary>
        /// Order
        /// </summary>
        public int Order
        {
            get => OrderBindable.Value;
            set => OrderBindable.Value = value;
        }

        [JsonIgnore]
        public readonly Bindable<string> AvatarFileBindable = new();

        public string AvatarFile
        {
            get => AvatarFileBindable.Value;
            set => AvatarFileBindable.Value = value;
        }

        [JsonIgnore]
        public Bindable<float> HueBindable = new BindableFloat
        {
            MinValue = 0,
            MaxValue = 1
        };

        public float Hue
        {
            get => HueBindable.Value;
            set => HueBindable.Value = value;
        }

        [JsonIgnore]
        public readonly Bindable<string> NameBindable = new();

        public string Name
        {
            get => NameBindable.Value;
            set => NameBindable.Value = value;
        }

        [JsonIgnore]
        public readonly Bindable<string> RomajiNameBindable = new();

        public string RomajiName
        {
            get => RomajiNameBindable.Value;
            set => RomajiNameBindable.Value = value;
        }

        [JsonIgnore]
        public readonly Bindable<string> EnglishNameBindable = new();

        public string EnglishName
        {
            get => EnglishNameBindable.Value;
            set => EnglishNameBindable.Value = value;
        }

        [JsonIgnore]
        public readonly Bindable<string> DescriptionBindable = new();

        public string Description
        {
            get => DescriptionBindable.Value;
            set => DescriptionBindable.Value = value;
        }
    }
}
