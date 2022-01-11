// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Game.Rulesets.Karaoke.Configuration;
using osu.Game.Rulesets.Karaoke.Edit.Generator.RomajiTags;
using osu.Game.Rulesets.Karaoke.Objects;
using osu.Game.Rulesets.Karaoke.Utils;

namespace osu.Game.Rulesets.Karaoke.Edit.ChangeHandlers.Lyrics
{
    public class LyricRomajiTagsChangeHandler : LyricTextTagsChangeHandler<RomajiTag>, ILyricRomajiTagsChangeHandler
    {
        private RomajiTagGeneratorSelector selector;

        [BackgroundDependencyLoader]
        private void load(KaraokeRulesetEditGeneratorConfigManager config)
        {
            selector = new RomajiTagGeneratorSelector(config);
        }

        public void AutoGenerate()
        {
            PerformOnSelection(lyric =>
            {
                var romajiTags = selector.GenerateRomajiTags(lyric);
                lyric.RomajiTags = romajiTags;
            });
        }

        public bool CanGenerate()
        {
            return HitObjects.Any(lyric => selector.CanGenerate(lyric));
        }

        protected override bool ContainsInLyric(Lyric lyric, RomajiTag textTag)
            => lyric.RomajiTags.Contains(textTag);

        protected override void AddToLyric(Lyric lyric, RomajiTag textTag)
        {
            var romajiTags = lyric.RomajiTags.ToList();
            romajiTags.Add(textTag);

            lyric.RomajiTags = TextTagsUtils.Sort(romajiTags);
        }

        protected override void RemoveFromLyric(Lyric lyric, RomajiTag textTag)
        {
            var romajiTags = lyric.RomajiTags.ToList();
            romajiTags.Remove(textTag);

            lyric.RomajiTags = romajiTags.ToArray();
        }
    }
}