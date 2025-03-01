// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Diagnostics;
using System.Reflection;
using osu.Game.Rulesets.Karaoke.Skinning.Groups;

namespace osu.Game.Rulesets.Karaoke.IO.Serialization.Converters
{
    public class KaraokeSkinGroupConverter : GenericTypeConverter<IGroup>
    {
        protected override Type GetTypeByName(string name)
        {
            // only get name from font
            var assembly = Assembly.GetExecutingAssembly();
            var type = assembly.GetType($"osu.Game.Rulesets.Karaoke.Skinning.Groups.{name}");
            Debug.Assert(type != null);
            return type;
        }
    }
}
