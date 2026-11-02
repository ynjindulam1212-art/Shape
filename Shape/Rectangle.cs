using System.Drawing;

namespace ShapeApp
{
    public class Rectangle : Shape
    {
        public double Width { get; }
        public double Height { get; }

        public Rectangle(double width, double height) : base("Тэгш өнцөгт")
        {
            Width = width;
            Height = height;
        }

        public override double Area() => Width * Height;
        public override double Perimeter() => 2 * (Width + Height);

        public override void Draw(Graphics g, int x, int y)
        {
            using var brush = new SolidBrush(FillColor);
            g.FillRectangle(brush, x, y, (int)Width, (int)Height);
            g.DrawRectangle(Pens.Black, x, y, (int)Width, (int)Height);
        }
    }
}