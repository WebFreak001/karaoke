// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Karaoke.Objects;

namespace osu.Game.Rulesets.Karaoke.Edit.Generator.Types
{
    /// <summary>
    /// Base interface of the auto-generator.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILyricPropertyGenerator<out T>
    {
        /// <summary>
        /// Determined if generate property from lyric is supported.
        /// </summary>
        /// <param name="lyric"></param>
        /// <returns></returns>
        bool CanGenerate(Lyric lyric);

        /// <summary>
        /// Generate the property from the lyric.
        /// </summary>
        /// <param name="lyric"></param>
        /// <returns></returns>
        T Generate(Lyric lyric);
    }
}
