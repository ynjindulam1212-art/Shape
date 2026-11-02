using System.Drawing;
using System.Windows.Forms;

namespace ShapeApp
{
    public partial class Form1 : Form
    {
        // Зурагдсан бүх дүрс ба тэдгээрийн байрлал
        private readonly List<(Shape Shape, Point Location)> drawnShapes = new();

        // Чирж зурах дүрсийг (өргөн, өндрөөр) үүсгэх функц. Сонгоогүй бол null
        private Func<int, int, Shape>? shapeFactory;

        // Сонгосон өнгө
        private Color currentColor = Color.SkyBlue;

        // Хулганаар чирж байгаа төлөв
        private bool isDragging;
        private Point startPoint;
        private Point currentPoint;

        // Шулуун (цэг холбох) горим
        private bool isPolylineMode;
        private readonly List<Point> pendingPoints = new();

        private readonly MenuStrip menu = new();

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            KeyPreview = true; // Esc товчийг хүлээж авахын тулд
            BuildMenu();
            Text = "Дүрсүүд - цэснээс дүрс сонгоно уу";
        }

        // Дээд талын цэсийг кодоор үүсгэнэ
        private void BuildMenu()
        {
            // ----- "Дүрсүүд" цэс -----
            var shapeMenu = new ToolStripMenuItem("Дүрсүүд");
            shapeMenu.DropDownItems.Add("Тойрог", null, (s, e) =>
                SetTool("Тойрог", (w, h) => new Circle(Math.Min(w, h) / 2.0)));
            shapeMenu.DropDownItems.Add("Тэгш өнцөгт", null, (s, e) =>
                SetTool("Тэгш өнцөгт", (w, h) => new Rectangle(w, h)));
            shapeMenu.DropDownItems.Add("Гурвалжин", null, (s, e) =>
                SetTool("Гурвалжин", (w, h) => new Triangle(w, h)));
            shapeMenu.DropDownItems.Add("Шулуун (цэг холбох)", null, (s, e) => SetPolylineTool());
            shapeMenu.DropDownItems.Add(new ToolStripSeparator());
            shapeMenu.DropDownItems.Add("Цэвэрлэх", null, (s, e) =>
            {
                drawnShapes.Clear();
                pendingPoints.Clear();
                Invalidate();
            });

            // ----- "Өнгө" цэс -----
            var colorMenu = new ToolStripMenuItem("Өнгө");
            var colors = new (string Name, Color Value)[]
            {
                ("Цэнхэр", Color.SkyBlue),
                ("Ногоон", Color.LightGreen),
                ("Улаан", Color.IndianRed),
                ("Шар", Color.Gold),
                ("Улбар шар", Color.Orange),
                ("Ягаан", Color.Plum)
            };
            foreach (var c in colors)
            {
                colorMenu.DropDownItems.Add(c.Name, null, (s, e) => currentColor = c.Value);
            }
            colorMenu.DropDownItems.Add(new ToolStripSeparator());
            colorMenu.DropDownItems.Add("Өөр өнгө...", null, (s, e) =>
            {
                using var dialog = new ColorDialog { Color = currentColor };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    currentColor = dialog.Color;
                }
            });

