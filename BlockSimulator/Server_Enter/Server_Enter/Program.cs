using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Server_Enter
{
    class Program
    {
        MyServer myServer = new MyServer();

        static void Main(string[] args)
        {
            new Program().Run();
        }

        void Run()
        {

            while (true)
            {
                Console.WriteLine("1. 서비스 시작");
                Console.WriteLine("2. 서비스 종료");
                Console.WriteLine("3. 브로드케스트");
                Console.WriteLine("4. 유저  킥");
                Console.WriteLine("5. 유저  밴");
                Console.WriteLine("6. 종료");
                int num;
                int.TryParse(Console.ReadLine(), out num);
                Console.WriteLine(num);
                //  Console.Clear();
                switch (num)
                {
                    case 1: myServer.ServerStart(); break;
                    case 2: myServer.ServerClose(); break;
                    //case 3: myServer.ServerClose(); break;
                    //case 4: myServer.ServerClose(); break;
                    //case 5: myServer.ServerClose(); break;
                    case 6: myServer.ServerClose(); return;
                }


            }
        }

    }
}
