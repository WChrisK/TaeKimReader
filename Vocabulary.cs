using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TaeKimReader
{
    public class Vocabulary : IEnumerable<KeyValuePair<string, Word>>
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

        public IEnumerator<KeyValuePair<string, Word>> GetEnumerator() => dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
