using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TaeKimReader
{
    public class SentenceExample : IEnumerable<Sentence>
    {
        private readonly ReadOnlyCollection<Sentence> sentences;

        internal SentenceExample(SentenceExampleBuilder builder)
        {
            sentences = builder.Build() ?? throw new Exception("Cannot make a sentence example from no sentences");
        }

        public IEnumerator<Sentence> GetEnumerator() => sentences.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
