using System;
using System.Threading;

namespace Lab_4
{
    class CGardener
    {
        public CFlowerbed Flowerbed { get; set; }
        public bool isWork { get; set; }

        private Thread thdGardener;

        public CGardener(CFlowerbed flwbed)
        {
            isWork = true;
            Flowerbed = flwbed;
            thdGardener = new Thread(new ThreadStart(Watering));
            thdGardener.Start();
        }



        public void Watering()
        {

            int i = 0;
            while (true)
            {
                if (!isWork)
                    thdGardener.Abort();
                if (i >= Flowerbed.CountFlowers)
                    i = 0;

                lock (Flowerbed)
                {
                    if (Flowerbed.GetFlower(i).State == CFlower.States.Wither)
                    {
                        Flowerbed.SetStateFlower(CFlower.States.Watering, i);
                    }
                }

                i++;
            }
        }
    }
}
