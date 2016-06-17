using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSampleTestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            // Make sure jarvisWPF is running
            LSamplePluginWPF.MyCustomClass.Test_InitWPFWindow();
        }
    }
}
