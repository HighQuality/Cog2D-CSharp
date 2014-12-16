// ---- AngelCode BmFont XML serializer ----------------------
// ---- By DeadlyDan @ deadlydan@gmail.com -------------------
// ---- There's no license restrictions, use as you will. ----
// ---- Credits to http://www.angelcode.com/ -----------------

using Cog.Modules.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;

namespace Cog.Modules.Renderer
{
    public class BitmapFont : Resource, IDisposable
    {
        internal class FontRenderer
        {
            public FontRenderer(FontFile fontFile, Texture fontTexture)
            {
                FontFile = fontFile;
                Texture = fontTexture;
                CharacterMap = new Dictionary<char, FontChar>();
                
                foreach (var fontCharacter in FontFile.Chars)
                {
                    char c = (char)fontCharacter.ID;
                    CharacterMap.Add(c, fontCharacter);
                }
            }

            internal Dictionary<char, FontChar> CharacterMap;
            internal FontFile FontFile;
            internal Texture Texture;

            public void DrawString(IRenderTarget renderTarget, string text, Color color, Vector2 position, HAlign horizontalAlignment, VAlign verticalAlignment)
            {
                if (horizontalAlignment != HAlign.Left)
                {
                    var lines = text.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        var size = MeassureString(lines[i]);
                        if (horizontalAlignment == HAlign.Right)
                            DrawString(renderTarget, text, color, position - new Vector2(size.X, 0f), HAlign.Left, verticalAlignment);
                        else
                            DrawString(renderTarget, text, color, position - new Vector2(size.X / 2f, 0f), HAlign.Left, verticalAlignment);
                        position.Y += size.Y;
                    }
                }
                else
                {
                    if (verticalAlignment != VAlign.Top)
                    {
                        var verticalOffset = MeassureString(text).Y;
                        if (verticalAlignment == VAlign.Center)
                            verticalOffset /= 2f;
                        position.Y -= verticalOffset;
                    }
                    int dx = (int)position.X;
                    int dy = (int)position.Y;

                    foreach (char c in text)
                    {
                        FontChar fc;
                        if (c == '\n')
                        {
                            dx = (int)position.X;
                            dy += FontFile.Common.LineHeight;
                        }
                        else if (CharacterMap.TryGetValue(c, out fc))
                        {
                            renderTarget.RenderTexture(Texture, new Vector2(dx + fc.XOffset, dy + fc.YOffset), color, Vector2.One, Vector2.Zero, 0f, new Rectangle(fc.X, fc.Y, fc.Width, fc.Height));

                            dx += fc.XAdvance;
                        }
                    }
                }
            }

            public void DrawString(IRenderTarget renderTarget, string text, Color color, Rectangle rect)
            {
                int dx = (int)rect.Left;
                int dy = (int)rect.Top;
                int wordWidth = 0;
                Action drawWord = null;

                foreach (char c in text)
                {
                    FontChar fc;
                    if (c == '\n')
                    {
                        dx = (int)rect.Left;
                        dy += FontFile.Common.LineHeight;
                    }
                    else if (CharacterMap.TryGetValue(c, out fc))
                    {
                        if (dx + fc.XAdvance > rect.Right)
                        {
                            dx = (int)rect.Left;
                            dy += FontFile.Common.LineHeight;
                            if (drawWord != null)
                            {
                                drawWord();
                                drawWord = null;
                            }
                        }
                        else if (char.IsWhiteSpace(c))
                        {
                            if (drawWord != null)
                            {
                                drawWord();
                                drawWord = null;
                            }

                            renderTarget.RenderTexture(Texture, new Vector2(dx + fc.XOffset, dy + fc.YOffset), color, Vector2.One, Vector2.Zero, 0f, new Rectangle(fc.X, fc.Y, fc.Width, fc.Height));
                            dx += fc.XAdvance;
                        }
                        else
                        {
                            int xx = dx;
                            int yy = dy;
                            drawWord += () =>
                            {
                                renderTarget.RenderTexture(Texture, new Vector2(xx + fc.XOffset, yy + fc.YOffset), color, Vector2.One, Vector2.Zero, 0f, new Rectangle(fc.X, fc.Y, fc.Width, fc.Height));
                            };
                            dx += fc.XAdvance;
                            wordWidth += fc.XAdvance;
                        }
                    }
                }

                if (drawWord != null)
                {
                    drawWord();
                }
            }

            public Vector2 MeassureString(string text)
            {
                int maxWidth = 0;
                int maxHeight = FontFile.Common.LineHeight;

                int lineWidth = 0;

                foreach (char c in text)
                {
                    FontChar fc;

                    if (c == '\n')
                    {
                        maxHeight += FontFile.Common.LineHeight;
                        lineWidth = 0;
                    }
                    else if (CharacterMap.TryGetValue(c, out fc))
                    {
                        lineWidth += fc.XAdvance;
                        if (lineWidth > maxWidth)
                            maxWidth = lineWidth;
                    }
                }

                return new Vector2(maxWidth, maxHeight);
            }
        }

        internal FontRenderer Renderer;
        private Texture texture;

        public BitmapFont(byte[] data, Func<string, Texture> loadTexture)
        {
            FontFile fontFile = FontLoader.Load(data);
            texture = loadTexture(fontFile.Pages[0].File);
            Renderer = new FontRenderer(fontFile, texture);
        }

        public void DrawString(IRenderTarget target, string text, Color color, Vector2 position, HAlign horizontalAlignment, VAlign verticalAlignment)
        {
            Renderer.DrawString(target, text, color, position, horizontalAlignment, verticalAlignment);
        }

        public void DrawString(IRenderTarget target, string text, Color color, Rectangle rect)
        {
            Renderer.DrawString(target, text, color, rect);
        }

