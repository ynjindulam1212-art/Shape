using System.Drawing;

namespace ShapeApp
{
    // Бүх дүрсийн эх (абстракт) класс
    public abstract class Shape
    {
        public string Name { get; protected set; }

        // Дүрсийг будах өнгө (цэсээс солигдоно)
        public Color FillColor { get; set; } = Color.LightGray;

        protected Shape(string name)
        {
            Name = name;
        }

        public abstract double Area();
        public abstract double Perimeter();
        public abstract void Draw(Graphics g, int x, int y);

        public virtual string Info()
        {
            return $"{Name}: талбай = {Area():F2}, периметр = {Perimeter():F2}";
        }
    }
}