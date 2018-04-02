using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleExtensions
{
    public static class ConsoleOutput
    {
        public static void Write(string text)
        {
            Console.Write(text);
        }

        public static void Write(string text, ConsoleColor foregroundColor = ConsoleColor.Gray, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.Write(text);
            Console.ResetColor();
        }

        public static void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        public static void WriteLine(string text, ConsoleColor foregroundColor = ConsoleColor.Gray, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }

    public static class ConsoleInput
    {
        public static string ReadLine()
        {
            return Console.ReadLine();
        }

        public static string ReadLinePassword()
        {
            var password = new SecureString();

            while(true)
            {
                var theConsoleKeyInfo = Console.ReadKey(true);
                if(theConsoleKeyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if(theConsoleKeyInfo.Key == ConsoleKey.Backspace)
                {
                    if(password.Length > 0)
                    {
                        password.RemoveAt(password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    password.AppendChar(theConsoleKeyInfo.KeyChar);
                    Console.Write("*");
                }
            }

            var pSecureString = Marshal.SecureStringToGlobalAllocUnicode(password);
            string strSecureString = Marshal.PtrToStringUni(pSecureString);

            return strSecureString;
        }
    }
}
