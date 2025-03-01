﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Karaoke.Graphics.Cursor;
using osu.Game.Rulesets.Karaoke.Objects;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Components.Timeline;

public partial class EditableLyricTimelineSelectionBlueprint : EditableTimelineSelectionBlueprint<Lyric>, IHasCustomTooltip<Lyric>
{
    private const float lyric_size = 20;

    public EditableLyricTimelineSelectionBlueprint(Lyric item)
        : base(item)
    {
        X = (float)Item.LyricStartTime;

        RelativeSizeAxes = Axes.X;
        Height = lyric_size;

        AddInternal(new Container
        {
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
            RelativeSizeAxes = Axes.Both,
            Masking = true,
            CornerRadius = 5,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Gray,
                },
                new OsuSpriteText
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Margin = new MarginPadding { Left = 5 },
                    RelativeSizeAxes = Axes.X,
                    Truncate = true,
                    Text = item.Text
                }
            }
        });
    }

    protected override void Update()
    {
        base.Update();

        // no bindable so we perform this every update
        float duration = (float)Item.LyricDuration;

        if (Width != duration)
        {
            Width = duration;
        }
    }

    public virtual ITooltip<Lyric> GetCustomTooltip() => new LyricTooltip();

    public Lyric TooltipContent => Item;
}
