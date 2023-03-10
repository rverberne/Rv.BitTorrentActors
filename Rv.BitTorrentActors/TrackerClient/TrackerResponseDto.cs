using System.Collections.Generic;

namespace Rv.BitTorrentActors.TrackerClient;

public class TrackerResponseDto
{
    public int? Interval { get; set; }
    public List<PeerDto> Peers { get; set; }

    public TrackerResponseDto()
    {
        Peers = new List<PeerDto>();
    }
}
