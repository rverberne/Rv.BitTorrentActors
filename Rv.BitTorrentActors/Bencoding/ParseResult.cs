namespace Rv.BitTorrentActors.Bencoding;

public abstract class ParseResult
{
}

public class ParsedValue : ParseResult
{
    public BenValue Value { get; }

    public ParsedValue(BenValue value)
    {
        Value = value;
    }
}

public class ParsedEndToken : ParseResult
{
}
