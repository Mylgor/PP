using System;
using System.Collections.Generic;
using System.Threading;
using System.IO.Pipes;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

//SERVER
namespace Lab_4
{
    enum Command
    {
        SearchWiltedFlower = 1,

        AmIWatered = 2,
        GrowingFlower = 3,
        WitherFlower = 4
    }

    enum Response
    {
        Ok = 0,
        Error = 1,
        Relax = 2,
        Watering = 3,

        FlowerWatered = 4,
        FlowerDontWatered = 5
    }

    class CFlowerbed
    {
        private const string PipeName = "dc4_pipe";

        private int _capacity;
        private int _flowersInside;
        private int _wateredFlower;
        private Dictionary<int, Command> _flowers;
        private NamedPipeServerStream pipeServer;

        public CFlowerbed(int capacity)
        {
            _capacity = capacity;
            _flowersInside = 0;
            _wateredFlower = 0;

            _flowers = new Dictionary<int, Command>();
        }

        private static NamedPipeServerStream CreatePipeServer()
        {
            return new NamedPipeServerStream(
                PipeName,
                PipeDirection.InOut,
                NamedPipeServerStream.MaxAllowedServerInstances,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous
            );
        }

        private void PrintInfo()
        {
            Console.WriteLine("Всего цветов: {0}, полито: {1}", _flowersInside, _wateredFlower);
        }

        private Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);
            return obj;
        }

        public void Start()
        {
            object obj = null;
            List<int> flower = null;

            Console.WriteLine("Грядка создана. Вместимость: {0}. Pipe: {1}.", _capacity, PipeName);
            while (true)
            {
                pipeServer = CreatePipeServer();
                pipeServer.WaitForConnection();
                try
                {
                    byte[] buffer = new byte[256];
                    int temp = pipeServer.Read(buffer, 0, buffer.Length);

                    if (temp > 1)
                    {


                        obj = ByteArrayToObject(buffer);
                        flower = obj as List<int>;
                    }
                    else
                    {
                        if (flower != null)
                        {
                            flower.Clear();
                            flower = null;
                        }
                    }

                    Command command;

                    //извлекаем команду цветка
                    if (flower != null)
                        command = (Command)flower[1];
                    else
                        command = (Command)buffer[0];
                    
                    switch (command)
                    {
                        case Command.WitherFlower:
                            WitherFlower(flower);//вянут
                            break;
                        case Command.SearchWiltedFlower:
                            SearchWiltedFlower(); //поиск завядших цветов
                            break;
                        case Command.GrowingFlower:
                            GrowingFlower(flower); //растут
                            break;
                        case Command.AmIWatered: //проверка полит ли цветок
                            AmIWatered(flower);
                            break;
                        default:
                            Console.WriteLine("Неизвестная команда");
                            break;
                    }
                    pipeServer.WaitForPipeDrain();
                    if (command != Command.AmIWatered)
                    {
                        PrintInfo();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void WitherFlower(List<int> flower)
        {
            _wateredFlower--;
            UpdateFlowers(flower[0], (Command)flower[1]);
            pipeServer.WriteByte((byte)Response.Ok);
        }

        private void GrowingFlower(List<int> flower)
        {
            //_wateredFlower++;
            UpdateFlowers(flower[0], (Command)flower[1]);

            pipeServer.WriteByte((byte)Response.Ok);
        }

        private void AmIWatered(List<int> flower)
        {
            Command cmd;
            bool exist = _flowers.TryGetValue(flower[0], out cmd);
            if (exist)
            {
                if (cmd == Command.WitherFlower)
                    pipeServer.WriteByte((byte)Response.FlowerDontWatered);
                else
                    pipeServer.WriteByte((byte)Response.FlowerWatered);
            }
            else
                pipeServer.WriteByte((byte)Response.Error);
        }

        private void UpdateFlowers(int idFlower, Command command)
        {
            Command cmd;
            bool exist = _flowers.TryGetValue(idFlower, out cmd);
           
            if (!exist)
            {
                _flowersInside++;
                _wateredFlower++;
                _flowers.Add(idFlower, command);
            }
            else if (cmd != command)
            {
                _flowers[idFlower] = command;
            }
        }

        private void SearchWiltedFlower()
        {
            bool isFound = false;
            int idFlower = _flowers.FirstOrDefault(x => x.Value == Command.WitherFlower).Key;

            if (idFlower != 0)
            {
                isFound = true;
                Console.WriteLine("Поливается цветок под номером {0}", idFlower);
                _flowers[idFlower] = Command.GrowingFlower;
                _wateredFlower++;
                pipeServer.WriteByte((byte)Response.Watering);
            }

            if (!isFound)
            {
                Console.WriteLine("Вянувших цветов нет, садовник отдыхает");
                pipeServer.WriteByte((byte)Response.Relax);
            }
        }
    }
}
