using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace JinxBot.Views
{
    [Serializable]
    internal class ToolTipPropertySet 
    {
        private Color m_titleColor = Color.Black;
        private Color m_backgroundColor, m_foregroundColor;
        private string m_titleText;
        private Image m_img;
        private string m_text;

        private const int MAX_WIDTH = 400;
        private readonly static Size CONSTRAINED_SIZE = new Size(MAX_WIDTH, 2000);

        public ToolTipPropertySet(string title, string text)
        {
            m_titleText = title;
            m_text = text;
        }

        public ToolTipPropertySet(string title, string text, Color titleColor)
            : this(title, text)
        {
            m_titleColor = titleColor;
        }

        public ToolTipPropertySet(string title, string text, Color titleColor, Image tipImg)
            : this(title, text, titleColor)
        {
            m_img = tipImg;
        }

        public ToolTipPropertySet(string title, string text, Color titleColor, Image tipImg, Color foreColor, Color backColor)
            : this(title, text, titleColor, tipImg)
        {
            m_foregroundColor = foreColor;
            m_backgroundColor = backColor;
        }

        public ToolTipPropertySet(string title, string text, Image tipImg)
            : this(title, text)
        {
            m_img = tipImg;
        }

        public string Text { get { return m_text; } set { m_text = value; } }
        public Image Image { get { return m_img; } set { m_img = value; } }
        public string Title { get { return m_titleText; } set { m_titleText = value; } }
        public Color TitleColor { get { return m_titleColor; } set { m_titleColor = value; } }
        public Color ForegroundColor { get { return m_foregroundColor; } set { m_foregroundColor = value; } }
        public Color BackgroundColor { get { return m_backgroundColor; } set { m_backgroundColor = value; } }

        public virtual Size MeasureArrangement(Font font)
        {
            Font titleFont = new Font(font.FontFamily, font.SizeInPoints + 4.0f, FontStyle.Bold, GraphicsUnit.Point);
            Size titleSize = TextRenderer.MeasureText(Title, titleFont);
            Size size = TextRenderer.MeasureText(Text, font, CONSTRAINED_SIZE, TextFormatFlags.WordBreak);
            Image img = Image;
            if (img != null)
                size.Width += 16 + img.Width;

            if (titleSize.Width > size.Width)
                size.Width = titleSize.Width + 16 + img.Width;
            if (size.Width > MAX_WIDTH)
                size.Width = MAX_WIDTH;
            size.Height += 16 + titleSize.Height;

            size.Width += 48;

            return size;
        }

        public virtual void Draw(Graphics graphics, Rectangle window, Brush backgroundBrush,
            Brush defaultForegroundBrush, Font font)
        {
            graphics.FillRectangle(backgroundBrush, window);
            //e.DrawBorder();
            
            if (backgroundBrush is LinearGradientBrush)
            {
                LinearGradientBrush brush = backgroundBrush as LinearGradientBrush;
                graphics.DrawRectangle(new Pen(new LinearGradientBrush(window, brush.LinearColors[1], brush.LinearColors[0], LinearGradientMode.ForwardDiagonal)),
                    window);
            }
            else
            {
                graphics.DrawRectangle(new Pen(defaultForegroundBrush), window);
            }

            if (m_img != null)
                graphics.DrawImage(m_img, new Point(2, 2));

            // title
            Font titleFont = new Font(font.FontFamily, font.SizeInPoints + 2.0f, FontStyle.Bold, GraphicsUnit.Point);
            graphics.DrawString(Title, titleFont, new SolidBrush(TitleColor),
                (8.0f + (float)m_img.Width), 2.0f);
            Size size = TextRenderer.MeasureText(Title, titleFont);
            // text
            graphics.DrawString(Text, font, defaultForegroundBrush, (8.0f + (float)m_img.Width),
                (8.0f + (float)size.Height));
        }

        private Color InvertColor(Color color)
        {
            return Color.FromArgb(color.R ^ 0xff, color.G ^ 0xff, color.B ^ 0xff);
        }

        public void Draw(Graphics graphics, Rectangle window, Color background, Color defaultForeground, Font font)
        {
            Draw(graphics, window, new SolidBrush(background), new SolidBrush(defaultForeground), font);
        }

        public ToolTipPropertySet(ToolTipPropertySet source)
        {
            this.m_img = new Bitmap(source.Image);
            this.m_text = source.Text;
            this.m_titleColor = source.TitleColor;
            this.m_titleText = source.Title;
        }

        public void Draw(Graphics graphics, Rectangle rectangle, Font font)
        {
            using (LinearGradientBrush backgroundBrush = new LinearGradientBrush(rectangle, Color.Black, m_backgroundColor, LinearGradientMode.ForwardDiagonal))
            using (LinearGradientBrush outlineBrush = new LinearGradientBrush(rectangle, m_backgroundColor, Color.Black, LinearGradientMode.ForwardDiagonal))
            using (SolidBrush titleBrush = new SolidBrush(m_titleColor))
            using (SolidBrush foregroundBrush = new SolidBrush(m_foregroundColor))
            using (Pen outlinePen = new Pen(outlineBrush))
            using (Font titleFont = new Font(font.FontFamily, font.SizeInPoints + 2.0f, FontStyle.Bold, GraphicsUnit.Point))
            {
                graphics.FillRectangle(backgroundBrush, rectangle);
                //e.DrawBorder();

                graphics.DrawRectangle(outlinePen, rectangle);

                if (m_img != null)
                    graphics.DrawImage(m_img, new Point(2, 2));

                // title
                graphics.DrawString(m_titleText, titleFont, titleBrush, (8.0f + (float)m_img.Width), 2.0f);
                Size size = TextRenderer.MeasureText(Title, titleFont);

                // text
                graphics.DrawString(m_text, font, foregroundBrush, (8.0f + (float)m_img.Width), (8.0f + (float)size.Height));
            }
        }
    }
}
