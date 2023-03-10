using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Rv.BitTorrentActors.Bencoding;

namespace Rv.BitTorrentActors.TrackerClient;

public class TrackerResponseSerializer
{
    public TrackerResponseDto Deserialize(Stream bencoding)
    {
        var bencodingParser = new BenValueParser();
        BenDictionary responseDict = (BenDictionary)bencodingParser.Parse(bencoding);

        TrackerResponseDto result = new TrackerResponseDto();
        result.Interval = ReadInterval(responseDict, result);
        result.Peers.AddRange(ReadPeers(responseDict));
        return result;
    }

    private int? ReadInterval(BenDictionary responseDict, TrackerResponseDto response)
    {
        int? result = responseDict.HasInt("interval")
            ? (int)responseDict.GetInt("interval")
            : null;
        return result;
    }

    private List<PeerDto> ReadPeers(BenDictionary responseDict)
    {
        if (responseDict.TryGetList("peers", out BenList? _))
            return ReadPeersFromListOfDicts(responseDict);
        
        if (responseDict.HasString("peers"))
            return ReadPeersFromString(responseDict);
        
        throw new InvalidOperationException("Invalid tracker response, peers are missing.");
    }

    private List<PeerDto> ReadPeersFromListOfDicts(BenDictionary responseDict)
    {
        var result = (responseDict.GetListOrDefault("peers") ?? new BenList()).Values
            .OfType<BenDictionary>()
            .Select(d => ReadPeerFromDict(d))
            .ToList();
        return result;
    }

    private PeerDto ReadPeerFromDict(BenDictionary peerDict)
    {
        var result = new PeerDto
        {
            Ip = IPAddress.Parse(peerDict.GetString("ip")),
            Port = (int)peerDict.GetInt("port"),
            PeerId = peerDict.GetStringBytes("peer id"),
        };
        return result;
    }

    private List<PeerDto> ReadPeersFromString(BenDictionary responseDict)
    {
        byte[] peersBytes = responseDict.GetStringBytes("peers");
        var result = Enumerable
            .Range(0, peersBytes.Length / 6)
            .Select(i => peersBytes.Skip(i * 6).Take(6).ToArray())
            .Select(p => ReadPeerFromSixBytes(p))
            .ToList();
        return result;
    }

    private PeerDto ReadPeerFromSixBytes(byte[] peerBytes)
    {
        byte[] ipBytes = peerBytes.Take(4).ToArray();
        byte[] portBytes = peerBytes.Skip(4).Take(2).ToArray();

        var result = new PeerDto
        {
            Ip = new IPAddress(ipBytes),
            Port = (portBytes[0] << 8) | (portBytes[1]),
            PeerId = new byte[20],
        };
        return result;
    }
}
