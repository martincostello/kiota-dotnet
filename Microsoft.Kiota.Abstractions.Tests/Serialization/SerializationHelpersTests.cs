using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions.Tests.Mocks;
using Moq;
using Xunit;

namespace Microsoft.Kiota.Abstractions.Tests.Serialization;

public class SerializationHelpersTests
{
    private const string _jsonContentType = "application/json";
    [Fact]
    public void DefensiveObject()
    {
        Assert.Throws<ArgumentNullException>(() => SerializationHelpers.Deserialize<TestEntity>(null, (Stream)null, null));
        Assert.Throws<ArgumentNullException>(() => SerializationHelpers.Deserialize<TestEntity>(_jsonContentType, (Stream)null, null));
        using var stream = new MemoryStream();
        Assert.Throws<ArgumentNullException>(() => SerializationHelpers.Deserialize<TestEntity>(_jsonContentType, stream, null));
        Assert.Throws<ArgumentNullException>(() => SerializationHelpers.Deserialize<TestEntity>(_jsonContentType, "", null));
    }
    [Fact]
    public void DefensiveObjectCollection()
    {
        Assert.Throws<ArgumentNullException>(() => SerializationHelpers.DeserializeCollection<TestEntity>(null, (Stream)null, null));
        Assert.Throws<ArgumentNullException>(() => SerializationHelpers.DeserializeCollection<TestEntity>(_jsonContentType, (Stream)null, null));
        using var stream = new MemoryStream();
        Assert.Throws<ArgumentNullException>(() => SerializationHelpers.DeserializeCollection<TestEntity>(_jsonContentType, stream, null));
        Assert.Throws<ArgumentNullException>(() => SerializationHelpers.DeserializeCollection<TestEntity>(_jsonContentType, "", null));
    }
    [Fact]
    public void DeserializesObjectWithoutReflection()
    {
        var strValue = "{'id':'123'}";
        var mockParseNode = new Mock<IParseNode>();
        mockParseNode.Setup(x => x.GetObjectValue(It.IsAny<ParsableFactory<TestEntity>>())).Returns(new TestEntity()
        {
            Id = "123"
        });
        var mockJsonParseNodeFactory = new Mock<IParseNodeFactory>();
        mockJsonParseNodeFactory.Setup(x => x.GetRootParseNode(It.IsAny<string>(), It.IsAny<Stream>())).Returns(mockParseNode.Object);
        mockJsonParseNodeFactory.Setup(x => x.ValidContentType).Returns(_jsonContentType);
        ParseNodeFactoryRegistry.DefaultInstance.ContentTypeAssociatedFactories[_jsonContentType] = mockJsonParseNodeFactory.Object;

        var result = SerializationHelpers.Deserialize(_jsonContentType, strValue, TestEntity.CreateFromDiscriminatorValue);

        Assert.NotNull(result);
    }
    [Fact]
    public void DeserializesObjectWithReflection()
    {
        var strValue = "{'id':'123'}";
        var mockParseNode = new Mock<IParseNode>();
        mockParseNode.Setup(x => x.GetObjectValue(It.IsAny<ParsableFactory<TestEntity>>())).Returns(new TestEntity()
        {
            Id = "123"
        });
        var mockJsonParseNodeFactory = new Mock<IParseNodeFactory>();
        mockJsonParseNodeFactory.Setup(x => x.GetRootParseNode(It.IsAny<string>(), It.IsAny<Stream>())).Returns(mockParseNode.Object);
        mockJsonParseNodeFactory.Setup(x => x.ValidContentType).Returns(_jsonContentType);
        ParseNodeFactoryRegistry.DefaultInstance.ContentTypeAssociatedFactories[_jsonContentType] = mockJsonParseNodeFactory.Object;

        var result = SerializationHelpers.Deserialize<TestEntity>(_jsonContentType, strValue);

        Assert.NotNull(result);
    }
    [Fact]
    public void DeserializesCollectionOfObject()
    {
        var strValue = "{'id':'123'}";
        var mockParseNode = new Mock<IParseNode>();
        mockParseNode.Setup(x => x.GetCollectionOfObjectValues(It.IsAny<ParsableFactory<TestEntity>>())).Returns(new List<TestEntity> {
            new TestEntity()
            {
                Id = "123"
            }
        });
        var mockJsonParseNodeFactory = new Mock<IParseNodeFactory>();
        mockJsonParseNodeFactory.Setup(x => x.GetRootParseNode(It.IsAny<string>(), It.IsAny<Stream>())).Returns(mockParseNode.Object);
        mockJsonParseNodeFactory.Setup(x => x.ValidContentType).Returns(_jsonContentType);
        ParseNodeFactoryRegistry.DefaultInstance.ContentTypeAssociatedFactories[_jsonContentType] = mockJsonParseNodeFactory.Object;

        var result = SerializationHelpers.DeserializeCollection(_jsonContentType, strValue, TestEntity.CreateFromDiscriminatorValue);

        Assert.NotNull(result);
        Assert.Single(result);
    }
}