        public Vector2 MeassureString(string str)
        {
            return Renderer.MeassureString(str);
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                texture.Dispose();
            }
        }

        ~BitmapFont()
        {
            Dispose(false);
        }
    }

    [Serializable]
    [XmlRoot("font")]
    public class FontFile
    {
        [XmlElement("info")]
        public FontInfo Info
        {
            get;
            set;
        }

        [XmlElement("common")]
        public FontCommon Common
        {
            get;
            set;
        }

        [XmlArray("pages")]
        [XmlArrayItem("page")]
        public List<FontPage> Pages
        {
            get;
            set;
        }

        [XmlArray("chars")]
        [XmlArrayItem("char")]
        public List<FontChar> Chars
        {
            get;
            set;
        }

        [XmlArray("kernings")]
        [XmlArrayItem("kerning")]
        public List<FontKerning> Kernings
        {
            get;
            set;
        }
    }

    [Serializable]
    public class FontInfo
    {
        [XmlAttribute("face")]
        public String Face
        {
            get;
            set;
        }

        [XmlAttribute("size")]
        public Int32 Size
        {
            get;
            set;
        }

        [XmlAttribute("bold")]
        public Int32 Bold
        {
            get;
            set;
        }

        [XmlAttribute("italic")]
        public Int32 Italic
        {
            get;
            set;
        }

        [XmlAttribute("charset")]
        public String CharSet
        {
            get;
            set;
        }

        [XmlAttribute("unicode")]
        public Int32 Unicode
        {
            get;
            set;
        }

        [XmlAttribute("stretchH")]
        public Int32 StretchHeight
        {
            get;
            set;
        }

        [XmlAttribute("smooth")]
        public Int32 Smooth
        {
            get;
            set;
        }

        [XmlAttribute("aa")]
        public Int32 SuperSampling
        {
            get;
            set;
        }

        private Rectangle _Padding;
        [XmlAttribute("padding")]
        public String Padding
        {
            get
            {
                return _Padding.Left + "," + _Padding.Top + "," + _Padding.Width + "," + _Padding.Height;
            }
            set
            {
                String[] padding = value.Split(',');
                _Padding = new Rectangle(Convert.ToInt32(padding[0]), Convert.ToInt32(padding[1]), Convert.ToInt32(padding[2]), Convert.ToInt32(padding[3]));
            }
        }

        private Point _Spacing;
        [XmlAttribute("spacing")]
        public String Spacing
        {
            get
            {
                return _Spacing.X + "," + _Spacing.Y;
            }
            set
            {
                String[] spacing = value.Split(',');
                _Spacing = new Point(Convert.ToInt32(spacing[0]), Convert.ToInt32(spacing[1]));
            }
        }

        [XmlAttribute("outline")]
        public Int32 OutLine
        {
            get;
            set;
        }
    }

    [Serializable]
    public class FontCommon
    {
        [XmlAttribute("lineHeight")]
        public Int32 LineHeight
        {
            get;
            set;
        }

        [XmlAttribute("base")]
        public Int32 Base
        {
            get;
            set;
        }

        [XmlAttribute("scaleW")]
        public Int32 ScaleW
        {
            get;
            set;
        }

        [XmlAttribute("scaleH")]
        public Int32 ScaleH
        {
            get;
            set;
        }

        [XmlAttribute("pages")]
        public Int32 Pages
        {
            get;
            set;
        }

        [XmlAttribute("packed")]
        public Int32 Packed
        {
            get;
            set;
        }

        [XmlAttribute("alphaChnl")]
        public Int32 AlphaChannel
        {
            get;
            set;
        }

        [XmlAttribute("redChnl")]
        public Int32 RedChannel
        {
            get;
            set;
        }

        [XmlAttribute("greenChnl")]
        public Int32 GreenChannel
        {
            get;
            set;
        }

        [XmlAttribute("blueChnl")]
        public Int32 BlueChannel
        {
            get;
            set;
        }
    }

    [Serializable]
    public class FontPage
    {
        [XmlAttribute("id")]
        public Int32 ID
        {
            get;
            set;
        }

        [XmlAttribute("file")]
        public String File
        {
            get;
            set;
        }
    }

    [Serializable]
    public class FontChar
    {
        [XmlAttribute("id")]
        public Int32 ID
        {
            get;
            set;
        }

        [XmlAttribute("x")]
        public Int32 X
        {
            get;
            set;
        }

        [XmlAttribute("y")]
        public Int32 Y
        {
            get;
            set;
        }

        [XmlAttribute("width")]
        public Int32 Width
        {
            get;
            set;
        }

        [XmlAttribute("height")]
        public Int32 Height
        {
            get;
            set;
        }

        [XmlAttribute("xoffset")]
        public Int32 XOffset
        {
            get;
            set;
        }

        [XmlAttribute("yoffset")]
        public Int32 YOffset
        {
            get;
            set;
        }

        [XmlAttribute("xadvance")]
        public Int32 XAdvance
        {
            get;
            set;
        }

        [XmlAttribute("page")]
        public Int32 Page
        {
            get;
            set;
        }

        [XmlAttribute("chnl")]
        public Int32 Channel
        {
            get;
            set;
        }
    }

    [Serializable]
    public class FontKerning
    {
        [XmlAttribute("first")]
        public Int32 First
        {
            get;
            set;
        }

        [XmlAttribute("second")]
        public Int32 Second
        {
            get;
            set;
        }

        [XmlAttribute("amount")]
        public Int32 Amount
        {
            get;
            set;
        }
    }

    public class FontLoader
    {
        public static FontFile Load(byte[] data)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(FontFile));
            TextReader textReader = new StreamReader(new MemoryStream(data));
            FontFile file = (FontFile)deserializer.Deserialize(textReader);
            textReader.Close();
            return file;
        }
    }
}