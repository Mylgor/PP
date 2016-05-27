using System;

namespace Lab_4
{
    class CFlower
    {
        public enum States
        {
            Watering,
            Wither
        }

        public CFlower()
        {
            Name = "Tulip";
            State = States.Wither;
        }

        public int Index { get; set; }
        public string Name { get; set; }
        public States State { get; set; }
    }
}
