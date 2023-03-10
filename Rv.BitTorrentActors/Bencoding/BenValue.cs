using System.IO;

namespace Rv.BitTorrentActors.Bencoding;

public abstract class BenValue
{
    public abstract void Encode(Stream stream);
}
