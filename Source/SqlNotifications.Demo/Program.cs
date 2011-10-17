using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using SqlNotifications.Demo.Scenarios.LocalUser;

namespace SqlNotifications.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            //new LocalUserScenario().Start();

            Hashtable ht = new Hashtable();

            HashSet<int> hs = new HashSet<int>();

            int j = 0;

            string[] ar = new string[12000000];

            for(int i = 0; i < 12000000; i++)
            {
                hs.Add(i);
                //ar[i] = i.ToString();
                j++;
            }

            GC.Collect();

            Console.WriteLine("Test" + j);
            Console.ReadLine();
        }
    }
}
