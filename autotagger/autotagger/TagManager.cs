using System.Collections.Generic;
using System.IO;

namespace autotagger
{
    public class TagManager
    {
        private List<Tag> tags = new List<Tag>();

        public TagManager()
        {
            Read();
        }

        public List<Tag> Get()
        {
            return tags;
        }

        private void Read()
        {
            if (!File.Exists("tags.txt")) return;

            StreamReader sr = new StreamReader("tags.txt");
            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                tags.Add(new Tag(s));
            }
            sr.Close();
        }

        public void Save()
        {
            TextWriter tw = new StreamWriter("tags.txt");
            foreach (Tag tag in tags)
            {
                tw.WriteLine(tag.Get());
            }
            tw.Close();
        }
    }
}
