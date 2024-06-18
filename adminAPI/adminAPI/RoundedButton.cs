using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace adminAPI
{
    public class RoundedButton : Button
    {
        // Радиус закругления углов
        public int CornerRadius { get; set; } = 20;

        // Цвет границы
        public Color BorderColor { get; set; } = Color.Yellow;

        // Толщина границы
        public int BorderThickness { get; set; } = 1;

        // Цвет подсветки при наведении
        public Color HoverBackColor { get; set; } = Color.LightCoral;

        private Color originalBackColor;
        private bool isHovered = false;

        public RoundedButton()
        {
            // Устанавливаем стиль, поддерживающий прозрачный фон
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.BackColor = Color.Transparent;
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.FlatStyle = FlatStyle.System;
            // Обработка событий для подсветки при наведении
            this.MouseEnter += RoundedButton_MouseEnter;
            this.MouseLeave += RoundedButton_MouseLeave;
        }

        private void RoundedButton_MouseEnter(object sender, EventArgs e)
        {
            isHovered = true;
            this.Invalidate();
        }

        private void RoundedButton_MouseLeave(object sender, EventArgs e)
        {
            isHovered = false;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Устанавливаем сглаживание для лучшего качества графики
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Определяем размеры и радиус
            int radius = CornerRadius;
            int diameter = radius * 2;
            int width = this.Width - 1;
            int height = this.Height - 1;

            // Создаем путь для закругленных углов
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(0, 0, diameter, diameter, 180, 90);
            path.AddArc(width - diameter, 0, diameter, diameter, 270, 90);
            path.AddArc(width - diameter, height - diameter, diameter, diameter, 0, 90);
            path.AddArc(0, height - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            // Заливка фона кнопки
            using (SolidBrush brush = new SolidBrush(isHovered ? HoverBackColor : this.BackColor))
            {
                e.Graphics.FillPath(brush, path);
            }

            // Рисование границы кнопки
            using (Pen pen = new Pen(BorderColor, BorderThickness))
            {
                e.Graphics.DrawPath(pen, path);
            }

            // Рисование текста кнопки
            TextRenderer.DrawText(e.Graphics, this.Text, this.Font, this.ClientRectangle, this.ForeColor, Color.Transparent,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }
}