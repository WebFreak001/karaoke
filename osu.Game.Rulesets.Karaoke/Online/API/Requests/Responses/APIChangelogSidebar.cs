﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Karaoke.Online.API.Requests.Responses
{
    public class APIChangelogSidebar
    {
        public IEnumerable<APIChangelogBuild> Changelogs { get; set; } = Array.Empty<APIChangelogBuild>();

        public int[] Years { get; set; } = Array.Empty<int>();
    }
}
