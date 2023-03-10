using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rv.BitTorrentActors.Bencoding;
using System.Text;

namespace Rv.BitTorrentActors.UnitTests.Tests.Bencoding;

[TestClass]
public class BenValueParser_List_Tests
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
    public void WhenStreamIsValidList_ThenListIsCorrectlyParsed()
    {
        // Act
        BenValue value = parser.Parse("l3:Fooi42ee");

        // Assert
        BenList result = (BenList)value;

        BenByteString elementOne = (BenByteString)result.Values[0];
        BenInteger elementTwo = (BenInteger)result.Values[1];

        Assert.AreEqual("Foo", Encoding.UTF8.GetString(elementOne.Value));
        Assert.AreEqual(42, elementTwo.Value);
    }

    [TestMethod]
    public void WhenListInsideList_ThenListIsCorrectlyParsed()
    {
        // Act
        BenValue value = parser.Parse("ll3:Fooee");

        // Assert
        BenList result = (BenList)value;
        BenList innerList = (BenList)result.Values[0];
        BenByteString elementTwo = (BenByteString)innerList.Values[0];

        Assert.AreEqual("Foo", Encoding.UTF8.GetString(elementTwo.Value));
    }
}
