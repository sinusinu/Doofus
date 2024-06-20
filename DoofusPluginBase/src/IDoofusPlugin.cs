namespace Doofus;

public interface IDoofusPlugin {
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public string? Argument { get; }

    public void Initialize(IDoofusEngineBridge engine);
    public Task DoActionAsync(IDoofusEngineBridge engine, string? argument);
}
