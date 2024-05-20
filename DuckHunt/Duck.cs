using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace DuckHunt
{
    public class Duck : Entity
    {
        private bool sprinting;
        private int sprintDuration = 5;
        private int sprintCooldown = 7;
        private int quackCooldown = 4;

        public bool Sprinting { get => sprinting; set => sprinting = value; }
        public int SprintDuration { get => sprintDuration; set => sprintDuration = value; }
        public int SprintCooldown { get => sprintCooldown; set => sprintCooldown = value; }
        public int QuackCooldown { get => quackCooldown; set => quackCooldown = value; }


        public Duck(Image sprite, double speed, int start_x, int start_y, Dictionary<string, Key> movement_keys)
        {
            // defines the characteristics of the hunter

            Sprite = sprite;
            Speed = speed;

            X = start_x;
            Y = start_y;

            Canvas.SetLeft(Sprite, X);
            Canvas.SetTop(Sprite, Y);

            Movement_keys = movement_keys;
        }

        private void Sprint()
        {
            // increases temporarily the speed of the duck
        }

        private void Quack()
        {
            // the duck quacks, slowing the hunter for a short duration of time
        }
    }
}
