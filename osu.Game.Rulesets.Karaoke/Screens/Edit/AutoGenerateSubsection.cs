// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Graphics.UserInterface;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Rulesets.Karaoke.Configuration;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Components.Markdown;
using osuTK;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit;

public abstract partial class AutoGenerateSubsection : FillFlowContainer
{
    private const int horizontal_padding = 20;

    protected AutoGenerateSubsection()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        // should wait until BDL in the parent class has been loaded.
        Schedule(() =>
        {
            Children = new Drawable[]
            {
                new GridContainer
                {
                    AutoSizeAxes = Axes.Y,
                    RelativeSizeAxes = Axes.X,
                    ColumnDimensions = new[]
                    {
                        new Dimension(),
                        new Dimension(GridSizeMode.Absolute, 5),
                        new Dimension(GridSizeMode.Absolute, 36)
                    },
                    RowDimensions = new[]
                    {
                        new Dimension(GridSizeMode.AutoSize)
                    },
                    Content = new[]
                    {
                        new Drawable?[]
                        {
                            CreateGenerateButton(),
                            null,
                            CreateConfigButton().With(x =>
                            {
                                x.Anchor = Anchor.Centre;
                                x.Origin = Anchor.Centre;
                                x.Size = new Vector2(36);
                            })
                        }
                    },
                },
                CreateDescriptionTextFlowContainer().With(x =>
                {
                    x.RelativeSizeAxes = Axes.X;
                    x.AutoSizeAxes = Axes.Y;
                    x.Padding = new MarginPadding { Horizontal = horizontal_padding };
                    x.Description = CreateInvalidDescriptionFormat();
                })
            };
        });
    }

    protected abstract EditorSectionButton CreateGenerateButton();

    protected virtual DescriptionTextFlowContainer CreateDescriptionTextFlowContainer() => new();

    protected abstract DescriptionFormat CreateInvalidDescriptionFormat();

    protected abstract ConfigButton CreateConfigButton();

    protected abstract partial class ConfigButton : IconButton, IHasPopover
    {
        protected ConfigButton()
        {
            Icon = FontAwesome.Solid.Cog;
            Action = openConfigSetting;

            void openConfigSetting()
                => this.ShowPopover();
        }

        public abstract Popover GetPopover();
    }

    protected abstract partial class MultiConfigButton : ConfigButton
    {
        private KaraokeRulesetEditGeneratorSetting? selectedSetting;

        protected MultiConfigButton()
        {
            Action = this.ShowPopover;
        }

        public sealed override Popover GetPopover()
        {
            if (selectedSetting == null)
                return createSelectionPopover();

            return new GeneratorConfigPopover(selectedSetting.Value);
        }

        protected abstract IEnumerable<KaraokeRulesetEditGeneratorSetting> AvailableSettings { get; }

        protected abstract string GetDisplayName(KaraokeRulesetEditGeneratorSetting setting);

        private Popover createSelectionPopover()
            => new OsuPopover
            {
                Child = new FillFlowContainer<OsuButton>
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(10),
                    Children = AvailableSettings.Select(x =>
                    {
                        string name = GetDisplayName(x);
                        return new AutoGenerateButton
                        {
                            Text = name,
                            Width = 150,
                            Action = () =>
                            {
                                selectedSetting = x;
                                this.ShowPopover();

                                // after show config pop-over, should make the state back for able to show this dialog next time.
                                selectedSetting = null;
                            },
                        };
                    }).ToList()
                }
            };

        private partial class AutoGenerateButton : EditorSectionButton
        {
        }
    }
}
