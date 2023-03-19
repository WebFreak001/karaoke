﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Extensions;
using osu.Framework.Graphics.Shaders;
using osu.Game.Rulesets.Karaoke.Skinning.Elements;

namespace osu.Game.Rulesets.Karaoke.Skinning.Tools
{
    // it's the temp logic to collect logic.
    public static class SkinConverterTool
    {
        public static ICustomizedShader[] ConvertLeftSideShader(ShaderManager? shaderManager, LyricStyle lyricStyle)
        {
            if (shaderManager == null)
                return Array.Empty<ICustomizedShader>();

            var shaders = lyricStyle.LeftLyricTextShaders.ToArray();
            attachShaders(shaderManager, shaders);

            return shaders;
        }

        public static ICustomizedShader[] ConvertRightSideShader(ShaderManager? shaderManager, LyricStyle lyricStyle)
        {
            if (shaderManager == null)
                return Array.Empty<ICustomizedShader>();

            var shaders = lyricStyle.RightLyricTextShaders.ToArray();
            attachShaders(shaderManager, shaders);

            return shaders;
        }

        private static void attachShaders(ShaderManager shaderManager, IEnumerable<ICustomizedShader> shaders)
        {
            // TODO: StepShader should not inherit from ICustomizedShader
            foreach (var shader in shaders)
            {
                switch (shader)
                {
                    case StepShader stepShader:
                        attachShaders(shaderManager, stepShader.StepShaders.ToArray());
                        break;

                    case null:
                        throw new InvalidCastException("shader cannot be null.");

                    default:
                        shaderManager.AttachShader(shader);
                        break;
                }
            }
        }
    }
}
