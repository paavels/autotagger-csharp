namespace autotagger
{
    public class Tag
    {
        private string tag;

        public Tag(string _tag)
        {
            tag = _tag;
        }

        public string Get()
        {
            return tag;
        }

        public void Set(string _tag)
        {
            tag = _tag;
        }
    }
}
