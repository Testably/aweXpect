using System.Reflection;
using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Core;

public sealed class ExtensibilityTests
{
	[Test]
	[Arguments(typeof(ExpectationBuilder))]
	[Arguments(typeof(EquivalencyExpectationBuilder))]
	public async Task BuildersWithInternalAbstractMembers_ShouldNotBeDerivableOutsideCore(Type type)
	{
		ConstructorInfo[] constructors =
			type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		await That(constructors).All()
			.Satisfy(constructor => constructor.IsAssembly || constructor.IsFamilyAndAssembly || constructor.IsPrivate)
			.Because("a derived class outside aweXpect.Core could not implement the internal abstract members");
	}

	[Test]
	[Arguments("aweXpect.Options.EnumerableQuantifier")]
	[Arguments("aweXpect.QuantifiedCollectionConstraint`2")]
	[Arguments("aweXpect.Options.RepeatedCheckOptions")]
	[Arguments("aweXpect.Results.RepeatedCheckResult`2")]
	[Arguments("aweXpect.Results.ObjectCountResult`3")]
	[Arguments("aweXpect.QuantifierExtensions")]
	[Arguments("aweXpect.ObjectEqualityOptionsExtensions")]
	public async Task TypesForExtensions_ShouldBeDeclaredInCore(string typeName)
	{
		Type? type = typeof(ExpectationBuilder).Assembly.GetType(typeName);

		await That(type).IsNotNull()
			.Because("an extension references only aweXpect.Core, and the name avoids a clash with a released aweXpect that still declares the type");
	}
}
