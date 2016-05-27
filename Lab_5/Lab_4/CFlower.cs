using System;
using System.Threading;
using System.IO;
using System.IO.Pipes;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

//CLIENT
namespace Lab_4
{
    class CFlower
    {
        private const string PipeName = "dc4_pipe";
        public int IdFlower { get; private set; }

        public CFlower()
        {
            Random rand = new Random();
            IdFlower = rand.Next(1000, 1000000);
        }

        private NamedPipeClientStream CreatePipeClientStream()
        {
            return new NamedPipeClientStream(
                ".",
                PipeName,
                PipeDirection.InOut,
                PipeOptions.Asynchronous
            );
        }

        private void TryConnect(NamedPipeClientStream pipeStream)
        {
            try
            {
                pipeStream.Connect();
            }
            catch (FileNotFoundException)
            {
                Thread.Sleep(1000);
                TryConnect(pipeStream);
            }
        }

        public void Start()
        {
            Console.WriteLine("Цветок создан. Pipe: {0}.", PipeName);

            try
            {
                SendCommand(Command.GrowingFlower);
                while (true)
                {
                    Thread.Sleep(100);

                    Console.WriteLine("Росту");
                    Thread.Sleep(4000);
                    Console.WriteLine("Вяну");
                    SendCommand(Command.WitherFlower);
                    Response response = SendCommand(Command.AmIWatered);
                    while (response != Response.FlowerWatered)
                    {
                        Console.WriteLine("Жду своей очереди");
                        Thread.Sleep(4000);
                        response = SendCommand(Command.AmIWatered);
                    }
                    SendCommand(Command.GrowingFlower);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Thread.Sleep(1000);
            }
        }

        private Response SendCommand(Command command)
        {
            //создаем список включающий команду и id цветка
            List<int> data = new List<int>();
            data.Add(IdFlower);
            data.Add(command.GetHashCode());

            byte[] bytObj = ObjectToByteArray(data);

            NamedPipeClientStream pipeStream = CreatePipeClientStream();
            TryConnect(pipeStream);
            pipeStream.Write(bytObj, 0, bytObj.Length);
            pipeStream.WaitForPipeDrain();
            Response response = (Response)pipeStream.ReadByte();
            pipeStream.WaitForPipeDrain();
            return response;
        }

        private byte[] ObjectToByteArray(List<int> obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
