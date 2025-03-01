﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Overlays;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.Settings
{
    public abstract partial class LabelledObjectFieldSwitchButton<T> : LabelledSwitchButton where T : class
    {
        protected readonly BindableList<T> SelectedItems = new();

        protected new ObjectFieldSwitchButton Component => (ObjectFieldSwitchButton)base.Component;

        private readonly T item;

        protected LabelledObjectFieldSwitchButton(T item)
        {
            this.item = item;

            // apply current value from the field in the item.
            Current.Value = GetFieldValue(item);

            // should change preview text box if selected ruby/romaji changed.
            Component.OnCommit += (sender, edited) =>
            {
                if (!edited)
                    return;

                ApplyValue(item, sender.Value);
            };

            // change style if focus.
            SelectedItems.BindCollectionChanged((_, _) =>
            {
                bool highLight = SelectedItems.Contains(item);
                Component.HighLight = highLight;
            });
        }

        protected void TriggerSelect()
        {
            // not trigger again if already focus.
            if (SelectedItems.Contains(item) && SelectedItems.Count == 1)
                return;

            // trigger selected.
            SelectedItems.Clear();
            SelectedItems.Add(item);
        }

        protected abstract bool GetFieldValue(T item);

        protected abstract void ApplyValue(T item, bool value);

        protected override SwitchButton CreateComponent() => new ObjectFieldSwitchButton
        {
            Selected = selected =>
            {
                if (selected)
                    TriggerSelect();
            }
        };

        protected partial class ObjectFieldSwitchButton : SwitchButton
        {
            public Action<bool> Selected;

            public Action<ObjectFieldSwitchButton, bool> OnCommit;

            protected override bool OnHover(HoverEvent e)
            {
                Selected?.Invoke(true);
                return base.OnHover(e);
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                Selected?.Invoke(false);
                base.OnHoverLost(e);
            }

            protected override void OnUserChange(bool value)
            {
                base.OnUserChange(value);
                OnCommit?.Invoke(this, true);
            }

            private Color4 highLightColour;
            private Color4 enabledColour;

            [BackgroundDependencyLoader(true)]
            private void load(OverlayColourProvider colourProvider, OsuColour colours)
            {
                highLightColour = colours.Yellow;

                // copied from SwitchButton
                enabledColour = colourProvider?.Highlight1 ?? colours.BlueDark;
            }

            public bool Value
            {
                get => Current.Value;
                set => Current.Value = value;
            }

            public bool HighLight
            {
                set
                {
                    if (InternalChild is not CircularContainer circularContainer)
                        throw new ArgumentNullException(nameof(circularContainer));

                    var switchContainer = circularContainer.Children.OfType<Container>().LastOrDefault()?.Child;
                    if (switchContainer == null)
                        throw new ArgumentNullException(nameof(switchContainer));

                    // only change dot colour because border colour should consider off case.
                    switchContainer.Colour = value ? highLightColour : enabledColour;
                }
            }
        }
    }
}
