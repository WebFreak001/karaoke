﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Karaoke.Edit.Lyrics.Extends.RubyRomaji.Components;
using osu.Game.Rulesets.Karaoke.Objects;
using osu.Game.Rulesets.Karaoke.Objects.Types;
using osu.Game.Rulesets.Karaoke.Utils;

namespace osu.Game.Rulesets.Karaoke.Edit.Lyrics.Extends.RubyRomaji
{
    public abstract class TextTagEditSection<TTextTag> : LyricPropertySection where TTextTag : class, ITextTag, new()
    {
        protected TextTagEditSection()
        {
            // add create button.
            addCreateButton();
        }

        protected override void OnLyricChanged(Lyric lyric)
        {
            RemoveAll(x => x is LabelledTextTagTextBox<TTextTag>);

            if (lyric == null)
                return;

            AddRange(GetBindableTextTags(lyric).Select(x =>
            {
                string relativeToLyricText = TextTagUtils.GetTextFromLyric(x, lyric.Text);
                string range = TextTagUtils.PositionFormattedString(x);

                return CreateLabelledTextTagTextBox(lyric, x).With(t =>
                {
                    t.Label = relativeToLyricText;
                    t.Description = range;
                    t.TabbableContentContainer = this;
                });
            }));
        }

        protected override void UpdateDisabledState(bool disabled)
        {
            if (disabled)
                return;

            // should auto-focus to the first time-tag if change the lyric.
            var firstTextTagTextBox = Children.OfType<LabelledTextTagTextBox<TTextTag>>().FirstOrDefault();
            firstTextTagTextBox?.Focus();
        }

        private void addCreateButton()
        {
            var fillFlowContainer = Content as FillFlowContainer;

            // create new button.
            fillFlowContainer?.Insert(int.MaxValue, new CreateNewTextTagButton<TTextTag>
            {
                Text = "Create new",
                Action = AddTextTag
            });
        }

        protected abstract IBindableList<TTextTag> GetBindableTextTags(Lyric lyric);

        protected abstract LabelledTextTagTextBox<TTextTag> CreateLabelledTextTagTextBox(Lyric lyric, TTextTag textTag);

        protected abstract void AddTextTag(TTextTag textTag);
    }
}
