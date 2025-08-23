using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lpr381Project
{
    public static class Logger
    {
        private static StreamWriter writer;

        public static void Init(string filePath = "log.txt", bool append = false)
        {
            writer = new StreamWriter(filePath, append);
            writer.AutoFlush = true;
        }

        public static void Close()
        {
            writer?.Close();
        }

        public static void Write(string message)
        {
            writer.Write(message);
        }

        public static void WriteLine(string message = "")
        {
            writer.WriteLine(message);
        }
    }
}
