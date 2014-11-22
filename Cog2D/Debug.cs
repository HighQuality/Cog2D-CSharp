using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cog
{
    public static class Debug
    {
        private const ConsoleColor outputInfoColor = ConsoleColor.White,
            successColor = ConsoleColor.DarkGreen,
            errorColor = ConsoleColor.DarkRed,
            eventColor = ConsoleColor.DarkYellow,
            infoColor = ConsoleColor.Gray,
            warningColor = ConsoleColor.Yellow;

        public static void Info(string message, params object[] parameters)
        {
            var oldColor = Console.ForegroundColor;
            OutputInfo("Info");
            Console.ForegroundColor = infoColor;
            Console.WriteLine(message, parameters);
            Console.ForegroundColor = oldColor;
        }

        public static void Warning(string message, params object[] parameters)
        {
            var oldColor = Console.ForegroundColor;
            OutputInfo("Warning");
            Console.ForegroundColor = warningColor;
            Console.WriteLine(message, parameters);
            Console.ForegroundColor = oldColor;
        }

        public static void Event(string message, params object[] parameters)
        {
            var oldColor = Console.ForegroundColor;
            OutputInfo("Event");
            Console.ForegroundColor = eventColor;
            Console.WriteLine(message, parameters);
            Console.ForegroundColor = oldColor;
        }

        public static void Error(string message, params object[] parameters)
        {
            var oldColor = Console.ForegroundColor;
            OutputInfo("Error");
            Console.ForegroundColor = errorColor;
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

        public static void NewLine()
        {
            Console.WriteLine();
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
