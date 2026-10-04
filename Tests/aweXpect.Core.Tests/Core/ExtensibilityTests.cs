using System.Reflection;
using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Core;

public sealed class ExtensibilityTests
{
	[Theory]
	[InlineData(typeof(ExpectationBuilder))]
	[InlineData(typeof(EquivalencyExpectationBuilder))]
	public async Task BuildersWithInternalAbstractMembers_ShouldNotBeDerivableOutsideCore(Type type)
	{
		ConstructorInfo[] constructors =
			type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		await That(constructors).All()
			.Satisfy(constructor => constructor.IsAssembly || constructor.IsFamilyAndAssembly || constructor.IsPrivate)
			.Because("a derived class outside aweXpect.Core could not implement the internal abstract members");
	}

	[Theory]
	[InlineData("aweXpect.Options.EnumerableQuantifier")]
	[InlineData("aweXpect.QuantifiedCollectionConstraint`2")]
	[InlineData("aweXpect.Options.RepeatedCheckOptions")]
	[InlineData("aweXpect.Results.RepeatedCheckResult`2")]
	[InlineData("aweXpect.Results.ObjectCountResult`3")]
	[InlineData("aweXpect.QuantifierExtensions")]
	[InlineData("aweXpect.ObjectEqualityOptionsExtensions")]
	public async Task TypesForExtensions_ShouldBeDeclaredInCore(string typeName)
	{
		Type? type = typeof(ExpectationBuilder).Assembly.GetType(typeName);

		await That(type).IsNotNull()
			.Because("an extension references only aweXpect.Core, and the name avoids a clash with a released aweXpect that still declares the type");
	}
}
