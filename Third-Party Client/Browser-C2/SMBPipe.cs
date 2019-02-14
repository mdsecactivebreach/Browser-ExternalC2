using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Pipes;


namespace Browser_C2
{
    class SMBPipe
    {

        public string PipeName;
        public NamedPipeClientStream client;
        private const int MaxBufferSize = 1024 * 1024;
        public void ConnectToPipe()
        {

            client = new NamedPipeClientStream(PipeName);
            var tries = 0;
            while (client.IsConnected == false)
            {
                if (tries == 20) break;
                client.Connect();
                tries++;
                System.Threading.Thread.Sleep(1000);
            }


        }
        public void SendData(byte[] response)
        {
            
           
            var writer = new BinaryWriter(client);
            writer.Write(response.Length);
            writer.Write(response);

            return;
        }
        public byte[] GetData()
        {

            var reader = new BinaryReader(client);
            var bufferSize = reader.ReadInt32();
            var size = bufferSize > MaxBufferSize
                ? MaxBufferSize
                : bufferSize;

            return reader.ReadBytes(size);


        }

    }
}