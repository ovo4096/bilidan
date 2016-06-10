using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiliDan.Live;

namespace BiliDanTest
{
    class Program
    {
        static void Main(string[] args)
        {
            List<DanMuListener> list = new List<DanMuListener>();
            for (int i = 0; i < 1000; i++)
            {
                DanMuListener listener = new DanMuListener(0);
                listener.Start();
                list.Add(listener);
            }
            Console.WriteLine(">_<");
            Console.ReadLine();

            foreach (var item in list)
            {
                item.Stop();
            }

            System.Threading.Thread.Sleep(1000 * 10);

            Console.WriteLine("End");
        }
    }
}
