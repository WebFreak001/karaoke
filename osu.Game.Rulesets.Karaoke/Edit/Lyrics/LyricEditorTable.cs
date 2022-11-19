﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Extensions.LocalisationExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Screens.Edit;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Karaoke.Edit.Lyrics;

/// <summary>
/// Just a copy of class <see cref="EditorTable"/>
/// </summary>
public abstract class LyricEditorTable : TableContainer
{
    private const float horizontal_inset = 20;

    protected const float ROW_HEIGHT = 25;

    public const int TEXT_SIZE = 14;

    protected readonly FillFlowContainer<RowBackground> BackgroundFlow;

    protected LyricEditorTable()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        Padding = new MarginPadding { Horizontal = horizontal_inset };
        RowSize = new Dimension(GridSizeMode.Absolute, ROW_HEIGHT);

        AddInternal(BackgroundFlow = new FillFlowContainer<RowBackground>
        {
            RelativeSizeAxes = Axes.Both,
            Depth = 1f,
            Padding = new MarginPadding { Horizontal = -horizontal_inset },
            Margin = new MarginPadding { Top = ROW_HEIGHT }
        });
    }

    protected override Drawable CreateHeader(int index, TableColumn column) => new HeaderText(column?.Header ?? default);

    private class HeaderText : OsuSpriteText
    {
        public HeaderText(LocalisableString text)
        {
            Text = text.ToUpper();
            Font = OsuFont.GetFont(size: 12, weight: FontWeight.Bold);
        }
    }

    public class RowBackground : OsuClickableContainer
    {
        public readonly object Item;

        private const int fade_duration = 100;

        private readonly Box hoveredBackground;

        public RowBackground(object item)
        {
            Item = item;

            RelativeSizeAxes = Axes.X;
            Height = 25;

            AlwaysPresent = true;

            CornerRadius = 3;
            Masking = true;

            Children = new Drawable[]
            {
                hoveredBackground = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0,
                },
            };

            // todo delete
            Action = () =>
            {
            };
        }

        private Color4 colourHover;
        private Color4 colourSelected;

        [BackgroundDependencyLoader]
        private void load(LyricEditorColourProvider colourProvider, ILyricEditorState state)
        {
            hoveredBackground.Colour = colourHover = colourProvider.Background1(state.Mode);
            colourSelected = colourProvider.Colour3(state.Mode);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            updateState();
            FinishTransforms(true);
        }

        private bool selected;

        public bool Selected
        {
            get => selected;
            set
            {
                if (value == selected)
                    return;

                selected = value;
                updateState();
            }
        }

        protected override bool OnHover(HoverEvent e)
        {
            updateState();
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            updateState();
            base.OnHoverLost(e);
        }

        private void updateState()
        {
            hoveredBackground.FadeColour(selected ? colourSelected : colourHover, 450, Easing.OutQuint);

            if (selected || IsHovered)
                hoveredBackground.FadeIn(fade_duration, Easing.OutQuint);
            else
                hoveredBackground.FadeOut(fade_duration, Easing.OutQuint);
        }
    }
}