using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square
{
    public static class Debug
    {
        private const ConsoleColor outputInfoColor = ConsoleColor.White,
            successColor = ConsoleColor.DarkGreen,
            infoColor = ConsoleColor.DarkYellow;

        public static void Info(string message, params object[] parameters)
        {
            var oldColor = Console.ForegroundColor;
            OutputInfo("Info");
            Console.ForegroundColor = infoColor;
            Console.WriteLine(message, parameters);
            Console.ForegroundColor = oldColor;
        }

        public static void Success(string message, params object[] parameters)
        {
            var oldColor = Console.ForegroundColor;
            OutputInfo("Success");
            Console.ForegroundColor = successColor;
            Console.WriteLine(message, parameters);
            Console.ForegroundColor = oldColor;
        }

        /// <summary>
        /// Writes "[TimeStamp] messageType: "
        /// Does not reset foreground color!
        /// </summary>
        private static void OutputInfo(string messageType)
        {
            Console.ForegroundColor = outputInfoColor;
            Console.Write('[');
            Console.Write(DateTime.Now.ToString());
            Console.Write("] ");
            Console.Write(messageType);
            Console.Write(": ");
        }
    }
}
