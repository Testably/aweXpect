using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Equivalency;

public sealed class EquivalencyOptionsTests
{
	[Fact]
	public async Task TypedOptions_ShouldKeepComparisonType()
	{
		EquivalencyOptions inner = new()
		{
			ComparisonType = EquivalencyComparisonType.ByValue,
		};

		EquivalencyOptions<int> result = new(inner);

		await That(result.ComparisonType).IsEqualTo(EquivalencyComparisonType.ByValue)
			.Because("a globally customized default must survive the typed options of a per-call callback");
	}

	[Fact]
	public async Task TypedOptions_ShouldKeepCustomOptions()
	{
		EquivalencyTypeOptions typeOptions = new()
		{
			IgnoreCollectionOrder = true,
		};
		EquivalencyOptions inner = new()
		{
			CustomOptions =
			{
				{
					typeof(int), typeOptions
				},
			},
		};

		EquivalencyOptions<int> result = new(inner);

		await That(result.CustomOptions).ContainsKey(typeof(int)).WhoseValue.IsSameAs(typeOptions)
			.Because("a globally customized default must survive the typed options of a per-call callback");
	}

	[Fact]
	public async Task TypedOptions_ShouldKeepDefaultComparisonTypeSelector()
	{
		EquivalencyOptions inner = new()
		{
			DefaultComparisonTypeSelector = _ => EquivalencyComparisonType.ByValue,
		};

		EquivalencyOptions<int> result = new(inner);

		await That(result.DefaultComparisonTypeSelector(typeof(object))).IsEqualTo(EquivalencyComparisonType.ByValue)
			.Because("a globally customized default must survive the typed options of a per-call callback");
	}

	[Fact]
	public async Task TypedOptions_ShouldKeepFields()
	{
		EquivalencyOptions inner = new()
		{
			Fields = IncludeMembers.None,
		};

		EquivalencyOptions<int> result = new(inner);

		await That(result.Fields).IsEqualTo(IncludeMembers.None)
			.Because("a globally customized default must survive the typed options of a per-call callback");
	}

	[Fact]
	public async Task TypedOptions_ShouldKeepIgnoreCollectionOrder()
	{
		EquivalencyOptions inner = new()
		{
			IgnoreCollectionOrder = true,
		};

		EquivalencyOptions<int> result = new(inner);

		await That(result.IgnoreCollectionOrder).IsTrue()
			.Because("a globally customized default must survive the typed options of a per-call callback");
	}

	[Fact]
	public async Task TypedOptions_ShouldKeepMaxRecursionDepth()
	{
		EquivalencyOptions inner = new()
		{
			MaxRecursionDepth = 7,
		};

		EquivalencyOptions<int> result = new(inner);

		await That(result.MaxRecursionDepth).IsEqualTo(7)
			.Because("a globally customized default must survive the typed options of a per-call callback");
	}

	[Fact]
	public async Task TypedOptions_ShouldKeepMembersToIgnore()
	{
		MemberToIgnore memberToIgnore = new MemberToIgnore.ByName("Foo");
		EquivalencyOptions inner = new()
		{
			MembersToIgnore = [memberToIgnore,],
		};

		EquivalencyOptions<int> result = new(inner);

		await That(result.MembersToIgnore).IsEqualTo([memberToIgnore,])
			.Because("a globally customized default must survive the typed options of a per-call callback");
	}

	[Fact]
	public async Task TypedOptions_ShouldKeepProperties()
	{
		EquivalencyOptions inner = new()
		{
			Properties = IncludeMembers.None,
		};

		EquivalencyOptions<int> result = new(inner);

		await That(result.Properties).IsEqualTo(IncludeMembers.None)
			.Because("a globally customized default must survive the typed options of a per-call callback");
	}

	[Fact]
	public async Task TypedOptions_ShouldNotShareCustomOptionsWithInnerOptions()
	{
		EquivalencyOptions inner = new();

		EquivalencyOptions<int> result = new(inner);
		result.For<string>(o => o);

		await That(inner.CustomOptions).IsEmpty()
			.Because("For<T> mutates the dictionary in place, which must not reach the globally customized default");
	}

	[Fact]
	public async Task TypedOptions_WhenInnerOptionsAlreadyContainTheType_ShouldOverrideThem()
	{
		EquivalencyOptions inner = new()
		{
			CustomOptions =
			{
				{
					typeof(string), new EquivalencyTypeOptions()
				},
			},
		};

		EquivalencyOptions<int> result = new(inner);
		result.For<string>(o => o with
		{
			IgnoreCollectionOrder = true,
		});

		await That(result.CustomOptions[typeof(string)].IgnoreCollectionOrder).IsTrue()
			.Because("the options of a single expectation win over the customized default");
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public async Task WhenMaxRecursionDepthIsNotPositive_ShouldThrowArgumentOutOfRangeException(int maxRecursionDepth)
	{
		void Act()
		{
			_ = new EquivalencyOptions
			{
				MaxRecursionDepth = maxRecursionDepth,
			};
		}

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("*The maximum recursion depth must be greater than zero*").AsWildcard()
			.Because("a limit below one would fail even the root comparison");
	}
}
