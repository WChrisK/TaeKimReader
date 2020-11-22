namespace TaeKimReader.Words
{
    public class VocabularyCard
    {
        public readonly string Front;
        public readonly string? BackFurigana;
        public readonly string? BackConjugation;
        public readonly string BackTranslation;

        public VocabularyCard(string front, string? backFurigana, string? backConjugation, string backTranslation)
        {
            Front = front;
            BackFurigana = backFurigana;
            BackConjugation = backConjugation;
            BackTranslation = backTranslation;
        }

        public override string ToString() => $"{Front};{BackFurigana?.Trim()};{BackConjugation};{BackTranslation}";
    }
}
