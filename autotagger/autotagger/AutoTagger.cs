using System;
using System.Collections.Generic;
using System.Linq;

namespace autotagger
{
    public class AutoTagger
    {
        private TagManager tagManager;

        public AutoTagger()
        {
            tagManager = new TagManager();
            Console.WriteLine("Tags read");
        }

        private void PrintHelp()
        {
            Console.WriteLine("Usages:");
            Console.WriteLine("autotagger parse [term]");
            Console.WriteLine("autotagger learn [word] [tag]");
            Console.WriteLine("autotagger tags - show existing tags");
        }

        private void Parse(string[] _args)
        {
            if (_args.Length == 0)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("autotagger parse [term]");
                Console.WriteLine("Where term is you want to autotag");
                return;
            }

            string term = string.Join(" ", _args.Select(_s => _s.ToString()));

        }

        private void Learn(string[] _args)
        {
            if (_args.Length < 0)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("autotagger learn [word] [tag]");
                Console.WriteLine("Associate word with tag");
                return;
            }
        }

        private void ShowTags()
        {
            List<Tag> tags = tagManager.Get();

            Console.WriteLine($"Found {tags.Count} tags");
            foreach (Tag tag in tags)
            {
                Console.WriteLine(tag.Get());
            }
        }


        public void Run(string _mode, string[] _args)
        {
            switch (_mode)
            {
                case "learn":
                    Learn(_args);
                    break;
                case "parse":
                    Parse(_args);
                    break;
                case "tags":
                    ShowTags();
                    break;
                default:
                    PrintHelp();
                    break;
            }

        }

        ~AutoTagger()
        {
            Console.WriteLine("Saving tags");
            tagManager.Save();

            Console.WriteLine("Application exiting");
        }
    }
}
