using System.Collections.Generic;
using Moq;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models.Conditions;
using VirtoCommerce.MarketplaceQuoteModule.Web;
using VirtoCommerce.StateMachineModule.Core.Models;
using VirtoCommerce.StateMachineModule.Core.Services;
using Xunit;

namespace VirtoCommerce.MarketplaceQuoteModule.Tests;

[Trait("Category", "Unit")]
public class StateMachineEntityTypesTests
{
    [Fact]
    public void Register_RegistersQuoteRequestWithItsConditionPrototype()
    {
        // Arrange
        var registered = new List<StateMachineEntityType>();
        var registrar = new Mock<IStateMachineEntityTypeRegistrar>();
        registrar.Setup(x => x.Register(It.IsAny<StateMachineEntityType>())).Callback<StateMachineEntityType>(registered.Add);

        // Act
        StateMachineEntityTypes.Register(registrar.Object);

        // Assert
        var entityType = Assert.Single(registered);
        // the id existing definitions store (legacy stateMachineTypes.addType value)
        Assert.Equal("VirtoCommerce.QuoteModule.Core.Models.QuoteRequest", entityType.Id);
        Assert.Equal("Quote request", entityType.Name);
        Assert.IsType<QuoteRequestConditionTreePrototype>(entityType.ConditionTreePrototypeFactory());
    }
}
