using System;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;


namespace DuckHunt
{
    public class Bullet
    {
        private double x, y;

        private double speed = 25;

        private double direction;

        private string has_hit = "";

        private Ellipse sprite;

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public double Speed { get => speed; set => speed = value; }
        public double Direction { get => direction; set => direction = value; }
        public string Has_hit { get => has_hit; set => has_hit = value; }
        public Ellipse Sprite { get => sprite; set => sprite = value; }

        public Bullet(double x, double y, double direction, Canvas canvas)
        {
            X = x;
            Y = y;
            Direction = direction - 90;

            Sprite = new Ellipse
            {
                Width = 15,
                Height = 15,
                Fill = Brushes.Red
            };
        }

        public void UpdatePosition(Window main_window, Duck duck)
        {
            X += speed * Math.Cos(direction * Math.PI / 180);
            Y += speed * Math.Sin(direction * Math.PI / 180);

            if (DetectCollision(duck))
            {
                Has_hit = "duck";
            }
            else if (DetectOutOfBounds(main_window))
            {
                Has_hit = "border";
            }

            Canvas.SetLeft(Sprite, X - Sprite.Width / 2);
            Canvas.SetTop(Sprite, Y - Sprite.Height / 2);
        }

        public bool DetectCollision(Duck duck)
        {
            return X >= duck.X && X <= duck.X + duck.Sprite.Width && Y >= duck.Y && Y <= duck.Y + duck.Sprite.Width;
        }

        public bool DetectOutOfBounds(Window main_window)
        {
            return X < 0 || X > main_window.ActualWidth || Y < 0 || Y > main_window.ActualHeight;
        }
    }
}