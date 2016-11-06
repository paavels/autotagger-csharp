using System;

namespace autotagger
{
    internal class Program
    {
        private static void Main(string[] _args)
        {
            AutoTagger app = new AutoTagger();
            //app.Run("parse", _args.Skip(1).ToArray());
            app.Run("parse", new[] { "MMGF2 MacBook Air 13\" i5 DC1.6GHz/ 8GB/ 128GB flash/ Intel HD 6000/ INT" });
            app.Run("parse", new[] { "MK482 Apple iMac 27\" Retina 5K QC i5 3.3GHz/ 8GB/ 2TB Fusion/ AMD Radeon R9 M395 2GB/ Int" });
            app.Run("parse", new[] { "Mobilais telefons Coolpad MAX 5.5\" 64GB - Champagne" });

            Console.ReadKey();
        }
    }
}
