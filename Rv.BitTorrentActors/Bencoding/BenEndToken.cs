using System;
using System.IO;

namespace Rv.BitTorrentActors.Bencoding;

public class BenEndToken : BenValue
{
    public static readonly BenEndToken Instance = new BenEndToken();

    public override void Encode(Stream stream)
    {
        throw new NotImplementedException();
    }
}
