using System.Drawing;

namespace ShapeApp
{
    public class Line : Shape
    {
        public Point Start { get; }
        public Point End { get; }

        public double Length
        {
            get
            {
                double dx = End.X - Start.X;
                double dy = End.Y - Start.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }
        }

        public Line(Point start, Point end) : base("Шулуун")
        {
            Start = start;
            End = end;
        }

        // Шулуун талбайгүй
        public override double Area() => 0;

        // Периметр гэж шулууны уртыг авна
        public override double Perimeter() => Length;

        // Цэгүүд абсолют координаттай тул x, y ашиглахгүй
        public override void Draw(Graphics g, int x, int y)
        {
            using var pen = new Pen(FillColor, 3);
            g.DrawLine(pen, Start, End);
        }

        // Шулуунд талбай байхгүй тул мэдээллийг өөрчилнө
        public override string Info()
        {
            return $"{Name}: урт = {Length:F2}";
        }
    }
}