using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Rv.BitTorrentActors.Bencoding;

// https://wiki.theory.org/BitTorrentSpecification#Bencoding
public class BenValueParser
{
    public BenValue Parse(string value)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(value));
        BenValue result = Parse(stream);
        return result;
    }

    public BenValue Parse(Stream stream)
    {
        BenValue result = ParseInternal(stream) switch
        {
            ParsedValue r => r.Value,
            _ => throw new InvalidOperationException("Invalid bencode stream,")
        };
        return result;
    }

    private ParseResult ParseInternal(Stream stream)
    {
        int firstByte = stream.ReadByte();
        
        if (firstByte == 'i')
            return new ParsedValue(ReadInteger(stream));

        if (firstByte >= '0' && firstByte <= '9')
            return new ParsedValue(ReadByteString(firstByte, stream));

        if (firstByte == 'l')
            return new ParsedValue(ReadList(stream));

        if (firstByte == 'd')
            return new ParsedValue(ReadDictionary(stream));
        
        if (firstByte == 'e')
            return new ParsedEndToken();

        throw new InvalidOperationException("Could not detect value kind.");
    }

    private BenInteger ReadInteger(Stream stream)
    {
        int[] bytes = ReadUntilIncluding(stream, 'e')[..^1];

        string valueAsString = new string(
            bytes
                .Select(b => (char)b)
                .ToArray());

        long valueAsLong = long.TryParse(valueAsString, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out long v)
            ? v
            : throw new InvalidOperationException("Could not parse the integer value.");
        
        return new BenInteger(valueAsLong);
    }

    private BenByteString ReadByteString(int firstByte, Stream stream)
    {
        int[] lengthBytes = ReadUntilIncluding(stream, ':')[..^1].Prepend(firstByte).ToArray();

        string lengthString = new string(
            lengthBytes
                .Select(b => (char)b)
                .ToArray());

        int length = int.TryParse(lengthString, NumberStyles.None, CultureInfo.InvariantCulture, out int v)
            ? v
            : throw new InvalidOperationException("Byte string length could not be parsed.");

        byte[] bytes = new byte[length];
        stream.Read(bytes, 0, length);
        return new BenByteString(bytes);
    }

    private BenList ReadList(Stream stream)
    {
        var list = new List<BenValue>();

        while (true)
        {
            ParseResult parsedElement = ParseInternal(stream);

            if (parsedElement is ParsedEndToken)
                break;
            
            if (parsedElement is ParsedValue { Value: BenValue value })
                list.Add(value);
        }

        return new BenList(list);
    }

    private BenDictionary ReadDictionary(Stream stream)
    {
        var dict = new Dictionary<BenByteString, BenValue>();

        while (true)
        {
            ParseResult parsedKey = ParseInternal(stream);

            if (parsedKey is ParsedEndToken)
                break;

            if (!(parsedKey is ParsedValue { Value: BenByteString key }))
                throw new InvalidOperationException("Dictionary key should be a string.");
            
            if (!(ParseInternal(stream) is ParsedValue { Value: BenValue value }))
                throw new InvalidOperationException("Unexpected dictionary value.");
            
            dict.Add(key, value);
        }

        return new BenDictionary(dict);
    }

    private int[] ReadUntilIncluding(Stream stream, char terminator)
    {
        var bytes = new List<int>();

        while (true)
        {
            int currByte = stream.ReadByte();

            if (currByte == terminator)
            {
                bytes.Add(currByte);
                break;
            }

            if (currByte == -1)
                throw new InvalidOperationException("Unexpected end of stream.");

            bytes.Add(currByte);
        }

        return bytes.ToArray();
    }
}
