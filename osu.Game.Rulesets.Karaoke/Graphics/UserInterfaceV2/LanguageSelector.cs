﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Globalization;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Karaoke.Graphics.UserInterface;
using osu.Game.Rulesets.Karaoke.Utils;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Karaoke.Graphics.UserInterfaceV2
{
    public partial class LanguageSelector : CompositeDrawable, IHasCurrentValue<CultureInfo>
    {
        private readonly LanguageSelectionSearchTextBox filter;

        private readonly BindableWithCurrent<CultureInfo> current = new();

        public Bindable<CultureInfo> Current
        {
            get => current.Current;
            set => current.Current = value;
        }

        public override bool AcceptsFocus => true;

        public override bool RequestsFocus => true;

        public LanguageSelector()
        {
            var languages = new BindableList<CultureInfo>(CultureInfoUtils.GetAvailableLanguages());

            RearrangeableLanguageListContainer languageList;
            InternalChild = new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                RowDimensions = new[]
                {
                    new Dimension(GridSizeMode.Absolute, 40),
                    new Dimension()
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        filter = new LanguageSelectionSearchTextBox
                        {
                            RelativeSizeAxes = Axes.X,
                        }
                    },
                    new Drawable[]
                    {
                        languageList = new RearrangeableLanguageListContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            RequestSelection = item =>
                            {
                                Current.Value = item;
                            },
                            Items = { BindTarget = languages }
                        }
                    }
                }
            };

            filter.Current.BindValueChanged(e => languageList.Filter(e.NewValue));
            Current.BindValueChanged(e => languageList.SelectedSet.Value = e.NewValue);
        }

        protected override void OnFocus(FocusEvent e)
        {
            base.OnFocus(e);

            GetContainingInputManager().ChangeFocus(filter);
        }

        private partial class LanguageSelectionSearchTextBox : SearchTextBox
        {
            protected override Color4 SelectionColour => Color4.Gray;

            public LanguageSelectionSearchTextBox()
            {
                PlaceholderText = @"type in keywords...";
            }
        }

        private partial class RearrangeableLanguageListContainer : RearrangeableTextFlowListContainer<CultureInfo>
        {
            protected override DrawableTextListItem CreateDrawable(CultureInfo item)
                => new DrawableLanguageListItem(item);

            private partial class DrawableLanguageListItem : DrawableTextListItem
            {
                public DrawableLanguageListItem(CultureInfo item)
                    : base(item)
                {
                }

                public override IEnumerable<LocalisableString> FilterTerms => new[]
                {
                    new LocalisableString(Model.Name),
                    new LocalisableString(Model.DisplayName),
                    new LocalisableString(Model.EnglishName),
                    new LocalisableString(Model.NativeName)
                };

                protected override void CreateDisplayContent(OsuTextFlowContainer textFlowContainer, CultureInfo model)
                    => textFlowContainer.AddText(CultureInfoUtils.GetLanguageDisplayText(model));
            }
        }
    }
}
