using TaeKimReader.Sentences;
using TaeKimReader.Words;

namespace TaeKimReader
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Vocabulary wordDictionary = new(args[0]);
            SentenceExamples sentenceExamples = new(args[1], wordDictionary);
            //VocabularyToCards.Write(wordDictionary, args[2]);
            SentencesToCards.Write(sentenceExamples, args[2]);
        }
    }
}
