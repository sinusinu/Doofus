namespace Doofus;

public interface IDoofusEngineBridge {
    public bool Verbose { get; }
    public string Language { get; }

    public Task<string> AskAIAsync(string systemPrompt, string[]? userPrompts, bool json = false);
    public void Write(string text, bool lineBreak = true);
}