using System;
using System.Threading;
using System.IO;
using System.IO.Pipes;

//CLIENT
namespace Lab_4
{
    class CGardener
    {
        private const string PipeName = "dc4_pipe";

        public CGardener()
        {}

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
            Console.WriteLine("Садовник создан. Pipe: {0}", PipeName);

            while(true)
            {
                try
                {
                    Console.WriteLine("Ищю завянувшие цветы");
                    var response = SendCommand(Command.SearchWiltedFlower);
                    switch (response)
                    {
                        case Response.Relax:
                            Console.WriteLine("Отдыхаю");
                            Thread.Sleep(3000);
                            break;
                        case Response.Watering:
                            Console.WriteLine("Поливаю");
                            Thread.Sleep(4000);
                            break;
                        default:
                            Console.WriteLine("Неизвестная команда");
                            Thread.Sleep(1000);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
        }

        private Response SendCommand(Command command)
        {
            NamedPipeClientStream pipeStream = CreatePipeClientStream();
            TryConnect(pipeStream);
            pipeStream.WriteByte((byte)command);
            pipeStream.WaitForPipeDrain();
            Response response = (Response)pipeStream.ReadByte();
            pipeStream.WaitForPipeDrain();
            return response;
        }
    }
}
