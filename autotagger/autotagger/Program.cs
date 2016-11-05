using System;
using System.Collections.Generic;

namespace autotagger
{
    class Program
    {
        static void printHelp()
        {
            Console.WriteLine("Usages:");
            Console.WriteLine("autotagger explain [term] [meaning]");
            Console.WriteLine("autotagger tags - show existing tags");
            Console.WriteLine("autotagger explanations - show explanations");
        }

        static void explain()
        {
            
        }

        static void showTags()
        {
            TagManager tagManager = new TagManager();
            List<Tag> tags = tagManager.Get();

            Console.WriteLine(string.Format("Found {0} tags", tags.Count));
            foreach (Tag tag in tags)
            {
                Console.WriteLine(tag.Get());
            }
        }

        static void Main(string[] args)
        {
            if(args.Length == 0) printHelp();

            showTags();

            Console.ReadKey();
        }
    }
}
