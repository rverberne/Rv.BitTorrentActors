using System.Collections.Generic;

namespace Rv.BitTorrentActors.TorrentFile;

public class MetaInfoDto
{
    public string? Announce { get; set; }
    //public List<string> AnnounceList { get; set; }
    public string? Comment { get; set; }
    public long? CreationDate { get; set; }
    public string? CreatedBy { get; set; }

    // Info Dictionary
    public long? PieceLength { get; set; }
    public List<byte[]> Pieces { get; }
    public long? Private { get; set; }
    public string? Name { get; set; }
    public long? Length { get; set; }
    public List<FileDto> Files { get; }

    // Calculated
    public byte[] InfoHash { get; set; }
    //public int TotalLength { get; set; }
    //public int NumPieces { get { return Pieces.Count; } }

    public MetaInfoDto()
    {
        Pieces = new List<byte[]>();
        Files = new List<FileDto>();
        InfoHash = new byte[0];
    }
}
