using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TaeKimReader.Words;

namespace TaeKimReader.Sentences
{
    public class SentenceExamples
    {
        private static readonly Regex EnglishSentenceRegex = new("^[-A-Za-z.]", RegexOptions.Compiled);

        private readonly Vocabulary vocabulary;
        private readonly Dictionary<Version, List<SentenceExample>> chapterToSentences = new();
        private List<SentenceExample> currentChapterList = new();
        private SentenceExampleBuilder sentenceBuilder = new();
        private List<WordElement> lastKanaSentence = new(new List<WordElement>());

        public SentenceExamples(string path, Vocabulary vocab)
        {
            vocabulary = vocab;

            foreach (string unprocessedLine in File.ReadAllLines(path, Encoding.UTF8))
            {
                bool startsWithIndent = unprocessedLine.StartsWith("    ") || unprocessedLine.StartsWith("\t");
                string line = unprocessedLine.Trim().Replace(';', ',');
                if (line.Length == 0)
                    continue;

                if (line.StartsWith('['))
                    HandleChapter(line);
                else
                    HandleSentence(line, startsWithIndent);
            }

            FinishSentenceIfNeeded();
        }

        private void FinishSentenceIfNeeded()
        {
            if (sentenceBuilder.Count > 0)
            {
                SentenceExample sentenceExample = new(sentenceBuilder);
                currentChapterList.Add(sentenceExample);
            }

            sentenceBuilder = new SentenceExampleBuilder();
        }

        private void HandleChapter(string line)
        {
            FinishSentenceIfNeeded();

            Version version = new(line[1..^1]);
            currentChapterList = new List<SentenceExample>();
            chapterToSentences[version] = currentChapterList;
        }

        private void HandleSentence(string line, bool startsWithIndent)
        {
            if (EnglishSentenceRegex.IsMatch(line))
            {
                Sentence sentence = new(lastKanaSentence, line);
                sentenceBuilder.Add(sentence);
            }
            else
            {
                if (!startsWithIndent && sentenceBuilder.Count > 0)
                    FinishSentenceIfNeeded();

                lastKanaSentence = BuildKanaSentence(line);
            }
        }

        private List<WordElement> BuildKanaSentence(string line)
        {
            List<WordElement> words = new();

            string kana = line.Contains(". ") ? line.Split(". ", 2)[1] : line;

            int index = 0;
            while (index < kana.Length)
            {
                IEnumerable<string> substringCombos = GenerateAllKanaCombos(kana, index);
                Word? longestWord = FindLongestWord(substringCombos);

                // Either we found a word, or we didn't and will just add the
                // letter. This should only happen for unexpected symbols that
                // do not have a translation, or hiragana/katakana.
                if (longestWord != null)
                    words.AddRange(longestWord.Elements);
                else
                    words.Add(new WordElement(kana[index].ToString()));

                index += longestWord?.Text.Length ?? 1;
            }

            return words;
        }

        private Word? FindLongestWord(IEnumerable<string> subWordCombos)
        {
            Word? longestWord = null;
            foreach (string subWord in subWordCombos)
                if (vocabulary.TryGetValue(subWord, out Word? word))
                    longestWord = word;

            if (longestWord != null)
                Debug.Assert(longestWord.Text.Length != 0, "Should never have an empty length word");

            return longestWord;
        }

        private static IEnumerable<string> GenerateAllKanaCombos(string kana, int index)
        {
            List<string> combos = new();

            int charsLeft = kana.Length - index;
            for (int length = 1; length < charsLeft; length++)
                combos.Add(kana.Substring(index, length));

            return combos;
        }

        public IEnumerable<(Version, ReadOnlyCollection<SentenceExample>)> GetSortedVersionDictionary()
        {
            return chapterToSentences.Select(pair => (pair.Key, pair.Value.AsReadOnly()))
                                     .OrderBy(t => t.Key)
                                     .ToList();
        }

        public void WriteToFile(string path)
        {
            using StreamWriter w = new(File.OpenWrite(path), Encoding.UTF8);
            foreach (var (version, sentenceExamples) in GetSortedVersionDictionary())
            {
                w.WriteLine(version);
                foreach (SentenceExample sentenceExample in sentenceExamples)
                {
                    w.WriteLine($"    {sentenceExample.First()}");
                    foreach (Sentence sentence in sentenceExample.Skip(1))
                        w.WriteLine($"        {sentence}");
                }
            }
        }
    }
}
