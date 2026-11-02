using System.Drawing;

namespace ShapeApp
{
    // Цэгүүдийг дарааллаар нь холбосон дүрс (шулуун эсвэл олон өнцөгт)
    public class Polyline : Shape
    {
        public List<Point> Points { get; }
        public bool Closed { get; }

        public Polyline(IEnumerable<Point> points, bool closed)
            : base(closed ? "Олон өнцөгт" : "Шулуун")
        {
            Points = points.ToList();
            Closed = closed;
        }

        // Талбай: хаалттай үед Гауссын томьёогоор тооцно
        public override double Area()
        {
            if (!Closed || Points.Count < 3) return 0;

            double sum = 0;
            for (int i = 0; i < Points.Count; i++)
            {
                Point a = Points[i];
                Point b = Points[(i + 1) % Points.Count];
                sum += (double)a.X * b.Y - (double)b.X * a.Y;
            }
            return Math.Abs(sum) / 2;
        }

        // Периметр: бүх хэрчмийн уртын нийлбэр
        public override double Perimeter()
        {
            double total = 0;
            int segments = Closed ? Points.Count : Points.Count - 1;
            for (int i = 0; i < segments; i++)
            {
                Point a = Points[i];
                Point b = Points[(i + 1) % Points.Count];
                double dx = b.X - a.X;
                double dy = b.Y - a.Y;
                total += Math.Sqrt(dx * dx + dy * dy);
            }
            return total;
        }

        // Цэгүүд нь абсолют координаттай тул x, y ашиглахгүй
        public override void Draw(Graphics g, int x, int y)
        {
            if (Points.Count < 2) return;

            Point[] pts = Points.ToArray();
            if (Closed)
            {
                using var brush = new SolidBrush(FillColor);
                g.FillPolygon(brush, pts);
                g.DrawPolygon(Pens.Black, pts);
            }
            else
            {
                using var pen = new Pen(FillColor, 3);
                g.DrawLines(pen, pts);
            }
        }
    }
}