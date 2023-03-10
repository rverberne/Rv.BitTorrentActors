using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Rv.BitTorrentActors.Bencoding;

namespace Rv.BitTorrentActors.TorrentFile;

// https://en.wikipedia.org/wiki/Torrent_file
// https://fbdtemme.github.io/torrenttools/topics/bittorrent-metafile-v1.html
// https://wiki.theory.org/BitTorrentSpecification#Metainfo_File_Structure
public class TorrentFileReader
{
    public MetaInfoDto Deserialize(Stream bencoding)
    {
        var bencodingParser = new BenValueParser();
        BenDictionary dict = (BenDictionary)bencodingParser.Parse(bencoding);

        var result = new MetaInfoDto();
        result.Announce = dict.GetStringOrDefault("announce");
        result.Comment = dict.GetStringOrDefault("comment");
        result.CreationDate = dict.GetIntOrDefault("creation date");
        result.CreatedBy = dict.GetStringOrDefault("created by");

        if (dict.GetDictionaryOrDefault("info") is BenDictionary info)
        {
            result.PieceLength = info.GetIntOrDefault("piece length");
            result.Pieces.AddRange(ReadPieces(info));
            result.Private = info.GetIntOrDefault("private");
            result.Name = info.GetStringOrDefault("name");
            result.Length = info.GetIntOrDefault("length");
            result.Files.AddRange(ReadFiles(info));

            using (var stream = new MemoryStream())
            using (var sha1 = SHA1.Create())
            {
                info.Encode(stream);
                byte[] infoDictBytes = stream.ToArray();

                result.InfoHash = sha1.ComputeHash(infoDictBytes);
            }
        }

        return result;
    }

    private List<byte[]> ReadPieces(BenDictionary info)
    {
        if (!info.TryGetStringBytes("pieces", out byte[]? pieces))
            return new List<byte[]>();
        
        List<byte[]> result = Enumerable
            .Range(0, pieces.Length / 20)
            .Select(i => pieces.Skip(i * 20).Take(20).ToArray())
            .ToList();
        return result;
    }

    private List<FileDto> ReadFiles(BenDictionary info)
    {
        List<FileDto> files = (info.GetListOrDefault("files") ?? new BenList())
            .OfType<BenDictionary>()
            .Select(f => ReadFile(f))
            .ToList();
        return files;
    }

    private FileDto ReadFile(BenDictionary file)
    {
        var result = new FileDto();
        result.Length = file.GetIntOrDefault("length");
        result.Path.AddRange((file.GetListOrDefault("path") ?? new BenList())
            .OfType<BenByteString>()
            .Select(s => s.AsString)
            .ToList());
        return result;
    }
}
