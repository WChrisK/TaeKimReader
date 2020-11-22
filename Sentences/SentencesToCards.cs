using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using TaeKimReader.Words;

namespace TaeKimReader.Sentences
{
    public static class SentencesToCards
    {
        private const string BreakTag = "<br>";

        public static void Write(SentenceExamples sentences, string cardOutputPath)
        {
            List<SentenceCard> cards = new();
            using StreamWriter w = new(File.OpenWrite(cardOutputPath), Encoding.UTF8);

            foreach (var (version, sentenceExample) in sentences.GetSortedVersionDictionary())
            {
                Debug.Assert(sentenceExample.Count > 0, $"Section {version} has zero sentences");

                WriteChapterCard(version, sentenceExample.Count, cards);
                WriteWordCards(sentenceExample, cards);
            }

            foreach (SentenceCard card in cards)
                w.WriteLine(card);
        }

        private static void WriteChapterCard(Version version, int sentenceExampleCount, ICollection<SentenceCard> cards)
        {
            string frontHtml = $"Chapter {version} ({sentenceExampleCount} examples)";
            const string backHtml = "You should bury this card, not answer it!";

            SentenceCard card = new(frontHtml, backHtml);
            cards.Add(card);
        }

        private static void WriteWordCards(IEnumerable<SentenceExample> sentenceExamples, ICollection<SentenceCard> cards)
        {
            foreach (SentenceExample sentenceExample in sentenceExamples)
            {
                StringBuilder front = new();
                StringBuilder translation = new();

                foreach (Sentence sentence in sentenceExample)
                {
                    foreach (WordElement element in sentence.Words)
                        front.Append(element.Text);

                    front.Append(BreakTag);
                    translation.Append(sentence.English + BreakTag);
                }

                // Trim the last <br>
                front.Remove(front.Length - BreakTag.Length, BreakTag.Length);
                translation.Remove(translation.Length - BreakTag.Length, BreakTag.Length);

                SentenceCard card = new(front.ToString(), translation.ToString());
                cards.Add(card);
            }
        }
    }
}
