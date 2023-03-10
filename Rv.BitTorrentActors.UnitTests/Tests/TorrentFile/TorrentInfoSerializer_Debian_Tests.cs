using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rv.BitTorrentActors.TorrentFile;
using Rv.BitTorrentActors.UnitTests.Utils;
using System.IO;

namespace Rv.BitTorrentActors.UnitTests.Tests.TorrentFile;

[TestClass]
public class TorrentInfoSerializer_Debian_Tests
{
    #nullable disable
    private Stream torrentFile;
    private TorrentFileReader reader;
    #nullable enable

    [TestInitialize]
    public void SetUp()
    {
        torrentFile = EmbeddedResources.GetResourceStream(typeof(TorrentInfoSerializer_Debian_Tests).Assembly, "debian-9.4.0-amd64-netinst.iso.torrent");
        reader = new TorrentFileReader();
    }

    [TestCleanup]
    public void TearDown()
    {
        torrentFile.Dispose();
    }

    [TestMethod]
    public void WhenTorrentFileHasAnnounce_ThenItIsDeserialized()
    {
        // Act
        MetaInfoDto torrentInfo = reader.Deserialize(torrentFile);

        // Assert
        Assert.AreEqual("http://bttracker.debian.org:6969/announce", torrentInfo.Announce);
    }

    [TestMethod]
    public void WhenTorrentFileHasComment_ThenItIsDeserialized()
    {
        // Act
        MetaInfoDto torrentInfo = reader.Deserialize(torrentFile);

        // Assert
        Assert.AreEqual("\"Debian CD from cdimage.debian.org\"", torrentInfo.Comment);
    }

    [TestMethod]
    public void WhenTorrentFileHasCreationDate_ThenItIsDeserialized()
    {
        // Act
        MetaInfoDto torrentInfo = reader.Deserialize(torrentFile);

        // Assert
        Assert.AreEqual(1520682848, torrentInfo.CreationDate);
    }

    [TestMethod]
    public void WhenTorrentFileHasOneFile_ThenItIsDeserialized()
    {
        // Act
        MetaInfoDto torrentInfo = reader.Deserialize(torrentFile);

        // Assert
        Assert.AreEqual("debian-9.4.0-amd64-netinst.iso", torrentInfo.Name);
        Assert.AreEqual(305135616, torrentInfo.Length);
    }

    [TestMethod]
    public void WhenTorrentHasPieceLength_ThenItIsDeserialized()
    {
        // Act
        MetaInfoDto torrentInfo = reader.Deserialize(torrentFile);

        // Assert
        Assert.AreEqual(262144, torrentInfo.PieceLength);
    }

    [TestMethod]
    public void WhenTorrentFileHasPiecesStringOfLength23280_ThenTheyAreDeserializedIn1164Pieces()
    {
        // Act
        MetaInfoDto torrentInfo = reader.Deserialize(torrentFile);

        // Assert
        Assert.AreEqual(1164, torrentInfo.Pieces.Count);
    }

    [TestMethod]
    public void WhenTorrentFileIsDeserialized_ThenTheInfoHashShouldBeCorrect()
    {
        // Act
        MetaInfoDto torrentInfo = reader.Deserialize(torrentFile);

        // Assert
        byte[] expectedInfoHash =
        {
            116, 49, 169, 105, 179, 71, 225, 75, 186, 100,
            27, 53, 23, 192, 36, 247, 180, 13, 251, 127
        };

        CollectionAssert.AreEqual(expectedInfoHash, torrentInfo.InfoHash);
    }
}
