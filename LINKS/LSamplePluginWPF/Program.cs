using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechEmulator
{
    /// <summary>
    /// Used when project is set as WindowsApplication.
    /// If we change the project type to a library this class will not be initialized.
    /// Note: WindowsApplication will react same as dll when it's called from LINKS using Functions.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Loads the window just as if it was called from LINKS.
            // Running in WindowsApplication mode will not allow usage of UI templates.
            SpeechEmulator.Window.Open();
        }
    }
}