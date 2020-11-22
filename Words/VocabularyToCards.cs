using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TaeKimReader.Words
{
    public static class VocabularyToCards
    {
        public static void Write(Vocabulary vocabulary, string cardOutputPath)
        {
            List<VocabularyCard> cards = new();
            using StreamWriter w = new(File.OpenWrite(cardOutputPath), Encoding.UTF8);

            foreach (var (version, wordList) in vocabulary.GetSortedVersionDictionary())
            {
                Debug.Assert(wordList.Count > 0, $"Section {version} has zero words");

                WriteChapterCard(version, wordList.Count, cards);
                WriteWordCards(wordList, cards);
            }

            foreach (VocabularyCard card in cards)
                w.WriteLine(card);
        }

        private static void WriteChapterCard(Version version, int wordListCount, ICollection<VocabularyCard> cards)
        {
            string frontHtml = $"Chapter {version} ({wordListCount} words)";
            const string backHtml = "You should bury this card, not answer it!";

            VocabularyCard card = new(frontHtml, null, null, backHtml);
            cards.Add(card);
        }

        private static void WriteWordCards(IEnumerable<Word> wordList, ICollection<VocabularyCard> cards)
        {
            foreach (Word word in wordList)
            {
                string? furigana = null;
                if (word.HasKanji)
                {
                    StringBuilder builder = new();
                    foreach (WordElement element in word.Elements)
                        builder.Append(element.IsKanji ? $" {element.Text}[{element.Ruby}]" : element.Text);
                    furigana = builder.ToString().Trim();
                }

                VocabularyCard card = new(word.Text, furigana, word.Extra, word.Translation);
                cards.Add(card);
            }
        }
    }
}
