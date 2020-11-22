using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using TaeKimReader.Words;

namespace TaeKimReader.Sentences
{
    public class Sentence
    {
        public readonly ReadOnlyCollection<WordElement> Words;
        public readonly string English;

        public Sentence(List<WordElement> words, string english)
        {
            Words = words.AsReadOnly();
            English = english;
        }

        public override string ToString()
        {
            StringBuilder builder = new();
            StringBuilder hiragana = new();

            foreach (WordElement element in Words)
            {
                builder.Append(element.Text);
                hiragana.Append(element.Ruby ?? element.Text);
            }

            return builder != hiragana ? $"{builder} [{hiragana}] ({English})" : $"{builder} ({English})";
        }
    }
}
