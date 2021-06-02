using System;
using System.Collections.Generic;
using System.Text;
using static Bgfx.Bgfx;

namespace OMEGA
{
    public class Canvas2D
    {
        public CanvasView DefaultView { get; private set; }

        public CanvasView CurrentView { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public (int Width, int Height) Size => (Width, Height);

        public CanvasStretchMode StretchMode { get; set; } = CanvasStretchMode.Stretch;

        public int MaxDrawCalls { get; }

        internal bool NeedsResetDisplay { get; set; }

        private readonly SpriteBatcher _spriteBatcher;
        private SpriteSortMode _spriteSortMode;

        private bool _insideBeginBlock;

        private int _drawCalls;
       
        
        private ShaderProgram _baseShader;
        private Texture2D _primitiveTexture;
        private Texture2D _currentTexture;
        private TextureFont _defaultFont;
        private TextureFont _currentFont;
        private readonly List<CanvasView> _additionalViews = new();
        private static readonly Color DefaultClearColor = new(0, 75, 255);
        private static ushort _staticViewId;

        internal Canvas2D(int width, int height, int maxDrawCalls)
        {
            Width = width;
            Height = height;
            MaxDrawCalls = maxDrawCalls;

            InitDefaultResources();

            _spriteBatcher = new SpriteBatcher(_currentShader);

            InitDefaultView();

            ApplyViewProperties(DefaultView);

        }


        public CanvasView CreateView()
        {
            return CreateView(DefaultClearColor);
        }

        public CanvasView CreateView(Color clearColor)
        {
            var view = new CanvasView(1f, 1f)
            {
                ViewId = _staticViewId++,
                ClearColor = clearColor,
            };

            if (view.ViewId > 0)
            {
                _additionalViews.Add(view);
            }

            return view;
        }

        public void Begin(CanvasView view = null, SpriteSortMode sortMode = SpriteSortMode.Deferred)
        {
            if (_insideBeginBlock)
            {
                throw new Exception("Cannot nest Canvas Begin Calls");
            }

            _insideBeginBlock = true;

            CurrentView = view ?? DefaultView;

            if (!CurrentView.Applied)
            {
                ApplyViewProperties(CurrentView);
            }

            Rect viewport = GetAbsoluteViewport(CurrentView);

            GraphicsContext.SetScissor(viewport.X1, viewport.Y1, viewport.Width, viewport.Height);

            _spriteSortMode = sortMode;

            if (sortMode == SpriteSortMode.Immediate)
            {
                
            }
        }

        private void ApplyDrawEffect()
        {
            GraphicsContext.SetState(_renderState);
        }

        public void End()
        {
            if (!_insideBeginBlock)
            {
                return;
            }

            _spriteBatcher.DrawBatch();
            
            ResetRenderState();
            _drawCalls = 0;
            _insideBeginBlock = false;
        }

        public void SetFont(TextureFont font)
        {
            _currentFont = font ?? _defaultFont;
        }

        public void DrawTriangle(VertexPositionColorTexture v1, VertexPositionColorTexture v2, VertexPositionColorTexture v3, Texture2D texture = null)
        {
            if (!_insideBeginBlock)
            {
                throw new Exception("Cannot call drawing methods in Canvas outside of Begin End Block");
            }

            texture ??= _primitiveTexture;

            if (_currentTexture != texture)
            {
                SetTexture(0, texture);
            }

            if (!_vertexStream.PushTriangle(v1, v2, v3))
            {
                RenderBatch();
                _vertexStream.PushTriangle(v1, v2, v3);
            }
        }

        public void DrawQuad(in Quad quad, Texture2D texture = null)
        {
            if (!_insideBeginBlock)
            {
                throw new Exception("Cannot call drawing methods in Canvas outside of Begin End Block");
            }

            texture ??= _primitiveTexture;

            if (_currentTexture != texture)
            {
                SetTexture(0, texture);
            }

            if (!_vertexStream.PushQuad(in quad))
            {
                RenderBatch();
                _vertexStream.PushQuad(in quad);
            }
        }

        public unsafe void DrawString(string text, Vec2 position, Color color, float scale = 1.0f)
        {
            if (!_insideBeginBlock)
            {
                throw new Exception("Cannot call drawing methods in Canvas outside of Begin End Block");
            }

            var font = _currentFont;

            if (_currentTexture != font.Texture)
            {
                SetTexture(0, font.Texture);
            }

            var offset = Vec2.Zero;
            var firstGlyphOfLine = true;

            fixed (TextureFont.Glyph* pGlyphs = font.Glyphs)
            {
                for (int i = 0; i < text.Length; ++i)
                {
                    var c = text[i];

                    switch (c)
                    {
                        case '\r':
                            continue;
                        case '\n':
                            offset.X = 0;
                            offset.Y = font.LineSpacing;
                            firstGlyphOfLine = true;
                            continue;
                    }

                    var currentGlyphIndex = font.GetGlyphIndexOrDefault(c);

                    var pCurrentGlyph = pGlyphs + currentGlyphIndex;

                    if (firstGlyphOfLine)
                    {
                        offset.X = Calc.Max(pCurrentGlyph->LeftSideBearing, 0);
                        firstGlyphOfLine = false;
                    }
                    else
                    {
                        offset.X += font.Spacing + pCurrentGlyph->LeftSideBearing;
                    }

                    var p = offset;

                    Transform transform = Transform.Identity;

                    transform.M11 = scale;
                    transform.M22 = scale;
                    transform.M41 = position.X;
                    transform.M42 = position.Y;

                    p.X += pCurrentGlyph->Cropping.X1;
                    p.Y += pCurrentGlyph->Cropping.Y1;
                    p += position;

                    Transform.TransformPoint(ref p, ref transform, out p);

                    if (!pCurrentGlyph->TextureRect.IsEmpty)
                    {
                        var quad = new Quad(font.Texture, pCurrentGlyph->TextureRect, RectF.FromBox(p.X, p.Y, pCurrentGlyph->TextureRect.Width * scale, pCurrentGlyph->TextureRect.Height * scale));
                        quad.SetColor(color);

                        if (!_vertexStream.PushQuad(in quad))
                        {
                            RenderBatch();
                            _vertexStream.PushQuad(in quad);
                        }
                    }

                    offset.X += pCurrentGlyph->Width + pCurrentGlyph->RightSideBearing;
                }
            }
        }

        public unsafe void DrawString(StringBuilder text, Vec2 position, Color color, Vec2 scale)
        {
            if (!_insideBeginBlock)
            {
                throw new Exception("Cannot call drawing methods in Canvas outside of Begin End Block");
            }

            var font = _currentFont;

            if (_currentTexture != font.Texture)
            {
                SetTexture(0, font.Texture);
            }

            var offset = Vec2.Zero;
            var firstGlyphOfLine = true;

            fixed (TextureFont.Glyph* pGlyphs = font.Glyphs)
            {
                for (int i = 0; i < text.Length; ++i)
                {
                    var c = text[i];

                    switch (c)
                    {
                        case '\r':
                            continue;
                        case '\n':
                            offset.X = 0;
                            offset.Y = font.LineSpacing;
                            firstGlyphOfLine = true;
                            continue;
                    }

                    var currentGlyphIndex = font.GetGlyphIndexOrDefault(c);

                    var pCurrentGlyph = pGlyphs + currentGlyphIndex;

                    if (firstGlyphOfLine)
                    {
                        offset.X = Calc.Max(pCurrentGlyph->LeftSideBearing, 0);
                        firstGlyphOfLine = false;
                    }
                    else
                    {
                        offset.X += font.Spacing + pCurrentGlyph->LeftSideBearing;
                    }

                    var p = offset;

                    Transform transform = Transform.Identity;

                    transform.M11 = scale.X;
                    transform.M22 = scale.Y;
                    transform.M41 = position.X;
                    transform.M42 = position.Y;

                    p.X += pCurrentGlyph->Cropping.X1;
                    p.Y += pCurrentGlyph->Cropping.Y1;
                    p += position;


                    Transform.TransformPoint(ref p, ref transform, out p);

                    if (!pCurrentGlyph->TextureRect.IsEmpty)
                    {
                        var quad = new Quad(font.Texture, pCurrentGlyph->TextureRect, RectF.FromBox(p.X, p.Y, pCurrentGlyph->TextureRect.Width * scale.X, pCurrentGlyph->TextureRect.Height * scale.Y));

                        if (!_vertexStream.PushQuad(in quad))
                        {
                            RenderBatch();
                            _vertexStream.PushQuad(in quad);
                        }
                    }

                    offset.X += pCurrentGlyph->Width + pCurrentGlyph->RightSideBearing;
                }
            }
        }



        public void SetTexture(int slot, Texture2D texture)
        {
            if (texture != _currentTexture)
            {
                Flush();
                _currentTexture = texture;
                _currentShader.SetTexture(slot, texture);
            }
        }

        internal Point ConvertToLocalCoord(int x, int y)
        {
            var viewport = GetAbsoluteViewport(DefaultView);
            return new Point(x - viewport.X1, y - viewport.Y1);
        }

        internal void HandleDisplayChange()
        {
            Console.WriteLine("Handle Display Change");

            var display_size = Platform.GetDisplaySize();

            Console.WriteLine($"Display Size:{display_size}");

            GraphicsContext.Reset(display_size.Width, display_size.Height, ResetFlags.Vsync);

            if (StretchMode == CanvasStretchMode.Resize)
            {
                Width = display_size.Width;
                Height = display_size.Height;
            }
            else
            {
                Width = Engine.RunningGame.GameInfo.ResolutionWidth;
                Height = Engine.RunningGame.GameInfo.ResolutionHeight;
            }

            ApplyCanvasStretchModeForBaseView();

            ApplyViewProperties(DefaultView);

            if (_additionalViews.Count > 0)
            {
                foreach (var view in _additionalViews)
                {
                    view.NeedsUpdateTransform = true;
                    ApplyViewProperties(view);
                }
            }

            NeedsResetDisplay = false;
        }

        private void Flush()
        {
            RenderBatch();
        }

        private void ApplyCanvasStretchModeForBaseView()
        {
            var (canvasWidth, canvasHeight) = Size;
            var (displayWidth, displayHeight) = Platform.GetDisplaySize();

            switch (StretchMode)
            {
                case CanvasStretchMode.LetterBox:
                {
                    float display_ratio = (float)displayWidth / displayHeight;
                    float view_ratio = (float)canvasWidth / canvasHeight;

                    float new_view_port_w = 1;
                    float new_view_port_h = 1;
                    float new_view_port_x = 0;
                    float new_view_port_y = 0;

                    bool horizontal_spacing = !(display_ratio < view_ratio);

                    if (horizontal_spacing)
                    {
                        new_view_port_w = view_ratio / display_ratio;
                        new_view_port_x = (1 - new_view_port_w) / 2f;
                    }
                    else
                    {
                        new_view_port_h = display_ratio / view_ratio;
                        new_view_port_y = (1 - new_view_port_h) / 2f;
                    }

                    DefaultView.Viewport = RectF.FromBox(new_view_port_x, new_view_port_y, new_view_port_w, new_view_port_h);

                }
                    break;
                case CanvasStretchMode.Stretch:
                case CanvasStretchMode.Resize:
                    DefaultView.NeedsUpdateTransform = true;
                    break;
            }

            DefaultView.Applied = false;
        }

        private void InitDefaultView()
        {
            DefaultView = CreateView(DefaultClearColor);

            CurrentView = DefaultView;
        }

        private void ApplyViewProperties(CanvasView view) 
        { 
            
            Console.WriteLine("Apply View");

            Rect viewport = GetAbsoluteViewport(view);

            Console.WriteLine($"Viewport: {viewport}");

            GraphicsContext.SetViewRect(view.ViewId, viewport.X1, viewport.Y1, viewport.Width, viewport.Height);
            GraphicsContext.SetViewClear(view.ViewId, ClearFlags.Color | ClearFlags.Depth, view.ClearColor.Rgba);

            GraphicsContext.Touch(view.ViewId);

            var projection = view.GetTransform(Width, Height);

            GraphicsContext.SetProjection(view.ViewId, ref projection.M11);

            if (view.RenderTarget != null)
            {
                GraphicsContext.SetFrameBuffer(view.ViewId, view.RenderTarget.Handle);
            }

            view.Applied = true;
        }

        public Rect GetAbsoluteViewport(CanvasView view)
        {
            var screen_size = Platform.GetDisplaySize();

            var view_viewport = view.Viewport;
            var base_view_viewport = DefaultView.Viewport;

            var default_view_viewport = Rect.FromBox
            (
                (int)(0.5f + screen_size.Width * base_view_viewport.X1),
                (int)(0.5f + screen_size.Height * base_view_viewport.Y1),
                (int)(0.5f + screen_size.Width * base_view_viewport.Width),
                (int)(0.5f + screen_size.Height * base_view_viewport.Height)
            );

            if (view.ViewId == 0)
            {
                return default_view_viewport;
            }
            else
            {
                //return Rect.FromBox
                //(
                //    (int)(0.5f + default_view_viewport.Width * view_viewport.X1 + default_view_viewport.X1),
                //    (int)(0.5f + default_view_viewport.Height * view_viewport.Y1 + default_view_viewport.Y1),
                //    (int)(0.5f + default_view_viewport.Width * view_viewport.Width),
                //    (int)(0.5f + default_view_viewport.Height * view_viewport.Height)
                //);

                return Rect.FromBox
                (
                    (int)(0.5f + screen_size.Width * view_viewport.X1),
                    (int)(0.5f + screen_size.Height * view_viewport.Y1),
                    (int)(0.5f + screen_size.Width * view_viewport.Width),
                    (int)(0.5f + screen_size.Height * view_viewport.Height)
                );
            }
        }

        private void InitDefaultResources()
        {
            _baseShader = Engine.Content.Get<ShaderProgram>("canvas_shader");

            _currentShader = _baseShader;

            _primitiveTexture = Texture2D.Create(1, 1, Color.White);

            _currentFont = _defaultFont = Engine.Content.Get<TextureFont>("default_font");
        }

        private void UpdateRenderState()
        {
            _renderState =
                StateFlags.WriteRgb |
                StateFlags.WriteA |
                StateFlags.WriteZ |
                _blendState |
                _depthState |
                _cullState;
        }

        private void ResetRenderState()
        {
            BlendMode = BlendMode.Alpha;
            CullMode = CullMode.None;
            DepthTest = DepthTest.LessOrEqual;
        }
    }
}
