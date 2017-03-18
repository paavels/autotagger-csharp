using System;

namespace autotagger
{
    internal class Program
    {
        private static void Main(string[] _args)
        {
            AutoTagger app = new AutoTagger();
            app.run();

            Console.ReadKey();

        }
    }
}
