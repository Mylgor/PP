using System;

namespace Lab_4
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ArgumError();
            }

            switch (args[0])
            {
                case "gardener":
                    CGardener gardener = new CGardener();
                    gardener.Start();
                    break;
                case "flower":
                    CFlower flower = new CFlower();
                    flower.Start();
                    break;
                case "flowerbed":
                    if (args.Length < 2)
                    {
                        ArgumError();
                        break;
                    }
                    CFlowerbed flowerbed = new CFlowerbed(int.Parse(args[1]));
                    flowerbed.Start();
                    break;
            }
        }

        private static void ArgumError()
        {
            Console.WriteLine("Используйте:");
            Console.WriteLine("dc4.exe flowerbed <вместимость цветов>");
            Console.WriteLine("dc4.exe flower");
            Console.WriteLine("dc4.exe gardener");
            Environment.Exit(1);
        }
    }
}
