using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Rv.BitTorrentActors.Bencoding;

public class BenDictionary : BenValue
{
    public IDictionary<BenByteString, BenValue> Values { get; }

    public BenDictionary(Dictionary<BenByteString, BenValue> values)
    {
        Values = new SortedDictionary<BenByteString, BenValue>(values);
    }

    public bool HasString(string key)
    {
        return (Get<BenByteString>(key) is BenByteString);
    }

    public string GetString(string key)
    {
        return Get<BenByteString>(key).AsString;
    }

    public string? GetStringOrDefault(string key)
    {
        string? result = TryGetString(key, out string? s)
            ? s
            : default;
        return result;
    }

    public bool TryGetString(string key, [NotNullWhen(returnValue: true)] out string? result)
    {
        if (Values.TryGetValue(new BenByteString(key), out BenValue? value) && value is BenByteString byteString)
        {
            result = byteString.AsString;
            return true;
        }
        
        result = null;
        return false;
    }

    public byte[] GetStringBytes(string key)
    {
        return Get<BenByteString>(key).Value;
    }

    public bool TryGetStringBytes(string key, [NotNullWhen(returnValue: true)] out byte[]? result)
    {
        if (Values.TryGetValue(new BenByteString(key), out BenValue? value) && value is BenByteString byteString)
        {
            result = byteString.Value;
            return true;
        }
        
        result = null;
        return false;
    }

    public bool HasInt(string key)
    {
        return (Get<BenInteger>(key) is BenInteger);
    }

    public long GetInt(string key)
    {
        return Get<BenInteger>(key).Value;
    }

    public long? GetIntOrDefault(string key)
    {
        long? result = TryGetInt(key, out long? i)
            ? i
            : default;
        return result;
    }

    public bool TryGetInt(string key, [NotNullWhen(returnValue: true)] out long? result)
    {
        if (Values.TryGetValue(new BenByteString(key), out BenValue? value) && value is BenInteger integer)
        {
            result = integer.Value;
            return true;
        }
        
        result = null;
        return false;
    }

    public bool HasList(string key)
    {
        return GetOrDefault<BenList>(key) is not null;
    }

    public BenList GetList(string key)
    {
        return Get<BenList>(key);
    }

    public BenList? GetListOrDefault(string key)
    {
        BenList? result = TryGetList(key, out BenList? l)
            ? l
            : default;
        return result;
    }

    public bool TryGetList(string key, [NotNullWhen(returnValue: true)] out BenList? result)
    {
        if (Values.TryGetValue(new BenByteString(key), out BenValue? value) && value is BenList list)
        {
            result = list;   
            return true;
        }
        
        result = null;
        return false;
    }
    
    public bool HasDictionary(string key)
    {
        return (Get<BenDictionary>(key) is BenDictionary);
    }

    public BenDictionary GetDictionary(string key)
    {
        return Get<BenDictionary>(key);
    }

    public BenDictionary? GetDictionaryOrDefault(string key)
    {
        BenDictionary? result = TryGetDictionary(key, out BenDictionary? d)
            ? d
            : default;
        return result;
    }

    public bool TryGetDictionary(string key, [NotNullWhen(returnValue: true)] out BenDictionary? result)
    {
        if (Values.TryGetValue(new BenByteString(key), out BenValue? value) && value is BenDictionary dictionary)
        {
            result = dictionary;   
            return true;
        }
        
        result = null;
        return false;
    }

    public T Get<T>(string key) where T : BenValue
    {
        if (!Values.TryGetValue(new BenByteString(key), out BenValue? benValue))
            throw new InvalidOperationException($"Key '{key}' not found.");

        if (benValue is not T)
            throw new InvalidOperationException($"Value is not of type '{typeof(T).Name}', got '{benValue.GetType().Name}'.");

        return (T)benValue;
    }

    public T? GetOrDefault<T>(string key) where T : BenValue
    {
        if (!Values.TryGetValue(new BenByteString(key), out BenValue? benValue))
            return null;

        if (benValue is not T)
            return null;

        return (T)benValue;
    }

    #region Shortcuts

    public List<string> GetListOfStrings(string key)
    {
        return (Get<BenList>(key) is BenList list)
            ? list.OfType<BenByteString>().Select(s => s.AsString).ToList()
            : new List<string>();
    }

    #endregion

    public override void Encode(Stream stream)
    {
        stream.WriteByte((byte)'d');

        foreach (var kvp in Values)
        {
            kvp.Key.Encode(stream);
            kvp.Value.Encode(stream);
        }

        stream.WriteByte((byte)'e');
    }
}