            menu.Items.Add(shapeMenu);
            menu.Items.Add(colorMenu);
            MainMenuStrip = menu;
            Controls.Add(menu);
        }

        // Чирж зурах дүрсээ сонгоно
        private void SetTool(string name, Func<int, int, Shape> factory)
        {
            FinishPolyline(); // хагас зурсан цэгүүд байвал дүрс болгож хадгална
            shapeFactory = factory;
            isPolylineMode = false;
            Text = $"Дүрсүүд - сонгосон: {name} (хулганаар чирж зурна)";
            Invalidate();
        }

        // Шулуун (цэг холбох) хэрэгслийг сонгоно
        private void SetPolylineTool()
        {
            FinishPolyline();
            shapeFactory = null;
            isPolylineMode = true;
            Text = "Шулуун - зүүн товчоор цэг тавина. Эхний цэг дээр дарвал дүрс хаагдана. Баруун товч = дуусгах, Esc = болиулах";
            Invalidate();
        }

        // Цэгүүдийг холбож дүрс болгоно (3+ цэг бол хаагдаж будагдана)
        private void FinishPolyline()
        {
            if (pendingPoints.Count >= 2)
            {
                bool closed = pendingPoints.Count >= 3;
                var shape = new Polyline(pendingPoints, closed) { FillColor = currentColor };
                drawnShapes.Add((shape, Point.Empty));
            }
            pendingPoints.Clear();
            Invalidate();
        }

        // Хоёр цэгийн хоорондох зай
        private static double Distance(Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        // Чирсэн хоёр цэгээс зүүн дээд буланг, өргөн, өндрийг гаргана
        private (int X, int Y, int W, int H) GetDragBounds()
        {
            int x = Math.Min(startPoint.X, currentPoint.X);
            int y = Math.Min(startPoint.Y, currentPoint.Y);
            int w = Math.Abs(startPoint.X - currentPoint.X);
            int h = Math.Abs(startPoint.Y - currentPoint.Y);
            return (x, y, w, h);
        }

        // Esc дарвал цэг холбож байгаа дүрсийг болиулна
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape && isPolylineMode)
            {
                pendingPoints.Clear();
                Invalidate();
            }
        }

        // Хулганы товч дарах
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // Шулуун горим: зүүн товч = цэг нэмэх, баруун товч = дуусгах
            if (isPolylineMode)
            {
                if (e.Button == MouseButtons.Left)
                {
                    // Эхний цэг дээр дахин дарвал дүрс хаагдаж дуусна
                    if (pendingPoints.Count >= 3 && Distance(e.Location, pendingPoints[0]) <= 10)
                    {
                        FinishPolyline();
                        return;
                    }

                    pendingPoints.Add(e.Location);
                    currentPoint = e.Location;
                    Invalidate();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    FinishPolyline();
                }
                return;
            }

            // Чирж зурах горим
            if (e.Button != MouseButtons.Left || shapeFactory == null) return;

            isDragging = true;
            startPoint = e.Location;
            currentPoint = e.Location;
        }

        // Хулгана хөдлөх: урьдчилан харуулах
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (isPolylineMode)
            {
                if (pendingPoints.Count > 0)
                {
                    currentPoint = e.Location;
                    Invalidate();
                }
                return;
            }

            if (!isDragging) return;

            currentPoint = e.Location;
            Invalidate();
        }

        // Хулганы товч суллах: чирсэн дүрсийг жагсаалтад нэмнэ
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (isPolylineMode || !isDragging || shapeFactory == null) return;

            isDragging = false;
            currentPoint = e.Location;

            var (x, y, w, h) = GetDragBounds();
            if (w >= 5 && h >= 5) // хэт жижиг чирэлтийг тоохгүй
            {
                Shape shape = shapeFactory(w, h);
                shape.FillColor = currentColor;
                drawnShapes.Add((shape, new Point(x, y)));
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Зурагдсан бүх дүрс
            foreach (var item in drawnShapes)
            {
                item.Shape.Draw(e.Graphics, item.Location.X, item.Location.Y);
            }
            // Эхний цэг дээр дарвал дүрс хаагдана гэдгийг харуулах дугуй
            if (pendingPoints.Count >= 3)
            {
                Point first = pendingPoints[0];
                e.Graphics.DrawEllipse(Pens.Red, first.X - 8, first.Y - 8, 16, 16);
            }

            // Чирч байх үеийн урьдчилсан дүрс
            if (isDragging && shapeFactory != null)
            {
                var (x, y, w, h) = GetDragBounds();
                if (w > 0 && h > 0)
                {
                    Shape preview = shapeFactory(w, h);
                    preview.FillColor = currentColor;
                    preview.Draw(e.Graphics, x, y);
                }
            }

            // Цэг холбож байх үеийн урьдчилсан шугам
            if (isPolylineMode && pendingPoints.Count > 0)
            {
                using var pen = new Pen(currentColor, 3);
                if (pendingPoints.Count >= 2)
                {
                    e.Graphics.DrawLines(pen, pendingPoints.ToArray());
                }
                // Сүүлийн цэгээс хулганы байрлал хүртэл
                e.Graphics.DrawLine(pen, pendingPoints[^1], currentPoint);

                foreach (Point p in pendingPoints)
                {
                    e.Graphics.FillEllipse(Brushes.Black, p.X - 3, p.Y - 3, 6, 6);
                }
            }

            // Сүүлд зурсан дүрсний мэдээллийг доор харуулна
            if (drawnShapes.Count > 0)
            {
                string info = drawnShapes[^1].Shape.Info();
                e.Graphics.DrawString(info, Font, Brushes.Black, 10, ClientSize.Height - 30);
            }
        }
    }
}