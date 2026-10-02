using System.Reflection;
using aweXpect.Equivalency;
using aweXpect.Options;
using aweXpect.Results;

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
	[InlineData(typeof(EnumerableQuantifier))]
	[InlineData(typeof(QuantifiedCollectionConstraint<,>))]
	[InlineData(typeof(RepeatedCheckOptions))]
	[InlineData(typeof(RepeatedCheckResult<,>))]
	[InlineData(typeof(ObjectCountResult<,,>))]
	[InlineData(typeof(ObjectCountResult<,,,>))]
	public async Task TypesForExtensions_ShouldBeDeclaredInCore(Type type)
	{
		await That(type.Assembly).IsSameAs(typeof(ExpectationBuilder).Assembly)
			.Because("an extension references only aweXpect.Core");
	}
}
