﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Edit.Checks.Components;
using osu.Game.Rulesets.Karaoke.Objects;

namespace osu.Game.Rulesets.Karaoke.Edit.Checks.Issues
{
    public class NoteIssue : Issue
    {
        public Note Note;

        public NoteIssue(Note note, IssueTemplate template, params object[] args)
            : base(note, template, args)
        {
            Note = note;
        }
    }
}
