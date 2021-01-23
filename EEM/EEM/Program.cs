using System;
using System.Reflection;

namespace EEM
{
    class Program
    {
        static void Main(string[] args)
        {
            var bytes = new byte[0];
            
            var assembly = AppDomain.Load(bytes);
            Type typeToExecute = assembly.GetType("Program");
            Object instance = Activator.CreateInstance(typeToExecute);
        }
    }
}
