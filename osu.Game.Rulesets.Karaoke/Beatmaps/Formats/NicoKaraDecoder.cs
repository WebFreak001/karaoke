﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NicoKaraParser;
using NicoKaraParser.Model;
using NicoKaraParser.Model.Font.Font;
using NicoKaraParser.Model.Font.Shadow;
using NicoKaraParser.Model.Layout;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps.Formats;
using osu.Game.IO;
using osu.Game.Rulesets.Karaoke.Skinning.Components;
using osuTK;
using osuTK.Graphics;
using BrushInfo = NicoKaraParser.Model.Font.Brush.BrushInfo;
using FontInfo = NicoKaraParser.Model.Font.Font.FontInfo;

namespace osu.Game.Rulesets.Karaoke.Beatmaps.Formats
{
    public class NicoKaraDecoder : Decoder<KaraokeSkin>
    {
        public static void Register()
        {
            AddDecoder<KaraokeSkin>("<?xml version=", m => new NicoKaraDecoder());
        }

        protected override void ParseStreamInto(LineBufferedReader stream, KaraokeSkin output)
        {
            Project nicoKaraProject;

            using (TextReader sr = new StringReader(stream.ReadToEnd()))
            {
                nicoKaraProject = new Parser().Deserialize(sr);
            }

            // Clean-up layout
            output.DefinedLayouts = new List<Skinning.Components.KaraokeLayout>();

            foreach (var karaokeLayout in nicoKaraProject.KaraokeLayouts)
            {
                Enum.TryParse(karaokeLayout.SmartHorizon.ToString(), out KaraokeTextSmartHorizon smartHorizon);
                Enum.TryParse(karaokeLayout.RubyAlignment.ToString(), out LyricTextAlignment rubyAlignment);

                output.DefinedLayouts.Add(new Skinning.Components.KaraokeLayout
                {
                    Name = karaokeLayout.Name,
                    Alignment = convertAnchor(karaokeLayout.HorizontalAlignment, karaokeLayout.VerticalAlignment),
                    HorizontalMargin = karaokeLayout.HorizontalMargin,
                    VerticalMargin = karaokeLayout.VerticalMargin,
                    Continuous = karaokeLayout.Continuous,
                    SmartHorizon = smartHorizon,
                    LyricsInterval = karaokeLayout.LyricsInterval,
                    RubyInterval = karaokeLayout.RubyInterval,
                    RubyAlignment = rubyAlignment,
                    RubyMargin = karaokeLayout.RubyMargin
                });
            }

            // Clean-up style
            output.DefinedFonts = new List<KaraokeFont>();

            foreach (var nicoKaraFont in nicoKaraProject.KaraokeFonts)
            {
                output.DefinedFonts.Add(new KaraokeFont
                {
                    Name = nicoKaraFont.Name,
                    UseShadow = nicoKaraFont.UseShadow,
                    ShadowOffset = convertShadowSlide(nicoKaraFont.ShadowSlide),
                    FrontTextBrushInfo = new KaraokeFont.TextBrushInfo
                    {
                        TextBrush = convertBrushInfo(nicoKaraFont.BrushInfos[0]),
                        BorderBrush = convertBrushInfo(nicoKaraFont.BrushInfos[1]),
                        ShadowBrush = convertBrushInfo(nicoKaraFont.BrushInfos[2]),
                    },
                    BackTextBrushInfo = new KaraokeFont.TextBrushInfo
                    {
                        TextBrush = convertBrushInfo(nicoKaraFont.BrushInfos[3]),
                        BorderBrush = convertBrushInfo(nicoKaraFont.BrushInfos[4]),
                        ShadowBrush = convertBrushInfo(nicoKaraFont.BrushInfos[5]),
                    },
                    LyricTextFontInfo = new KaraokeFont.TextFontInfo
                    {
                        LyricTextFontInfo = convertFontInfo(nicoKaraFont.FontInfos[0]),
                        NakaTextFontInfo = convertFontInfo(nicoKaraFont.FontInfos[1]),
                        EnTextFontInfo = convertFontInfo(nicoKaraFont.FontInfos[2]),
                    },
                    RubyTextFontInfo = new KaraokeFont.TextFontInfo
                    {
                        LyricTextFontInfo = convertFontInfo(nicoKaraFont.FontInfos[3]),
                        NakaTextFontInfo = convertFontInfo(nicoKaraFont.FontInfos[4]),
                        EnTextFontInfo = convertFontInfo(nicoKaraFont.FontInfos[5]),
                    },
                    RomajiTextFontInfo = new KaraokeFont.TextFontInfo
                    {
                        // Just copied from ruby setting
                        LyricTextFontInfo = convertFontInfo(nicoKaraFont.FontInfos[3]),
                        NakaTextFontInfo = convertFontInfo(nicoKaraFont.FontInfos[4]),
                        EnTextFontInfo = convertFontInfo(nicoKaraFont.FontInfos[5]),
                    }
                });
            }

            Vector2 convertShadowSlide(ShadowSlide side) => new Vector2(side.X, side.Y);

            Anchor convertAnchor(HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
            {
                Enum.TryParse((1 << ((int)horizontalAlignment + 3)).ToString(), out Anchor horizontalAnchor);
                Enum.TryParse((1 << (int)verticalAlignment).ToString(), out Anchor verticalAnchor);

                return horizontalAnchor | verticalAnchor;
            }

            Skinning.Components.FontInfo convertFontInfo(FontInfo info) =>
                new Skinning.Components.FontInfo
                {
                    FontName = info.FontName,
                    Bold = info.FontStyle == FontStyle.Bold,
                    CharSize = info.CharSize,
                    EdgeSize = info.EdgeSize
                };

            Skinning.Components.BrushInfo convertBrushInfo(BrushInfo info)
            {
                Enum.TryParse(info.Type.ToString(), out BrushType type);

                // Convert BrushGradient
                var brushGradient = info.GradientPositions.Select((t, i) => new Skinning.Components.BrushInfo.BrushGradient { XPosition = t, Color = convertColor(info.GradientColors[i]) }).ToList();

                return new Skinning.Components.BrushInfo
                {
                    Type = type,
                    SolidColor = convertColor(info.SolidColor),
                    BrushGradients = brushGradient
                };

                Color4 convertColor(System.Drawing.Color color) => new Color4(color.R, color.G, color.B, color.A);
            }
        }
    }
}