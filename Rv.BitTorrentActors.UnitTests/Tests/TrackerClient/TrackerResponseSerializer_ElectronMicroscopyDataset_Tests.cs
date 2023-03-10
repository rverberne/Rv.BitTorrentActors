using System.IO;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rv.BitTorrentActors.TrackerClient;
using Rv.BitTorrentActors.UnitTests.Utils;

namespace Rv.BitTorrentActors.UnitTests.Tests.TrackerClient;

[TestClass]
public class TrackerResponseSerializer_ElectronMicroscopyDataset_Tests
{
    #nullable disable
    private Stream bencoding;
    private TrackerResponseSerializer serializer;
    #nullable enable
    
    [TestInitialize]
    public void SetUp()
    {
        bencoding = EmbeddedResources.GetResourceStream(typeof(TrackerResponseSerializer_ElectronMicroscopyDataset_Tests).Assembly, "Electron Microscopy (CA1 hippocampus) Dataset.trackerResponse");
        serializer = new TrackerResponseSerializer();
    }

    [TestCleanup]
    public void TearDown()
    {
        bencoding.Dispose();
    }

    [TestMethod]
    public void WhenTrackerResponseHasInterval_ThenItIsDeserialized()
    {
        // Act
        TrackerResponseDto response = serializer.Deserialize(bencoding);

        // Assert
        Assert.AreEqual(30, response.Interval);
    }

    [TestMethod]
    public void WhenTrackerResponseContainsPeersAsListOfDictionary_ThenTheyAreDeserialized()
    {
        // Act
        TrackerResponseDto response = serializer.Deserialize(bencoding);

        // Assert
        PeerDto peer1 = response.Peers[0];
        PeerDto peer2 = response.Peers[1];
        PeerDto peer3 = response.Peers[2];

        Assert.AreEqual(3, response.Peers.Count);
        Assert.AreEqual(new IPAddress(new byte[] { 209, 181, 242, 180 }), peer1.Ip);
        Assert.AreEqual(16881, peer1.Port);

        Assert.AreEqual(new IPAddress(new byte[] { 70, 57, 182, 176 }), peer2.Ip);
        Assert.AreEqual(33761, peer2.Port);

        Assert.AreEqual(new IPAddress(new byte[] { 185, 149, 90, 114 }), peer3.Ip);            
        Assert.AreEqual(51033, peer3.Port);
    }
}
