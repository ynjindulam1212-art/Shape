using System.Drawing;

namespace ShapeApp
{
    public class Triangle : Shape
    {
        public double Base { get; }
        public double Height { get; }

        public Triangle(double baseLength, double height) : base("Гурвалжин")
        {
            Base = baseLength;
            Height = height;
        }

        public override double Area() => 0.5 * Base * Height;

        public override double Perimeter()
        {
            double hypotenuse = Math.Sqrt(Base * Base + Height * Height);
            return Base + Height + hypotenuse;
        }

        public override void Draw(Graphics g, int x, int y)
        {
            Point[] points =
            {
        new Point(x, y + (int)Height),
        new Point(x + (int)Base, y + (int)Height),
        new Point(x, y)
    };
            using var brush = new SolidBrush(FillColor);
            g.FillPolygon(brush, points);
            g.DrawPolygon(Pens.Black, points);
        }
    }
}