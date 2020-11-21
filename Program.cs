namespace TaeKimReader
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Vocabulary wordDictionary = new(args[0]);
            Sentences sentences = new(args[1], wordDictionary);
        }
    }
}
