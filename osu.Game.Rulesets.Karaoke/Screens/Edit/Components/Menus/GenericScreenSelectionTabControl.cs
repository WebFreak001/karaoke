// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Screens.Edit.Components.Menus;
using osuTK;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Components.Menus
{
    /// <summary>
    /// Copied from <see cref="EditorScreenSwitcherControl"/>
    /// </summary>
    /// <typeparam name="TScreenMode"></typeparam>
    public partial class GenericScreenSelectionTabControl<TScreenMode> : OsuTabControl<TScreenMode>
    {
        public GenericScreenSelectionTabControl()
        {
            AutoSizeAxes = Axes.X;
            RelativeSizeAxes = Axes.Y;

            TabContainer.RelativeSizeAxes &= ~Axes.X;
            TabContainer.AutoSizeAxes = Axes.X;
            TabContainer.Padding = new MarginPadding(10);
        }

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider colourProvider)
        {
            AccentColour = colourProvider.Light3;

            AddInternal(new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = colourProvider.Background2,
            });
        }

        protected override Dropdown<TScreenMode> CreateDropdown() => null;

        protected override TabItem<TScreenMode> CreateTabItem(TScreenMode value) => new TabItem(value);

        private partial class TabItem : OsuTabItem
        {
            private const float transition_length = 250;

            public TabItem(TScreenMode value)
                : base(value)
            {
                Text.Margin = new MarginPadding();
                Text.Anchor = Anchor.CentreLeft;
                Text.Origin = Anchor.CentreLeft;

                Text.Font = OsuFont.TorusAlternate;

                Bar.Expire();
            }

            [BackgroundDependencyLoader]
            private void load(OverlayColourProvider colourProvider)
            {
            }

            protected override void OnActivated()
            {
                base.OnActivated();
                Bar.ScaleTo(new Vector2(1, 5), transition_length, Easing.OutQuint);
            }

            protected override void OnDeactivated()
            {
                base.OnDeactivated();
                Bar.ScaleTo(Vector2.One, transition_length, Easing.OutQuint);
            }
        }
    }
}
