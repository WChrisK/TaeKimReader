namespace TaeKimReader.Sentences
{
    public class SentenceCard
    {
        public readonly string Front;
        public readonly string BackTranslation;

        public SentenceCard(string front, string backTranslation)
        {
            Front = front;
            BackTranslation = backTranslation;
        }

        public override string ToString() => $"{Front};{BackTranslation}";

    }
}
