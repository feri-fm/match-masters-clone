
[System.Serializable]
public struct Id
{
    public int value;

    public readonly static Id empty = new Id(-1);

    public Id(int value)
    {
        this.value = value;
    }
    public override string ToString()
    {
        return value.ToString();
    }

    public override bool Equals(object obj)
    {
        if (obj is Id id)
            return id.value == value;
        return false;
    }
    public override int GetHashCode()
    {
        return value;
    }

    public static bool operator ==(Id left, Id right) => left.Equals(right);
    public static bool operator !=(Id left, Id right) => !left.Equals(right);
}

public class IdentifierGenerator
{
    public int lastId;
    public Id Generate() => new Id(++lastId);
    public void Clear() { lastId = 0; }

    public IdentifierGeneratorData Save() { return new IdentifierGeneratorData() { lastId = lastId }; }
    public void Load(IdentifierGeneratorData data) { lastId = data.lastId; }
}
public class IdentifierGeneratorData
{
    public int lastId;
}
