﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Karaoke.Edit.ImportLyric.AssignLanguage;
using osu.Game.Rulesets.Karaoke.Edit.ImportLyric.DragFile;
using osu.Game.Rulesets.Karaoke.Edit.ImportLyric.GenerateRuby;
using osu.Game.Rulesets.Karaoke.Edit.ImportLyric.GenerateTimeTag;
using osu.Game.Rulesets.Karaoke.Edit.ImportLyric.Success;
using osu.Game.Screens;

namespace osu.Game.Rulesets.Karaoke.Edit.ImportLyric
{
    public class ImportLyricSubScreenStack : OsuScreenStack
    {
        public void Push(ImportLyricStep step)
        {
            switch (step)
            {
                case ImportLyricStep.ImportLyric:
                    Push(new DragFileSubScreen());
                    return;

                case ImportLyricStep.AssignLanguage:
                    Push(new AssignLanguageSubScreen());
                    return;

                case ImportLyricStep.GenerateRuby:
                    Push(new GenerateRubySubScreen());
                    return;

                case ImportLyricStep.GenerateTimeTag:
                    Push(new GenerateTimeTagSubScreen());
                    return;

                case ImportLyricStep.Success:
                    Push(new SuccessSubScreen());
                    return;

                default:
                    throw new ScreenNotCurrentException("Screen is not in the lyric import step.");
            }
        }
    }
}
