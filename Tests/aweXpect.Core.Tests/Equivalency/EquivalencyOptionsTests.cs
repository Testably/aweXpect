using System.Collections.Generic;
using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Equivalency;

public sealed class EquivalencyOptionsTests
{
	[Fact]
	public async Task For_ShouldNotChangeTheOptionsItIsCalledOn()
	{
		EquivalencyOptions inner = new();

		EquivalencyOptions<int> result = new(inner);
		_ = result.For<string>(_ => new EquivalencyTypeOptions());

		await That(inner.GetOptionsFor(typeof(string))).IsSameAs(inner)
			.Because("the customized default is shared by every expectation");
		await That(result.GetOptionsFor(typeof(string))).IsSameAs(result);
	}

	[Fact]
	public async Task For_WhenTypeIsAnInterface_ShouldThrowArgumentException()
	{
		void Act()
			=> new EquivalencyOptions().For<IEnumerable<int>>(x => x with
			{
				IgnoreCollectionOrder = true,
			});

		await That(Act).Throws<ArgumentException>()
			.WithMessage("""
			             Options cannot be registered for the interface IEnumerable<int>, because they are looked up by the runtime type of a value and its base types. Register them for a class or struct instead.
			             """)
			.Because("a registration for an interface would never apply");
	}

	[Fact]
	public async Task For_WhenTypeIsNullable_ShouldApplyToTheUnderlyingType()
	{
		EquivalencyTypeOptions typeOptions = new()
		{
			IgnoreCollectionOrder = true,
		};

		EquivalencyOptions result = new EquivalencyOptions().For<int?>(_ => typeOptions);

		await That(result.GetOptionsFor(typeof(int))).IsSameAs(typeOptions)
			.Because("the runtime type of a boxed nullable value is its underlying type");
	}

	[Fact]
	public async Task ToString_WhenVisibilitiesAreCombined_ShouldNameEachOfThem()
	{
		EquivalencyOptions options = new()
		{
			Fields = IncludeMembers.Public | IncludeMembers.Internal,
			Properties = IncludeMembers.Internal,
		};

		string result = options.ToString();

		await That(result).IsEqualTo(" - include public and internal fields and internal properties");
	}

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
	public async Task TypedOptions_ShouldKeepRegistrations()
	{
		EquivalencyTypeOptions typeOptions = new()
		{
			IgnoreCollectionOrder = true,
		};
		EquivalencyOptions inner = new EquivalencyOptions().For<int>(_ => typeOptions);

		EquivalencyOptions<int> result = new(inner);

		await That(result.GetOptionsFor(typeof(int))).IsSameAs(typeOptions)
			.Because("a globally customized default must survive the typed options of a per-call callback");
	}

	[Fact]
	public async Task TypedOptions_WhenInnerOptionsAlreadyContainTheType_ShouldOverrideThem()
	{
		EquivalencyOptions inner = new EquivalencyOptions().For<string>(_ => new EquivalencyTypeOptions());

		EquivalencyOptions<int> result = new EquivalencyOptions<int>(inner).For<string>(o => o with
		{
			IgnoreCollectionOrder = true,
		});

		await That(result.GetOptionsFor(typeof(string)).IgnoreCollectionOrder).IsTrue()
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
