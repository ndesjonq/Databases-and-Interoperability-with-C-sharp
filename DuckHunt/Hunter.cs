using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;

namespace DuckHunt
{
    public class Hunter : Entity
    {
        private double direction;

        private bool rapidFiring;
        private int rapidFireDuration = 8;
        private int rapidFireCooldown = 15;

        private List<Bullet> bullets;

        private bool quacked;
        private int quackedDuration = 2;

        public bool RapidFiring { get => rapidFiring; set => rapidFiring = value; }
        public int RapidFireDuration { get => rapidFireDuration; set => rapidFireDuration = value; }
        public int RapidFireCooldown { get => rapidFireCooldown; set => rapidFireCooldown = value; }
        public bool Quacked { get => quacked; set => quacked = value; }
        public int QuackedDuration { get => quackedDuration; set => quackedDuration = value; }
        internal List<Bullet> Bullets { get => bullets; set => bullets = value; }
        public double Direction { get => direction; set => direction = value; }

        public Hunter(Image sprite, double speed, int start_x, int start_y, Dictionary<string, Key> movement_keys)
        {
            Sprite = sprite;
            Speed = speed;

            X = start_x;
            Y = start_y;

            Canvas.SetLeft(Sprite, X);
            Canvas.SetTop(Sprite, Y);

            Movement_keys = movement_keys;
        }

        public void Shoot(Canvas canvas)
        {
            MainWindow.bullets.Add(new Bullet(
                Canvas.GetLeft(Sprite) + Sprite.ActualWidth / 2 + Sprite.ActualWidth * 0.4 * Math.Cos((Direction - 90) * Math.PI / 180),
                Canvas.GetTop(Sprite) + Sprite.ActualHeight / 2 + Sprite.ActualHeight * 0.4 * Math.Sin((Direction - 90) * Math.PI / 180),
                Direction, 
                canvas));

            canvas.Children.Add(MainWindow.bullets[^1].Sprite);
        }

        public void Rotate(Canvas canvas)
        {
            Vector vector1 = new(Mouse.GetPosition(canvas).X - (Canvas.GetLeft(Sprite) + Sprite.ActualWidth / 2),
                                 Mouse.GetPosition(canvas).Y - (Canvas.GetTop(Sprite) + Sprite.ActualHeight / 2));

            Vector vector2 = new(0, -1);

            Direction = Vector.AngleBetween(vector2, vector1);

            RotateTransform player_orientation = (RotateTransform)Sprite.RenderTransform;
            if (player_orientation.Angle != Direction)
            {
                player_orientation.Angle = Direction - 90;
            }
        }

        public void RapidFire()
        {
            // modifies the speed and fire rate temporarily
        }
    }
}
