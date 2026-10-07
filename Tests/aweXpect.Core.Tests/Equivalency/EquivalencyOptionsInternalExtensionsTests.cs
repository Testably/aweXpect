using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Equivalency;

public sealed class EquivalencyOptionsInternalExtensionsTests
{
	[Test]
	public async Task GetInheritedOptions_ShouldKeepTheComparisonTypeOfTheOptions()
	{
		EquivalencyTypeOptions defaultValue = new()
		{
			ComparisonType = EquivalencyComparisonType.ByValue,
		};
		EquivalencyOptions options = new()
		{
			ComparisonType = EquivalencyComparisonType.ByMembers,
		};

		EquivalencyTypeOptions result = options.GetInheritedOptions(defaultValue);

		await That(result.ComparisonType).IsEqualTo(EquivalencyComparisonType.ByMembers)
			.Because("the comparison type of the top-level options applies to the whole graph");
	}

	[Test]
	public async Task GetInheritedOptions_ShouldNotInheritTheComparisonTypeOfTheDefaultValue()
	{
		EquivalencyTypeOptions defaultValue = new()
		{
			ComparisonType = EquivalencyComparisonType.ByMembers,
			IgnoreCollectionOrder = true,
		};
		EquivalencyOptions options = new();

		EquivalencyTypeOptions result = options.GetInheritedOptions(defaultValue);

		await That(result.ComparisonType).IsNull()
			.Because("the comparison type of a registration describes the registered type only, not its members");
		await That(result.IgnoreCollectionOrder).IsTrue()
			.Because("the other options of the enclosing type still apply to its members");
	}

	[Test]
	public async Task GetOptionsFor_ForARuntimeType_ShouldUseTheOptionsRegisteredForType()
	{
		EquivalencyTypeOptions typeOptions = new();
		EquivalencyOptions options = new EquivalencyOptions().For<Type>(_ => typeOptions);

		EquivalencyTypeOptions result = options.GetOptionsFor(typeof(int).GetType());

		await That(result).IsSameAs(typeOptions)
			.Because("the runtime type of a Type member is RuntimeType, which is the only type a user cannot name");
	}

	[Test]
	public async Task GetOptionsFor_ShouldApplyTheRegistrationToTheFinalOptions()
	{
		EquivalencyOptions options = new EquivalencyOptions().For<MyBaseClass>(o => o with
			{
				Fields = IncludeMembers.None,
			}) with
			{
				IgnoreCollectionOrder = true,
			};

		EquivalencyTypeOptions result = options.GetOptionsFor(typeof(MyBaseClass));

		await That(result.Fields).IsEqualTo(IncludeMembers.None);
		await That(result.IgnoreCollectionOrder).IsTrue()
			.Because("an option set after the registration has to apply to the registered type as well");
	}

	[Test]
	public async Task GetOptionsFor_WhenBothTheTypeAndItsBaseTypeAreRegistered_ShouldUseTheOptionsOfTheType()
	{
		EquivalencyTypeOptions baseTypeOptions = new();
		EquivalencyTypeOptions typeOptions = new();
		EquivalencyOptions options = new EquivalencyOptions()
			.For<MyBaseClass>(_ => baseTypeOptions)
			.For<MyDerivedClass>(_ => typeOptions);

		EquivalencyTypeOptions result = options.GetOptionsFor(typeof(MyDerivedClass));

		await That(result).IsSameAs(typeOptions)
			.Because("the more specific registration has to win over the one for the base type");
	}

	[Test]
	public async Task GetOptionsFor_WhenOnlyTheBaseTypeIsRegistered_ShouldUseTheOptionsOfTheBaseType()
	{
		EquivalencyTypeOptions baseTypeOptions = new();
		EquivalencyOptions options = new EquivalencyOptions().For<MyBaseClass>(_ => baseTypeOptions);

		EquivalencyTypeOptions result = options.GetOptionsFor(typeof(MyDerivedClass));

		await That(result).IsSameAs(baseTypeOptions)
			.Because("a member of an abstract type is always an instance of a derived type");
	}

	[Test]
	public async Task GetOptionsFor_WhenTypeIsNotRegistered_ShouldUseTheOptionsThemselves()
	{
		EquivalencyOptions options = new EquivalencyOptions().For<MyDerivedClass>(_ => new EquivalencyTypeOptions());

		EquivalencyTypeOptions result = options.GetOptionsFor(typeof(MyBaseClass));

		await That(result).IsSameAs(options)
			.Because("a registration for a derived type must not apply to its base type");
	}

	private class MyBaseClass;

	private sealed class MyDerivedClass : MyBaseClass;
}
