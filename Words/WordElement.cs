namespace TaeKimReader.Words
{
    public class WordElement
    {
        public readonly string Text;
        public readonly string? Ruby;

        public bool IsKanji => Ruby != null && Ruby != Text;

        public WordElement(string text, string? ruby = null)
        {
            Text = text;
            Ruby = ruby;
        }

        public override string ToString() => Ruby != null ? $"{Text} ({Ruby})" : Text;
    }
}
