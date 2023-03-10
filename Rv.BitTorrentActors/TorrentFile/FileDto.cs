using System.Collections.Generic;

namespace Rv.BitTorrentActors.TorrentFile;

public class FileDto
{
    public List<string> Path { get; set; }
    public long? Length { get; set; }

    public FileDto()
    {
        Path = new List<string>();
    }
}
