// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Karaoke.Beatmaps;
using osu.Game.Rulesets.Karaoke.Edit.Utils;
using osu.Game.Rulesets.Karaoke.Objects;
using osu.Game.Screens.Edit;

namespace osu.Game.Rulesets.Karaoke.Edit.ChangeHandlers
{
    // todo: not a good design because eventually karaoke beatmap will not have the the field with list type.
    // it should be wrap into class (e.g. localizationInfo) with list of translate inside.
    // so guess this class will be removed eventually.
    public abstract partial class BeatmapListPropertyChangeHandler<TItem> : Component
    {
        [Resolved, AllowNull]
        private EditorBeatmap beatmap { get; set; }

        private KaraokeBeatmap karaokeBeatmap => EditorBeatmapUtils.GetPlayableBeatmap(beatmap);

        protected IEnumerable<Lyric> Lyrics => karaokeBeatmap.HitObjects.OfType<Lyric>();

        // todo: should be interface.
        protected BindableList<TItem> Items = new();

        [BackgroundDependencyLoader]
        private void load()
        {
            Items.AddRange(GetItemsFromBeatmap(karaokeBeatmap));

            // todo: find a better way to handle only beatmap property changed.
            beatmap.TransactionEnded += syncItemsFromBeatmap;

            syncItemsFromBeatmap();

            void syncItemsFromBeatmap()
            {
                var items = GetItemsFromBeatmap(karaokeBeatmap);

                if (Items.SequenceEqual(items))
                    return;

                Items.AddRange(items.Except(Items));
                Items.RemoveAll(x => !items.Contains(x));
            }
        }

        protected void PerformObjectChanged(TItem item, Action<TItem>? action)
        {
            // should call change from editor beatmap because there's only way to trigger transaction ended.
            beatmap.BeginChange();
            action?.Invoke(item);
            beatmap.EndChange();
        }

        protected abstract IList<TItem> GetItemsFromBeatmap(KaraokeBeatmap beatmap);

        public void Add(TItem item)
        {
            var items = GetItemsFromBeatmap(karaokeBeatmap);
            if (items.Contains(item))
                throw new InvalidOperationException(nameof(item));

            PerformObjectChanged(item, i =>
            {
                items.Add(i);
                OnItemAdded(i);
            });
        }

        public void Remove(TItem item)
        {
            var items = GetItemsFromBeatmap(karaokeBeatmap);
            if (!items.Contains(item))
                throw new InvalidOperationException($"{nameof(item)} is not in the list");

            PerformObjectChanged(item, i =>
            {
                items.Remove(i);
                OnItemRemoved(i);
            });
        }

        protected abstract void OnItemAdded(TItem item);

        protected abstract void OnItemRemoved(TItem item);
    }
}
