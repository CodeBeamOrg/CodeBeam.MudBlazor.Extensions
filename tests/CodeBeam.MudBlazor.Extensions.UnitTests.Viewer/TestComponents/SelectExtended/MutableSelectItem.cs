namespace MudExtensions.UnitTests.TestComponents;

public sealed class MutableSelectItem
{
    public MutableSelectItem(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; }

    public string Name { get; set; }
}
