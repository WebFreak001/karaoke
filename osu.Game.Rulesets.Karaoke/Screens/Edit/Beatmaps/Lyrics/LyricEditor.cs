﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Karaoke.Configuration;
using osu.Game.Rulesets.Karaoke.Extensions;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.Compose;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.LyricList;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.Settings;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.States;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.States.Modes;
using osu.Game.Rulesets.Timing;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Rulesets.UI.Scrolling.Algorithms;
using osu.Game.Screens;
using osu.Game.Screens.Edit;
using osuTK;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics
{
    [Cached(typeof(ILyricEditorState))]
    public partial class LyricEditor : Container, ILyricEditorState, IKeyBindingHandler<KaraokeEditAction>, IKeyBindingHandler<PlatformAction>
    {
        [Cached]
        private readonly LyricEditorColourProvider colourProvider = new();

        [Cached(typeof(ILyricSelectionState))]
        private readonly LyricSelectionState lyricSelectionState;

        [Cached(typeof(ILyricCaretState))]
        private readonly LyricCaretState lyricCaretState;

        [Cached(typeof(ITextingModeState))]
        private readonly TextingModeState textingModeState;

        [Cached(typeof(IEditReferenceLyricModeState))]
        private readonly EditReferenceLyricModeState editReferenceLyricModeState;

        [Cached(typeof(ILanguageModeState))]
        private readonly LanguageModeState languageModeState;

        [Cached(typeof(IEditRubyModeState))]
        private readonly EditRubyModeState editRubyModeState;

        [Cached(typeof(IEditRomajiModeState))]
        private readonly EditRomajiModeState editRomajiModeState;

        [Cached(typeof(ITimeTagModeState))]
        private readonly TimeTagModeState timeTagModeState;

        [Cached(typeof(IEditNoteModeState))]
        private readonly EditNoteModeState editNoteModeState;

        [Cached(typeof(ILyricEditorClipboard))]
        private readonly LyricEditorClipboard lyricEditorClipboard;

        [Cached(typeof(ILyricEditorVerifier))]
        private readonly LyricEditorVerifier lyricEditorVerifier;

        [Cached(typeof(IIssueNavigator))]
        private readonly IssueNavigator issueNavigator;

        [Cached(typeof(IScrollingInfo))]
        private readonly LocalScrollingInfo scrollingInfo = new();

        [Cached]
        private readonly BindableBeatDivisor beatDivisor = new();

        private readonly Bindable<LyricEditorMode> bindableMode = new();
        private readonly Bindable<ModeWithSubMode> bindableModeAndSubMode = new();
        private readonly IBindable<LyricEditorLayout> bindablePreferLayout = new Bindable<LyricEditorLayout>(LyricEditorLayout.Preview);
        private readonly Bindable<LyricEditorLayout> bindableCurrentLayout = new();

        public IBindable<LyricEditorMode> BindableMode => bindableMode;

        public IBindable<ModeWithSubMode> BindableModeAndSubMode => bindableModeAndSubMode;

        private readonly GridContainer gridContainer;
        private readonly Container editArea;
        private readonly LoadingSpinner loading;
        private readonly Container leftSideSettings;
        private readonly Container rightSideSettings;

        public LyricEditor()
        {
            // global state
            AddInternal(lyricSelectionState = new LyricSelectionState());
            AddInternal(lyricCaretState = new LyricCaretState());

            // state for target mode only.
            AddInternal(textingModeState = new TextingModeState());
            AddInternal(editReferenceLyricModeState = new EditReferenceLyricModeState());
            AddInternal(languageModeState = new LanguageModeState());
            AddInternal(editRubyModeState = new EditRubyModeState());
            AddInternal(editRomajiModeState = new EditRomajiModeState());
            AddInternal(timeTagModeState = new TimeTagModeState());
            AddInternal(editNoteModeState = new EditNoteModeState());

            // Separated feature.
            AddInternal(lyricEditorClipboard = new LyricEditorClipboard());
            AddInternal(lyricEditorVerifier = new LyricEditorVerifier());
            AddInternal(issueNavigator = new IssueNavigator());

            Add(gridContainer = new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                Content = new[]
                {
                    new Drawable[]
                    {
                        leftSideSettings = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                        },
                        editArea = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Children = new[]
                            {
                                loading = new LoadingSpinner(true)
                                {
                                    Depth = int.MinValue,
                                },
                            }
                        },
                        rightSideSettings = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                        },
                    }
                }
            });

            BindableMode.BindValueChanged(e =>
            {
                updateTheSubMode();

                // should control grid container spacing and place some component.
                initializeSettingsArea();

                reCalculateLayout();

                // cancel selecting if switch mode.
                lyricSelectionState.EndSelecting(LyricEditorSelectingAction.Cancel);
            }, true);

            initialSubModeChanged<TextingEditMode>(); // texting
            initialSubModeChanged<LanguageEditMode>(); // language
            initialSubModeChanged<RubyTagEditMode>(); // ruby
            initialSubModeChanged<RomajiTagEditMode>(); // romaji
            initialSubModeChanged<TimeTagEditMode>(); //time-tag
            initialSubModeChanged<NoteEditMode>(); // note

            bindablePreferLayout.BindValueChanged(e =>
            {
                reCalculateLayout();
            });

            bindableCurrentLayout.BindValueChanged(e =>
            {
                Schedule(() =>
                {
                    // should switch the layout after loaded.
                    switchLayout(e.NewValue);
                });
            }, true);
        }

        private void initialSubModeChanged<TSubMode>() where TSubMode : Enum
        {
            var editModeState = getEditModeState<TSubMode>();
            if (editModeState == null)
                throw new ArgumentNullException();

            editModeState.BindableEditMode.BindValueChanged(e =>
            {
                updateTheSubMode();
            });
        }

        private void updateTheSubMode()
        {
            bindableModeAndSubMode.Value = new ModeWithSubMode
            {
                Mode = bindableMode.Value,
                SubMode = getTheSubMode(bindableMode.Value),
                Default = false,
            };

            Enum getTheSubMode(LyricEditorMode mode) =>
                mode switch
                {
                    LyricEditorMode.View => null,
                    LyricEditorMode.Texting => textingModeState.BindableEditMode.Value,
                    LyricEditorMode.Reference => null,
                    LyricEditorMode.Language => languageModeState.BindableEditMode.Value,
                    LyricEditorMode.EditRuby => editRubyModeState.BindableEditMode.Value,
                    LyricEditorMode.EditRomaji => editRomajiModeState.BindableEditMode.Value,
                    LyricEditorMode.EditTimeTag => timeTagModeState.BindableEditMode.Value,
                    LyricEditorMode.EditNote => editNoteModeState.BindableEditMode.Value,
                    LyricEditorMode.Singer => null,
                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
                };
        }

        private void initializeSettingsArea()
        {
            var settings = getSettings();
            if (settings != null && checkDuplicatedWithExistSettings(settings))
                return;

            leftSideSettings.Clear();
            rightSideSettings.Clear();

            var direction = settings?.Direction;
            float width = settings?.SettingsWidth ?? 0;

            gridContainer.ColumnDimensions = new[]
            {
                new Dimension(GridSizeMode.Absolute, direction == SettingsDirection.Left ? width : 0),
                new Dimension(),
                new Dimension(GridSizeMode.Absolute, direction == SettingsDirection.Right ? width : 0),
            };

            if (settings == null)
                return;

            switch (settings.Direction)
            {
                case SettingsDirection.Left:
                    leftSideSettings.Add(settings);
                    break;

                case SettingsDirection.Right:
                    rightSideSettings.Add(settings);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.Direction));
            }

            LyricEditorSettings getSettings() =>
                Mode switch
                {
                    LyricEditorMode.Texting => new TextingSettings(),
                    LyricEditorMode.Reference => new ReferenceSettings(),
                    LyricEditorMode.Language => new LanguageSettings(),
                    LyricEditorMode.EditRuby => new RubyTagSettings(),
                    LyricEditorMode.EditRomaji => new RomajiTagSettings(),
                    LyricEditorMode.EditTimeTag => new TimeTagSettings(),
                    LyricEditorMode.EditNote => new NoteSettings(),
                    LyricEditorMode.Singer => new SingerSettings(),
                    _ => null
                };

            bool checkDuplicatedWithExistSettings(LyricEditorSettings lyricEditorSettings)
            {
                var type = lyricEditorSettings.GetType();
                if (leftSideSettings.Children?.FirstOrDefault()?.GetType() == type)
                    return true;

                if (rightSideSettings.Children?.FirstOrDefault()?.GetType() == type)
                    return true;

                return false;
            }
        }

        private void reCalculateLayout()
        {
            var supportedLayout = getSupportedLayout(Mode);
            var preferLayout = bindablePreferLayout.Value;

            bindableCurrentLayout.Value = GetSuitableLayout(supportedLayout, preferLayout);

            static LyricEditorLayout getSupportedLayout(LyricEditorMode mode) =>
                mode switch
                {
                    LyricEditorMode.View => LyricEditorLayout.Preview,
                    LyricEditorMode.Texting => LyricEditorLayout.Preview | LyricEditorLayout.Detail,
                    LyricEditorMode.Reference => LyricEditorLayout.Preview | LyricEditorLayout.Detail,
                    LyricEditorMode.Language => LyricEditorLayout.Preview | LyricEditorLayout.Detail,
                    LyricEditorMode.EditRuby => LyricEditorLayout.Preview | LyricEditorLayout.Detail,
                    LyricEditorMode.EditRomaji => LyricEditorLayout.Preview | LyricEditorLayout.Detail,
                    LyricEditorMode.EditTimeTag => LyricEditorLayout.Detail,
                    LyricEditorMode.EditNote => LyricEditorLayout.Detail,
                    LyricEditorMode.Singer => LyricEditorLayout.Preview,
                    _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
                };
        }

        internal static LyricEditorLayout GetSuitableLayout(LyricEditorLayout supportedLayout, LyricEditorLayout preferLayout)
        {
            var union = supportedLayout & preferLayout;
            return union != 0 ? union : supportedLayout;
        }

        private void switchLayout(LyricEditorLayout layout)
        {
            loading.Show();

            LoadComponentAsync(new DelayedLoadWrapper(getContent(layout).With(x =>
            {
                x.RelativeSizeAxes = Axes.Both;
            })).With(x =>
            {
                x.RelativeSizeAxes = Axes.Both;
                x.RelativePositionAxes = Axes.Y;
                x.Y = -0.5f;
                x.Alpha = 0;
            }), content =>
            {
                const double remove_old_editor_time = 300;
                const double new_animation_time = 1000;

                var oldComponent = editArea.Children.Where(x => x != loading).OfType<DelayedLoadWrapper>().FirstOrDefault();
                oldComponent?.MoveToY(-0.5f, remove_old_editor_time).FadeOut(remove_old_editor_time).OnComplete(x =>
                {
                    x.Expire();
                });

                editArea.Add(content);
                content.Delay(oldComponent != null ? remove_old_editor_time : 0)
                       .Then()
                       .FadeIn(new_animation_time)
                       .MoveToY(0, new_animation_time)
                       .OnComplete(_ =>
                       {
                           loading.Hide();
                       });
            });

            static Container getContent(LyricEditorLayout layout) =>
                layout switch
                {
                    LyricEditorLayout.Preview => new Container
                    {
                        Children = new[]
                        {
                            new PreviewLyricList
                            {
                                RelativeSizeAxes = Axes.Both
                            }
                        }
                    },
                    LyricEditorLayout.Detail => new Container
                    {
                        Children = new Drawable[]
                        {
                            new LyricComposer
                            {
                                RelativeSizeAxes = Axes.Both,
                                Size = new Vector2(1, 0.6f)
                            },
                            new DetailLyricList
                            {
                                RelativePositionAxes = Axes.Y,
                                Position = new Vector2(0, 0.6f),
                                Size = new Vector2(1, 0.4f),
                                RelativeSizeAxes = Axes.Both
                            },
                        }
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(layout), layout, null)
                };
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            var baseDependencies = new DependencyContainer(base.CreateChildDependencies(parent));

            // Add shader manager as part of dependencies.
            // it will call CreateResourceStore() in KaraokeRuleset and add the resource.
            return new OsuScreenDependencies(false, new DrawableRulesetDependencies(baseDependencies.GetRuleset(), baseDependencies));
        }

        [BackgroundDependencyLoader]
        private void load(EditorBeatmap beatmap, KaraokeRulesetLyricEditorConfigManager lyricEditorConfigManager)
        {
            // set-up divisor.
            beatDivisor.Value = beatmap.BeatmapInfo.BeatDivisor;
            lyricEditorConfigManager.BindWith(KaraokeRulesetLyricEditorSetting.LyricEditorPreferLayout, bindablePreferLayout);
        }

        public virtual bool OnPressed(KeyBindingPressEvent<KaraokeEditAction> e) =>
            e.Action switch
            {
                KaraokeEditAction.MoveToPreviousLyric => lyricCaretState.MoveCaret(MovingCaretAction.PreviousLyric),
                KaraokeEditAction.MoveToNextLyric => lyricCaretState.MoveCaret(MovingCaretAction.NextLyric),
                KaraokeEditAction.MoveToFirstLyric => lyricCaretState.MoveCaret(MovingCaretAction.FirstLyric),
                KaraokeEditAction.MoveToLastLyric => lyricCaretState.MoveCaret(MovingCaretAction.LastLyric),
                KaraokeEditAction.MoveToPreviousIndex => lyricCaretState.MoveCaret(MovingCaretAction.PreviousIndex),
                KaraokeEditAction.MoveToNextIndex => lyricCaretState.MoveCaret(MovingCaretAction.NextIndex),
                KaraokeEditAction.MoveToFirstIndex => lyricCaretState.MoveCaret(MovingCaretAction.FirstIndex),
                KaraokeEditAction.MoveToLastIndex => lyricCaretState.MoveCaret(MovingCaretAction.LastIndex),
                _ => false
            };

        public void OnReleased(KeyBindingReleaseEvent<KaraokeEditAction> e)
        {
        }

        public bool OnPressed(KeyBindingPressEvent<PlatformAction> e)
        {
            switch (e.Action)
            {
                case PlatformAction.Cut:
                    lyricEditorClipboard.Cut();
                    return true;

                case PlatformAction.Copy:
                    lyricEditorClipboard.Copy();
                    return true;

                case PlatformAction.Paste:
                    lyricEditorClipboard.Paste();
                    return true;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<PlatformAction> e)
        {
        }

        public LyricEditorMode Mode
            => bindableMode.Value;

        public void SwitchMode(LyricEditorMode mode)
            => bindableMode.Value = mode;

        public void SwitchSubMode<TSubMode>(TSubMode subMode) where TSubMode : Enum
        {
            var editModeState = getEditModeState<TSubMode>();
            if (editModeState == null)
                throw new ArgumentNullException();

            editModeState.ChangeEditMode(subMode);
        }

        [CanBeNull]
        private IHasEditModeState<TSubMode> getEditModeState<TSubMode>() where TSubMode : Enum
            => InternalChildren.OfType<IHasEditModeState<TSubMode>>().FirstOrDefault();

        public virtual void NavigateToFix(LyricEditorMode mode)
        {
            switch (mode)
            {
                case LyricEditorMode.Texting:
                case LyricEditorMode.Language:
                case LyricEditorMode.EditTimeTag:
                    SwitchMode(mode);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        private class LocalScrollingInfo : IScrollingInfo
        {
            public IBindable<ScrollingDirection> Direction { get; } = new Bindable<ScrollingDirection>(ScrollingDirection.Left);

            public IBindable<double> TimeRange { get; } = new BindableDouble(5000)
            {
                MinValue = 1000,
                MaxValue = 10000
            };

            public IScrollAlgorithm Algorithm { get; } = new SequentialScrollAlgorithm(new List<MultiplierControlPoint>());
        }
    }
}
