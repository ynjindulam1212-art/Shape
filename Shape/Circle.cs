using System.Drawing;

namespace ShapeApp
{
    public class Circle : Shape
    {
        public double Radius { get; }

        public Circle(double radius) : base("Тойрог")
        {
            Radius = radius;
        }

        public override double Area() => Math.PI * Radius * Radius;
        public override double Perimeter() => 2 * Math.PI * Radius;

        public override void Draw(Graphics g, int x, int y)
        {
            int d = (int)(Radius * 2);
            using var brush = new SolidBrush(FillColor); // сонгосон өнгөөр буддаг
            g.FillEllipse(brush, x, y, d, d);
            g.DrawEllipse(Pens.Black, x, y, d, d);
        }
    }
}