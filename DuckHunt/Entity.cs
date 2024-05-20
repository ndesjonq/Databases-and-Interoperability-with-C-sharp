using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DuckHunt
{
    public class Entity
    {
        private double x, y;

        private bool left, right, up, down;
        private Dictionary<string, Key> movement_keys;

        private Image sprite;

        private double speed;

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public bool Left { get => left; set => left = value; }
        public bool Right { get => right; set => right = value; }
        public bool Up { get => up; set => up = value; }
        public bool Down { get => down; set => down = value; }
        public Image Sprite { get => sprite; set => sprite = value; }
        public Dictionary<string, Key> Movement_keys { get => movement_keys; set => movement_keys = value; }
        public double Speed { get => speed; set => speed = value; }

        public void Move(Canvas canvas)
        {
            if (Left)
            {
                X -= speed;
            }
            if (Right)
            {
                X += speed;
            }
            if (Up)
            {
                Y -= speed;
            }
            if (Down)
            {
                Y += speed;
            }

            if (X < 0)
            {
                X = 0;
            }

            if (X > canvas.ActualWidth - Sprite.Width)
            {
                X = canvas.ActualWidth - Sprite.Width;
            }

            if (Y <= 0)
            {
                Y = 0;
            }

            if (Y > canvas.ActualHeight - Sprite.Width)
            {
                Y = canvas.ActualHeight - Sprite.Width;
            }

            Canvas.SetLeft(Sprite, X);
            Canvas.SetTop(Sprite, Y);
        }
    }
}
