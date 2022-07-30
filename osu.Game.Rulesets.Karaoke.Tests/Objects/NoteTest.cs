// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Game.Rulesets.Karaoke.Objects;

namespace osu.Game.Rulesets.Karaoke.Tests.Objects
{
    public class NoteTest
    {
        [TestCase]
        public void TestClone()
        {
            var note = new Note
            {
                Text = "lyric",
            };

            var clonedNote = note.DeepClone();

            Assert.AreNotSame(clonedNote.TextBindable, note.TextBindable);
            Assert.AreEqual(clonedNote.Text, note.Text);

            Assert.AreNotSame(clonedNote.RubyTextBindable, note.RubyTextBindable);
            Assert.AreEqual(clonedNote.RubyText, note.RubyText);

            Assert.AreNotSame(clonedNote.DisplayBindable, note.DisplayBindable);
            Assert.AreEqual(clonedNote.Display, note.Display);

            Assert.AreNotSame(clonedNote.ToneBindable, note.ToneBindable);
            Assert.AreEqual(clonedNote.Tone, note.Tone);

            Assert.AreEqual(clonedNote.Duration, note.Duration);

            Assert.AreEqual(clonedNote.StartIndex, note.StartIndex);

            Assert.AreEqual(clonedNote.EndIndex, note.EndIndex);

            Assert.AreSame(clonedNote.ParentLyric, note.ParentLyric);
        }
    }
}
