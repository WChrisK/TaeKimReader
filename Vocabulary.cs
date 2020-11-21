using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TaeKimReader
{
    public class Vocabulary
    {
        private static readonly Regex WordRegex = new(@"\d+\. (.+)- (.+)", RegexOptions.Compiled);

        private readonly Dictionary<string, Word> dictionary = new();
        private readonly Dictionary<Version, List<Word>> chapterToWords = new();
        private List<Word> currentChapterList = new();

        public Vocabulary(string path)
        {
            foreach (string unprocessedLine in File.ReadAllLines(path, Encoding.UTF8))
            {
                string line = unprocessedLine.Trim().Replace(';', ',');
                if (line.Length == 0)
                    continue;

                if (line.StartsWith('['))
                    HandleChapter(line);
                else
                    HandleWord(line);
            }
        }

        public bool TryGetValue(string kana, [MaybeNullWhen(false)] out Word? word)
        {
            return dictionary.TryGetValue(kana, out word);
        }

        private void HandleChapter(string line)
        {
            Version version = new(line[1..^1]);
            currentChapterList = new List<Word>();
            chapterToWords[version] = currentChapterList;
        }

        private void HandleWord(string line)
        {
            Match match = WordRegex.Match(line);
            if (!match.Success)
                throw new Exception($"Unexpected vocabulary word from line: {line}");

            string wordAndInfo = match.Groups[1].Value.Trim();
            string translation = match.Groups[2].Value.Trim();
            Word word = new(wordAndInfo, translation);
            currentChapterList.Add(word);

            if (!dictionary.ContainsKey(word.Text))
                dictionary.Add(word.Text, word);
        }

        public IEnumerable<(Version, ReadOnlyCollection<Word>)> GetSortedVersionDictionary()
        {
            return chapterToWords.Select(pair => (pair.Key, pair.Value.AsReadOnly()))
                                 .OrderBy(t => t.Key)
                                 .ToList();
        }

        public void WriteToFile(string path)
        {
            using StreamWriter w = new(File.OpenWrite(path));
            foreach (var (version, words) in GetSortedVersionDictionary())
            {
                w.WriteLine(version);
                foreach (Word word in words)
                    w.WriteLine($"    {word}");
            }
        }
    }
}
