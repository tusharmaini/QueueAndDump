using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QueueAndCallback
{
    class Program
    {
        static void Main(string[] args)
        {
            IFileWriter fileWriter = new DefferedWriter();

            for (int i = 0; i < 20; i++)
            {
                fileWriter.Write(Guid.NewGuid().ToString());
                Thread.Sleep(1000);
            }
        }
    }


    internal interface  IFileWriter
    {

        void Write(string content);

    }

    internal class DefferedWriter : IFileWriter
    {
        private static readonly ConcurrentQueue<string> Data;

        private static readonly Timer Timer;


        static DefferedWriter()
        {
            Data = new ConcurrentQueue<string>();

            Timer = new Timer(Callback, null, 1000, 2000);
        }

        public void Write(string content)
        {
            Data.Enqueue(content);
        }

        public static void Callback(object state)
        {
            var dataToWrite = new List<string>();

            string entry;

            while (Data.TryDequeue(out entry))
            {
                dataToWrite.Add(entry);
            }

            if (dataToWrite.Any())
                WriteToFile(dataToWrite);
        }

        private static void WriteToFile(List<string> dataToWrite)
        {
            using (StreamWriter writer = File.AppendText("D:/log.txt"))
            {
                string logAt = string.Format("Log Entry @ {0}", DateTime.Now.ToLongTimeString());
                Console.WriteLine(logAt);
                writer.WriteLine(logAt);
                foreach (var data in dataToWrite)
                {
                    writer.WriteLine(data);    
                    Console.WriteLine(data);
                }
                
            }
        }
    }
}
