using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Equivalency;

public sealed class EquivalencyOptionsExtensionsTests
{
	[Fact]
	public async Task GetTypeOptions_ForARuntimeType_ShouldUseTheOptionsRegisteredForType()
	{
		EquivalencyTypeOptions typeOptions = new();
		EquivalencyOptions options = new()
		{
			CustomOptions =
			{
				{
					typeof(Type), typeOptions
				},
			},
		};

		EquivalencyTypeOptions result = options.GetTypeOptions(typeof(int).GetType(), new EquivalencyTypeOptions());

		await That(result).IsSameAs(typeOptions)
			.Because("the runtime type of a Type member is RuntimeType, which is the only type a user cannot name");
	}

	[Fact]
	public async Task GetTypeOptions_WhenBothTheTypeAndItsBaseTypeAreRegistered_ShouldUseTheOptionsOfTheType()
	{
		EquivalencyTypeOptions baseTypeOptions = new();
		EquivalencyTypeOptions typeOptions = new();
		EquivalencyOptions options = new()
		{
			CustomOptions =
			{
				{
					typeof(MyBaseClass), baseTypeOptions
				},
				{
					typeof(MyDerivedClass), typeOptions
				},
			},
		};

		EquivalencyTypeOptions result = options.GetTypeOptions(typeof(MyDerivedClass), new EquivalencyTypeOptions());

		await That(result).IsSameAs(typeOptions)
			.Because("the more specific registration has to win over the one for the base type");
	}

	[Fact]
	public async Task GetTypeOptions_WhenOnlyTheBaseTypeIsRegistered_ShouldUseTheOptionsOfTheBaseType()
	{
		EquivalencyTypeOptions baseTypeOptions = new();
		EquivalencyOptions options = new()
		{
			CustomOptions =
			{
				{
					typeof(MyBaseClass), baseTypeOptions
				},
			},
		};

		EquivalencyTypeOptions result = options.GetTypeOptions(typeof(MyDerivedClass), new EquivalencyTypeOptions());

		await That(result).IsSameAs(baseTypeOptions)
			.Because("a member of an abstract type is always an instance of a derived type");
	}

	[Fact]
	public async Task GetTypeOptions_WhenTypeIsNotRegistered_ShouldKeepTheComparisonTypeOfTheOptions()
	{
		EquivalencyTypeOptions defaultValue = new()
		{
			ComparisonType = EquivalencyComparisonType.ByValue,
		};
		EquivalencyOptions options = new()
		{
			ComparisonType = EquivalencyComparisonType.ByMembers,
		};

		EquivalencyTypeOptions result = options.GetTypeOptions(typeof(MyBaseClass), defaultValue);

		await That(result.ComparisonType).IsEqualTo(EquivalencyComparisonType.ByMembers)
			.Because("the comparison type of the top-level options applies to the whole graph");
	}

	[Fact]
	public async Task GetTypeOptions_WhenTypeIsNotRegistered_ShouldNotInheritTheComparisonTypeOfTheDefaultValue()
	{
		EquivalencyTypeOptions defaultValue = new()
		{
			ComparisonType = EquivalencyComparisonType.ByMembers,
			IgnoreCollectionOrder = true,
		};
		EquivalencyOptions options = new();

		EquivalencyTypeOptions result = options.GetTypeOptions(typeof(MyBaseClass), defaultValue);

		await That(result.ComparisonType).IsNull()
			.Because("the comparison type of a registration describes the registered type only, not its members");
		await That(result.IgnoreCollectionOrder).IsTrue()
			.Because("the other options of the enclosing type still apply to its members");
	}

	[Fact]
	public async Task GetTypeOptions_WhenTypeIsNotRegistered_ShouldUseTheDefaultValue()
	{
		EquivalencyTypeOptions defaultValue = new();
		EquivalencyOptions options = new()
		{
			CustomOptions =
			{
				{
					typeof(MyDerivedClass), new EquivalencyTypeOptions()
				},
			},
		};

		EquivalencyTypeOptions result = options.GetTypeOptions(typeof(MyBaseClass), defaultValue);

		await That(result).IsSameAs(defaultValue)
			.Because("a registration for a derived type must not apply to its base type");
	}

	private class MyBaseClass;

	private sealed class MyDerivedClass : MyBaseClass;
}
