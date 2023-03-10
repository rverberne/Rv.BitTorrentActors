using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Rv.BitTorrentActors.Bencoding;

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
        long pos = stream.Position;
        BenValueKind valueKind = ReadValueType(stream);
        stream.Seek(pos, SeekOrigin.Begin);

        switch (valueKind)
        {
            case BenValueKind.Integer:
                return ReadInteger(stream);
            case BenValueKind.ByteString:
                return ReadByteString(stream);
            case BenValueKind.List:
                return ReadList(stream);
            case BenValueKind.Dictionary:
                return ReadDictionary(stream);
            default:
                throw new InvalidOperationException("Unknown value kind.");
        }
    }

    private BenInteger ReadInteger(Stream stream)
    {
        int[] bytes = ReadUntilIncluding(stream, 'e');

        if (!(bytes.Length >= 3))
            throw new InvalidOperationException("Integer value should consist of atleast 3 bytes.");

        if (!IsIntegerStart(bytes.First()))
            throw new InvalidOperationException("Integer value should start with 'i'.");

        if (!IsIntegerEnd(bytes.Last()))
            throw new InvalidOperationException("Integer value should end with 'e'.");

        string stringValue = new string(
            bytes[1..^1]
                .Select(b => (char)b)
                .ToArray());
        long intVal = long.TryParse(stringValue, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out long v)
            ? v
            : throw new InvalidOperationException("Could not parse the integer value.");
        return new BenInteger(intVal);
    }

    private bool IsIntegerStart(int byteValue)
    {
        bool result = byteValue == 'i';
        return result;
    }

    private bool IsIntegerEnd(int byteValue)
    {
        bool result = byteValue == 'e';
        return result;
    }

    private BenByteString ReadByteString(Stream stream)
    {
        int[] lengthBytesIncludingColon = ReadUntilIncluding(stream, ':');

        if (!(lengthBytesIncludingColon.Length >= 2))
            throw new InvalidOperationException("Byte string value should start with atleast one number and a colon.");

        if (!(lengthBytesIncludingColon.Last() == ':'))
            throw new InvalidOperationException("Byte string length should end with a colon.");

        int[] lengthBytes = lengthBytesIncludingColon[..^1];

        if (!lengthBytes.All(b => IsNumberChar(b)))
            throw new InvalidOperationException("Byte string length should consist of only numbers.");
        
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
        if (!IsListStart(stream.ReadByte()))
            throw new InvalidOperationException("List should start with 'l'.");
        
        var list = new List<BenValue>();

        while (true)
        {
            int currByte = stream.ReadByte();

            if (currByte == -1)
                throw new InvalidOperationException("Unexpected end of stream.");
            
            if (IsListEnd(currByte))
                break;
            
            stream.Seek(-1, SeekOrigin.Current);

            BenValue listItem = Parse(stream);
            list.Add(listItem);
        }

        return new BenList(list);
    }

    private bool IsListStart(int value)
    {
        bool result = value == 'l';
        return result;
    }

    private bool IsListEnd(int value)
    {
        bool result = value == 'e';
        return result;
    }

    private BenDictionary ReadDictionary(Stream stream)
    {
        if (!IsDictionaryStart(stream.ReadByte()))
            throw new InvalidOperationException("Dictionary should start with 'd'.");
        
        var dict = new Dictionary<BenByteString, BenValue>();

        while (true)
        {
            int byteValue = stream.ReadByte();

            if (byteValue == -1)
                throw new InvalidOperationException("Unexpected end of stream.");

            if (IsDictionaryEnd(byteValue))
                break;
            
            stream.Seek(-1, SeekOrigin.Current);

            BenByteString key = (BenByteString)Parse(stream);
            BenValue value = Parse(stream);
            dict.Add(key, value);
        }

        return new BenDictionary(dict);
    }

    private bool IsDictionaryStart(int value)
    {
        bool result = value == 'd';
        return result;
    }

    private bool IsDictionaryEnd(int value)
    {
        bool result = value == 'e';
        return result;
    }

    private BenValueKind ReadValueType(Stream stream)
    {
        int firstByte = stream.ReadByte();

        if (firstByte == 'i')
            return BenValueKind.Integer;
        
        if (IsListStart(firstByte))
            return BenValueKind.List;
        
        if (IsDictionaryStart(firstByte))
            return BenValueKind.Dictionary;
        
        if (IsNumberChar(firstByte))
            return BenValueKind.ByteString;
        
        throw new InvalidOperationException("Could not detect value kind.");

    }

    private int[] ReadUntilIncluding(Stream stream, char terminator)
    {
        var intBytes = new List<int>();

        while (true)
        {
            int currByte = stream.ReadByte();

            if (currByte == terminator)
            {
                intBytes.Add(currByte);
                break;
            }
            else if (currByte == -1)
            {
                throw new InvalidOperationException("Unexpected end of stream.");
            }
            else
            {
                intBytes.Add(currByte);
            }
        }

        return intBytes.ToArray();
    }

    private bool IsNumberChar(int charByte)
    {
        switch (charByte)
        {
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                return true;
            default:
                return false;
        }
    }
}
