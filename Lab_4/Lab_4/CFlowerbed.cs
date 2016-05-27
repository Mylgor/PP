using System;
using System.Collections.Generic;
using System.Threading;

namespace Lab_4
{
    class CFlowerbed
    {
        public int CountFlowers { get; set; }

        private object obj = new object();
        private List<CFlower> m_flowers;


        //конструктор
        public CFlowerbed(int cntFlowers = 0)
        {
            CountFlowers = cntFlowers;
            m_flowers = new List<CFlower>();
            for (int i = 0; i < CountFlowers; i++)
            {
                CFlower tempFlower = new CFlower();
                m_flowers.Add(tempFlower);
            }

            Thread thdSetStage = new Thread(new ThreadStart(TimeBetweenSetStageFlower));
            thdSetStage.Start();
        }



        public CFlower GetFlower(int index)
        {
            if (index < CountFlowers)
                return m_flowers[index];
            else
                return null;
        }


        void TimeBetweenSetStageFlower()
        {
            TimerCallback tmrCallback = new TimerCallback(ChangeState);
            Timer timer = new Timer(tmrCallback, null, 3000, 3000);
        }


        void ChangeState(object sender)
        {
            for (int i = 0; i < m_flowers.Count; i++)
            {
                SetStateFlower(CFlower.States.Wither, i);
            }
        }


        public void SetStateFlower(CFlower.States state, int ind)
        {
            lock (obj)
            {
                if (ind < m_flowers.Count)
                {
                    m_flowers[ind].State = state;

                    Console.WriteLine("flower number: {0}, state: {1}", ind.ToString(), m_flowers[ind].State.ToString());
                }
            }
        }
    }
}
