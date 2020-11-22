using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TaeKimReader.Sentences
{
    public class SentenceExampleBuilder
    {
        private readonly List<Sentence> sentences = new();

        public int Count => sentences.Count;

        public void Add(Sentence sentence)
        {
            sentences.Add(sentence);
        }

        public ReadOnlyCollection<Sentence>? Build() => sentences.Count > 0 ? sentences.AsReadOnly() : null;
    }
}
