﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Bindables;

namespace osu.Game.Rulesets.Karaoke.Edit.Lyrics.States.Modes
{
    public interface IHasSpecialAction<TSpecialAction> where TSpecialAction : Enum
    {
        Bindable<TSpecialAction> BindableSpecialAction { get; }
    }
}
