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
        int firstByte = stream.ReadByte();

        if (firstByte == 'i')
            return ReadInteger(stream);

        if (firstByte >= '0' && firstByte <= '9')
            return ReadByteString(firstByte, stream);

        if (firstByte == 'l')
            return ReadList(stream);

        if (firstByte == 'd')
            return ReadDictionary(stream);
        
        if (firstByte == 'e')
            return BenEndToken.Instance;

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
            BenValue listItem = Parse(stream);

            if (listItem is BenEndToken)
                break;
            
            list.Add(listItem);
        }

        return new BenList(list);
    }

    private BenDictionary ReadDictionary(Stream stream)
    {
        var dict = new Dictionary<BenByteString, BenValue>();

        while (true)
        {
            BenValue key = Parse(stream);

            if (key is BenEndToken)
                break;
            
            BenValue value = Parse(stream);
            dict.Add((BenByteString)key, value);
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
