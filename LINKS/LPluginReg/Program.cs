using PluginLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LPluginReg
{
    class Program
    {
        static void Main(string[] args)
        {
            Configuration c = new Configuration();
            c.Load();
            
            foreach (string file in args)
            {
                try
                {
                    Assembly a = Assembly.LoadFrom(file);
                    c.AddPlugin(a.FullName, file);
                    
                    var test = a.GetTypes();
                    test = null;
                }
                catch (Exception error)
                {                    
                    if (error is System.Reflection.ReflectionTypeLoadException)
                    {
                        ReflectionTypeLoadException typeLoadException = error as ReflectionTypeLoadException;
                        Exception LastException = null;
                        foreach (Exception e in typeLoadException.LoaderExceptions)
                        {
                            if (LastException == null || !LastException.Message.Equals(e.Message))
                            {
                                Console.WriteLine(e.Message);
                                if (e.InnerException != null)
                                    Console.WriteLine(e.InnerException.Message);
                            }
                            LastException = e;
                        }
                    }
                    else
                    {
                        Console.WriteLine(error.Message);
                        if (error.InnerException != null)
                            Console.WriteLine(error.InnerException.Message);
                    }
                    Console.ReadKey();                    
                }
            }

            c.Save();
            c = null;
        }

    }
    
}
