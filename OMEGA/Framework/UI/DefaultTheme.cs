namespace OMEGA.Framework.UI
{
    public class DefaultTheme : IGuiDrawer
    {
        public Color TextColor { get; set; } = Color.White;

        public Color TextShadowColor { get; set; } = Color.Black;

        public Color PanelBgColor { get; set; } = new(100, 100, 100);

        public Color PanelBorderColor { get; set; } = new(150, 150, 150);

        public int PanelBorderSize = 2;

        public Color WindowBgColor { get; set; } = new(100, 100, 100);

        public int WindowHeaderHeight { get; set; } = 30;

        public Color WindowHeaderColor { get; set; } = new(50, 100, 250);

        public Color WindowCloseButtonColor { get; set; } = new(250, 50, 50);

        public Color WindowCloseButtonHoverColor { get; set; } = new(250, 100, 100);

        public Color WindowCloseButtonActiveColor { get; set; } = new(200, 10, 10);

        public Color ButtonColor { get; set; } = new(150, 150, 150);

        public Color ButtonHoverColor { get; set; } = new(50, 100, 255);

        public Color ButtonActiveColor { get; set; } = new(20, 80, 235);

        public Color CheckboxColor { get; set; } = new(50, 50, 50);

        public Color CheckboxBorderColor { get; set; } = new(100, 100, 100);

        public Color CheckboxIndicatorColor { get; set; } = new(50, 100, 255);

        public Color TabHeaderColor { get; set; } = new(150, 150, 150);

        public Color ListViewBgColor { get; set; } = new(50, 50, 50);

        public Color ListViewRowColor { get; set; } = new(70, 70, 70);

        public Color ListViewRowHoverColor { get; set; } = new(20, 80, 235);

        public Color ListViewRowSelectedColor { get; set; } = new(50, 100, 255);


        private TextureFont _mFont;

        public DefaultTheme(Gui gui)
        {
            _mFont = Blitter.DefaultFont;
            Gui = gui;
        }

        public Gui Gui { get; init; }

        public TextureFont DrawFont
        {
            get => _mFont;
            set => _mFont = value ?? Blitter.DefaultFont;
        }

        public void DrawButton(Button button)
        {
            var color = ButtonColor;
            var borderColor = PanelBorderColor;

            if (button.Hovered)
            {
                color = ButtonHoverColor;
            }

            if (button.Active)
            {
                color = ButtonActiveColor;
            }

            if (button.On)
            {
                color = ButtonHoverColor;
            }

            DrawPanel(button.DrawX, button.DrawY, button.Width, button.Height, ref color, ref borderColor,
                PanelBorderSize);
        }

        public void DrawCheckbox(CheckBox checkBox)
        {
            var color = CheckboxColor;
            var borderColor = CheckboxBorderColor;

            DrawInsetPanel(checkBox.DrawX, checkBox.DrawY, checkBox.Width, checkBox.Height, ref color, ref borderColor,
                2);

            int indicator_w = checkBox.Width - 4;
            int indicator_h = checkBox.Height - 4;

            if (checkBox.On)
            {
                Blitter.SetColor(CheckboxIndicatorColor);
                Blitter.FillRect(checkBox.DrawX + checkBox.Width / 2 - indicator_w / 2,
                    checkBox.DrawY + checkBox.Height / 2 - indicator_h / 2, indicator_w, indicator_h);
            }
        }

        public void DrawLabel(Label label)
        {
            var color = TextColor;
            var shadowColor = TextShadowColor;
            if (Gui.DebugMode)
            {
                Blitter.SetColor(Color.Red);
                Blitter.DrawRect(label.DrawX, label.DrawY, label.Width, label.Height, 1);
            }

            DrawShadowedText(label.Text, label.DrawX, label.DrawY, label.Width, label.Height, label.TextMeasure.W,
                label.TextMeasure.H, ref color, ref shadowColor);
        }

        public void DrawListView(ListView listView)
        {
            var bgColor = ListViewBgColor;
            var borderColor = PanelBorderColor;

            DrawInsetPanel(listView.DrawX, listView.DrawY, listView.Width, listView.Height, ref bgColor,
                ref borderColor, PanelBorderSize);

            Blitter.Clip(listView.DrawX, listView.DrawY, listView.Width, listView.Height);

            for (int i = 0; i < listView.Items.Count; ++i)
            {
                var item = listView.Items[i];

                var row_y = listView.DrawY + i * listView.RowHeight - listView.TranslateY;

                if (i % 2 == 0)
                {
                    Blitter.SetColor(ListViewRowColor);
                    Blitter.FillRect(listView.DrawX, row_y, listView.Width, listView.RowHeight);
                }

                if (i == listView.HoveredIndex)
                {
                    Blitter.SetColor(ListViewRowHoverColor);
                    Blitter.FillRect(listView.DrawX, row_y, listView.Width, listView.RowHeight);
                }

                if (i == listView.SelectedIndex)
                {
                    Blitter.SetColor(ListViewRowSelectedColor);
                    Blitter.FillRect(listView.DrawX, row_y, listView.Width, listView.RowHeight);
                }


                var stringSize = DrawFont.MeasureString(item.Label);
                Blitter.SetColor(TextColor);
                Blitter.DrawText(listView.DrawX + 15, (int) (row_y + listView.RowHeight / 2 - stringSize.Y / 2),
                    item.Label);

                if (!listView.ScrollThumbRect.IsEmpty)
                {
                    var thumb_rect = listView.ScrollThumbRect;
                    Blitter.SetColor(Color.Black);
                    Blitter.FillRect(thumb_rect.X1 + listView.DrawX,
                        thumb_rect.Y1 + listView.DrawY + listView.TranslateY, thumb_rect.Width, thumb_rect.Height);
                }
            }

            Blitter.Clip();
        }

        public void DrawPanel(Panel panel)
        {
            var bgColor = PanelBgColor;
            var borderColor = PanelBorderColor;

            DrawPanel(panel.DrawX, panel.DrawY, panel.Width, panel.Height, ref bgColor, ref borderColor,
                PanelBorderSize);
        }

        public void DrawSelectorSlider(SelectorSlider selectorSlider)
        {
            var options = selectorSlider.Options;
            var draw_x = selectorSlider.DrawX;
            var draw_y = selectorSlider.DrawY;
            var width = selectorSlider.Width;
            var height = selectorSlider.Height;
            var steps = options.Length;
            var thumb_size = selectorSlider.ThumbSize;

            // Draw Background Steps

            for (int i = 0; i < steps; ++i)
            {
                var option_rect = options[i].Rect;
                Blitter.SetColor(Color.Black);
                Blitter.FillRect(
                    draw_x + option_rect.X1,
                    draw_y + option_rect.Y1,
                    option_rect.Width,
                    option_rect.Height
                );
            }

            // Draw Background Bar

            switch (selectorSlider.Orientation)
            {
                case Orientation.Horizontal:
                    Blitter.SetColor(Color.White);
                    Blitter.FillRect(draw_x + thumb_size / 2 + 2, draw_y + thumb_size / 2 - 1,
                        width - thumb_size / 2 - 4, 2);
                    Blitter.SetColor(Color.Black);
                    Blitter.DrawRect(draw_x + thumb_size / 2 + 2, draw_y + thumb_size / 2 - 1,
                        width - thumb_size / 2 - 4, 2, 2);
                    break;
                case Orientation.Vertical:
                    Blitter.SetColor(Color.White);
                    Blitter.FillRect(draw_x + thumb_size / 2 + 2, draw_y + thumb_size / 2 - 1, 2,
                        height - thumb_size / 2 - 4);
                    Blitter.SetColor(Color.Black);
                    Blitter.DrawRect(draw_x + thumb_size / 2 + 2, draw_y + thumb_size / 2 - 1, 2,
                        height - thumb_size / 2 - 4, 2);
                    break;
            }

            var current_option_rect = options[selectorSlider.SelectedIndex].Rect.Inflated(-3);

            // Draw Slider

            Blitter.SetColor(Color.White);
            Blitter.FillRect(
                draw_x + current_option_rect.X1,
                draw_y + current_option_rect.Y1,
                current_option_rect.Width,
                current_option_rect.Height
            );

            Blitter.SetColor(Color.Black);
            Blitter.DrawRect(
                draw_x + current_option_rect.X1,
                draw_y + current_option_rect.Y1,
                current_option_rect.Width,
                current_option_rect.Height
            );
        }

        public void DrawTabs(Tabs tabs)
        {
        }

        public void DrawWindow(Window window)
        {
            var bgColor = WindowBgColor;
            var borderColor = PanelBorderColor;

            DrawPanel(window.DrawX, window.DrawY, window.Width, window.Height, ref bgColor, ref borderColor,
                PanelBorderSize);

            Blitter.SetColor(WindowHeaderColor);
            Blitter.FillRect(window.DrawX, window.DrawY, window.Width, WindowHeaderHeight);
        }

        private static void DrawPanel(int x, int y, int w, int h, ref Color color, ref Color borderColor,
            int borderSize)
        {
            Blitter.SetColor(color);
            Blitter.FillRect(x, y, w, h);
            Blitter.SetColor(borderColor);
            Blitter.DrawRect(x, y, w, h, borderSize);
        }

        private void DrawShadowedText(
            string text,
            int x,
            int y,
            int w,
            int h,
            int textW,
            int textH,
            ref Color color,
            ref Color shadowColor)
        {
            int textDrawX = (x + w / 2 - textW / 2);
            int textDrawY = (y + h / 2 - textH / 2);

            Blitter.SetColor(shadowColor);
            Blitter.DrawText(textDrawX, textDrawY + 1, text);
            Blitter.SetColor(color);
            Blitter.DrawText(textDrawX, textDrawY, text);
            if (Gui.DebugMode)
            {
                Blitter.SetColor(Color.Green);
                Blitter.DrawRect(textDrawX, textDrawY, textW, textH);
            }
        }

        private static void DrawInsetPanel(int x, int y, int w, int h, ref Color color, ref Color borderColor,
            int borderSize)
        {
            Blitter.SetColor(color);
            Blitter.FillRect(x, y, w, h);
            Blitter.SetColor(borderColor);
            Blitter.DrawRect(x, y, w, h, borderSize);
        }
    }
}