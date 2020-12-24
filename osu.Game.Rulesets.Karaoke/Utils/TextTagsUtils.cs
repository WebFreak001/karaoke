﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Karaoke.Objects.Types;

namespace osu.Game.Rulesets.Karaoke.Utils
{
    public static class TextTagsUtils
    {
        public static T[] Sort<T>(T[] textTags, Sorting sorting = Sorting.Asc) where T : ITextTag
        {
            switch (sorting)
            {
                case Sorting.Asc:
                    return textTags?.OrderBy(x => x.StartIndex).ThenBy(x => x.EndIndex).ToArray();

                case Sorting.Desc:
                    return textTags?.OrderByDescending(x => x.EndIndex).ThenByDescending(x => x.StartIndex).ToArray();

                default:
                    throw new ArgumentOutOfRangeException(nameof(sorting));
            }
        }

        public static T[] FindInvalid<T>(T[] textTags, string lyric, Sorting sorting = Sorting.Asc) where T : ITextTag
        {
            // check is null or empty
            if (textTags == null || textTags.Length == 0)
                return new T[] { };

            // todo : need to make sure is need to sort in here?
            var sortedTextTags = Sort(textTags, sorting);

            var invalidList = new List<T>();

            // check invalid range
            invalidList.AddRange(sortedTextTags.Where(x => x.StartIndex < 0 || x.EndIndex > lyric.Length));

            // check end is less or equal to start index
            invalidList.AddRange(sortedTextTags.Where(x => x.EndIndex <= x.StartIndex));

            // find other is smaller or bigger
            foreach (var textTag in sortedTextTags)
            {
                if (invalidList.Contains(textTag))
                    continue;

                var checkTags = sortedTextTags.Except(new[] { textTag });

                switch (sorting)
                {
                    case Sorting.Asc:
                        // start index within tne target
                        invalidList.AddRange(checkTags.Where(x => x.StartIndex >= textTag.StartIndex && x.StartIndex < textTag.EndIndex));
                        break;

                    case Sorting.Desc:
                        // end index within tne target
                        invalidList.AddRange(checkTags.Where(x => x.EndIndex > textTag.StartIndex && x.EndIndex <= textTag.EndIndex));
                        break;
                }
            }

            return Sort(invalidList.Distinct().ToArray());
        }

        // todo : might think about better way for lyric merging ruby or romaji using.
        public static T Shifting<T>(T textTag, int shifting) where T : ITextTag, new()
        {
            return new T
            {
                StartIndex = textTag.StartIndex + shifting,
                EndIndex = textTag.EndIndex + shifting,
                Text = textTag.Text
            };
        }

        public static T Combine<T>(T textTagA, T textTagB) where T : ITextTag, new()
        {
            var sortinValue = Sort(new[] { textTagA, textTagB });
            return new T
            {
                StartIndex = sortinValue[0].StartIndex,
                EndIndex = sortinValue[1].EndIndex,
                Text = sortinValue[0].Text + sortinValue[1].Text
            };
        }

        public enum Sorting
        {
            /// <summary>
            /// Mark next time tag is error if conflict.
            /// </summary>
            Asc,

            /// <summary>
            /// Mark previous tag is error if conflict.
            /// </summary>
            Desc
        }
    }
}