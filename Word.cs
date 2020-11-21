using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TaeKimReader
{
    public class Word
    {
        private static readonly Regex WordExtraInfo = new(@"(\\(\w+(-\w+)?\\))", RegexOptions.Compiled);

        public IReadOnlyList<WordElement> Elements { get; }
        public string? Extra { get; }
        public string Translation { get; }
        public string Text { get; }

        public Word(string wordAndInfo, string translation)
        {
            Elements = ReadElements(wordAndInfo);
            Extra = ReadExtra(wordAndInfo);
            Translation = translation;
            Text = string.Join("", Elements.Select(e => e.Text));
        }

        private static List<WordElement> ReadElements(string wordAndInfo)
        {
            List<WordElement> wordElements = new();

            string[] tokens = wordAndInfo.Split("【");
            if (tokens.Length > 1)
            {
                string kana = tokens[0];
                string fragments = tokens[1].Split("】")[0];
                PopulateWordElements(kana, fragments, wordElements);
            }
            else
                PopulateNoFuriganaElements(wordAndInfo, wordElements);

            return wordElements;
        }

        private static void PopulateNoFuriganaElements(string wordAndInfo, List<WordElement> elements)
        {
            string text = wordAndInfo;
            if (wordAndInfo.Contains("("))
                text = wordAndInfo.Split("(")[0];

            elements.Add(new WordElement(text));
        }

        private static void PopulateWordElements(string kana, string fragments, List<WordElement> elements)
        {
            int kanaIndex = 0;
            foreach (string fragment in fragments.Split('・'))
            {
                char kanaChar = kana[kanaIndex];

                if (Kana.Hiragana.Contains(kanaChar))
                {
                    ReadHiraganaComponent(kana, fragment, elements, ref kanaIndex);
                    continue;
                }

                WordElement word = new(kanaChar.ToString(), fragment);
                elements.Add(word);

                kanaIndex++;
            }
        }

        private static void ReadHiraganaComponent(string kana, string fragment, List<WordElement> elements,
            ref int kanaIndex)
        {
            string kanaSubstring = kana.Substring(kanaIndex, fragment.Length);
            if (kanaSubstring != fragment)
                throw new Exception($"In {kana}, expected {fragment} to be found, but it was not");

            WordElement word = new(fragment);
            elements.Add(word);

            kanaIndex += fragment.Length;
        }

        private static string? ReadExtra(string wordAndInfo)
        {
            Match match = WordExtraInfo.Match(wordAndInfo);
            return match.Success ? match.Value : null;
        }

        public override string ToString() => Extra != null ? $"{Text} ({Extra}) = {Translation}" : $"{Text} = {Translation}";
    }
}
