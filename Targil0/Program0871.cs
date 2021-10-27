using System;

namespace Targil0
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Welcome0871();
        }

        static partial void Welcome3539();
        private static void Welcome0871()
        {
            Console.Write("Enter your name: ");
            string name = Console.ReadLine();
            Console.WriteLine("{0}, welcome to my first console application.\n", name);
            Console.ReadKey();
        }
    }
}
