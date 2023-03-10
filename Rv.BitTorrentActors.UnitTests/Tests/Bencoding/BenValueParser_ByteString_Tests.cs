using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Rv.BitTorrentActors.Bencoding;

namespace Rv.BitTorrentActors.UnitTests.Tests.Bencoding;

[TestClass]
public class BenValueParser_ByteString_Tests
{
    #nullable disable
    private BenValueParser parser;
    #nullable enable

    [TestInitialize]
    public void SetUp()
    {
        parser = new BenValueParser();
    }

    [TestMethod]
    [DataRow("1:a", "a")]
    [DataRow("5:Hello", "Hello")]
    [DataRow("2::x", ":x")]
    public void WhenStreamIsValidByteString_ThenByteStringIsCorrectlyParsed(string streamContent, string expectedValue)
    {
        // Act
        BenValue value = parser.Parse(streamContent);

        // Assert
        BenByteString result = (BenByteString)value;

        Assert.AreEqual(expectedValue, Encoding.UTF8.GetString(result.Value));
    }
}
