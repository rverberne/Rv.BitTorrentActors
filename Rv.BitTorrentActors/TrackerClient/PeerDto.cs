using System.Net;

namespace Rv.BitTorrentActors.TrackerClient;

public class PeerDto
{
    public IPAddress? Ip { get; set; }
    public int? Port { get; set; }
    public byte[]? PeerId { get; set; }
}