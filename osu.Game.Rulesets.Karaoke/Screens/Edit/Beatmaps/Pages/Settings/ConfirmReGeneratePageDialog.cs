// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics.Sprites;
using osu.Game.Overlays.Dialog;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Pages.Settings;

public partial class ConfirmReGeneratePageDialog : PopupDialog
{
    public ConfirmReGeneratePageDialog(Action<bool> okAction)
    {
        Icon = FontAwesome.Solid.Trash;
        HeaderText = "Are you sure re-generate the pages? It will delete all the pages in the beatmap.";
        BodyText = "page";
        Buttons = new PopupDialogButton[]
        {
            new PopupDialogOkButton
            {
                Text = @"Yes. Go for it.",
                Action = () => okAction.Invoke(true),
            },
            new PopupDialogCancelButton
            {
                Text = @"No! Abort mission!",
                Action = () => okAction.Invoke(false),
            },
        };
    }
}
