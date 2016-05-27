using System;

namespace Lab_4
{
    class Program
    {
        static void Main(string[] args)
        {
            CFlowerbed flowerbed = new CFlowerbed(10);
            CGardener[] gardener = { new CGardener(flowerbed), new CGardener(flowerbed)};
        }
    }
}
