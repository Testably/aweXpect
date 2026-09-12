using aweXpect.Equivalency;

namespace aweXpect.Internal.Tests.Equivalency;

public sealed class EquivalencyOptionsExtensionsTests
{
	[Fact]
	public async Task Generic_For_Ignoring_StringAndTypePredicate_ShouldSetOptionForType()
	{
		EquivalencyOptions options = new();

		EquivalencyOptions result =
			options.For<MyClass>(o => o.Ignoring((n, t) => n.EndsWith("At") && t == typeof(DateTime)));

		await That(result.MembersToIgnore).IsEmpty();
		await That(result.CustomOptions).ContainsKey(typeof(MyClass))
			.WhoseValue.IsEquivalentTo(new
			{
				MembersToIgnore = It.Is<MemberToIgnore[]>().That.HasCount(1),
			});
		await That(result.ToString()).IsEqualTo("""
		                                         - include public fields and properties
		                                         - for EquivalencyOptionsExtensionsTests.MyClass:
		                                           - include public fields and properties
		                                           - ignore members: [(n, t) => n.EndsWith("At") && t == typeof(DateTime)]
		                                        """);
	}

	[Fact]
	public async Task Generic_For_Ignoring_StringPredicate_ShouldSetOptionForType()
	{
		EquivalencyOptions options = new();

		EquivalencyOptions result = options.For<MyClass>(o => o.Ignoring(x => x == "foo"));

		await That(result.MembersToIgnore).IsEmpty();
		await That(result.CustomOptions).ContainsKey(typeof(MyClass))
			.WhoseValue.IsEquivalentTo(new
			{
				MembersToIgnore = It.Is<MemberToIgnore[]>().That.HasCount(1),
			});
		await That(result.ToString()).IsEqualTo("""
		                                         - include public fields and properties
		                                         - for EquivalencyOptionsExtensionsTests.MyClass:
		                                           - include public fields and properties
		                                           - ignore members: [x => x == "foo"]
		                                        """);
	}

	[Fact]
	public async Task Generic_For_Ignoring_StringTypeAndMemberInfoPredicate_ShouldSetOptionForType()
	{
		EquivalencyOptions options = new();

		EquivalencyOptions result = options.For<MyClass>(o
			=> o.Ignoring((n, t, f) => n.EndsWith("At") && t == typeof(DateTime) && f != null));

		await That(result.MembersToIgnore).IsEmpty();
		await That(result.CustomOptions).ContainsKey(typeof(MyClass))
			.WhoseValue.IsEquivalentTo(new
			{
				MembersToIgnore = It.Is<MemberToIgnore[]>().That.HasCount(1),
			});
		await That(result.ToString()).IsEqualTo("""
		                                         - include public fields and properties
		                                         - for EquivalencyOptionsExtensionsTests.MyClass:
		                                           - include public fields and properties
		                                           - ignore members: [(n, t, f) => n.EndsWith("At") && t == typeof(DateTime) && f != null]
		                                        """);
	}

	[Fact]
	public async Task Generic_For_Ignoring_TypePredicate_ShouldSetOptionForType()
	{
		EquivalencyOptions options = new();

		EquivalencyOptions result = options.For<MyClass>(o => o.Ignoring(x => x == typeof(DateTime)));

		await That(result.MembersToIgnore).IsEmpty();
		await That(result.CustomOptions).ContainsKey(typeof(MyClass))
			.WhoseValue.IsEquivalentTo(new
			{
				MembersToIgnore = It.Is<MemberToIgnore[]>().That.HasCount(1),
			});
		await That(result.ToString()).IsEqualTo("""
		                                         - include public fields and properties
		                                         - for EquivalencyOptionsExtensionsTests.MyClass:
		                                           - include public fields and properties
		                                           - ignore members: [x => x == typeof(DateTime)]
		                                        """);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public async Task Generic_For_IgnoringCollectionOrder_ShouldSetOptionForType(bool ignoreCollectionOrder)
	{
		EquivalencyOptions options = new();

		EquivalencyOptions result = options.For<MyClass>(o => o.IgnoringCollectionOrder(ignoreCollectionOrder));

		await That(result.IgnoreCollectionOrder).IsFalse();
		await That(result.CustomOptions).ContainsKey(typeof(MyClass))
			.WhoseValue.IsEquivalentTo(new
			{
				IgnoreCollectionOrder = ignoreCollectionOrder,
			});

		if (ignoreCollectionOrder)
		{
			await That(result.ToString()).IsEqualTo("""
			                                         - include public fields and properties
			                                         - for EquivalencyOptionsExtensionsTests.MyClass:
			                                           - include public fields and properties
			                                           - ignore collection order
			                                        """);
		}
	}

	[Theory]
	[AutoData]
	public async Task Generic_For_IgnoringMember_ShouldSetOptionForType(string memberToIgnore)
	{
		EquivalencyOptions options = new();

		EquivalencyOptions result = options.For<MyClass>(o => o.IgnoringMember(memberToIgnore));

		await That(result.MembersToIgnore).IsEmpty();
		await That(result.CustomOptions).ContainsKey(typeof(MyClass))
			.WhoseValue.IsEquivalentTo(new
			{
				MembersToIgnore = It.Is<MemberToIgnore[]>().That.HasCount(1),
			});
		await That(result.ToString()).IsEqualTo($"""
		                                          - include public fields and properties
		                                          - for EquivalencyOptionsExtensionsTests.MyClass:
		                                            - include public fields and properties
		                                            - ignore members: ["{memberToIgnore}"]
		                                         """);
	}

	[Theory]
	[InlineData(IncludeMembers.None)]
	[InlineData(IncludeMembers.Internal)]
	[InlineData(IncludeMembers.Private)]
	public async Task Generic_For_IncludingFields_ShouldSetOptionForType(IncludeMembers fieldsToInclude)
	{
		EquivalencyOptions options = new();
		string expectedVisibility = fieldsToInclude switch
		{
			IncludeMembers.Public => "public",
			IncludeMembers.Internal => "internal",
			IncludeMembers.Private => "private",
			_ => "no",
		};

		EquivalencyOptions result = options.For<MyClass>(o => o.IncludingFields(fieldsToInclude));

		await That(result.Fields).IsEqualTo(IncludeMembers.Public);
		await That(result.CustomOptions).ContainsKey(typeof(MyClass))
			.WhoseValue.IsEquivalentTo(new
			{
				Fields = fieldsToInclude,
			});
		await That(result.ToString()).IsEqualTo($"""
		                                          - include public fields and properties
		                                          - for EquivalencyOptionsExtensionsTests.MyClass:
		                                            - include {expectedVisibility} fields and public properties
		                                         """);
	}

	[Theory]
	[InlineData(IncludeMembers.None)]
	[InlineData(IncludeMembers.Internal)]
	[InlineData(IncludeMembers.Private)]
	public async Task Generic_For_IncludingProperties_ShouldSetOptionForType(IncludeMembers propertiesToInclude)
	{
		EquivalencyOptions options = new();
		string expectedVisibility = propertiesToInclude switch
		{
			IncludeMembers.Public => "public",
			IncludeMembers.Internal => "internal",
			IncludeMembers.Private => "private",
			_ => "no",
		};

		EquivalencyOptions result = options.For<MyClass>(o => o.IncludingProperties(propertiesToInclude));

		await That(result.Properties).IsEqualTo(IncludeMembers.Public);
		await That(result.CustomOptions).ContainsKey(typeof(MyClass))
			.WhoseValue.IsEquivalentTo(new
			{
				Properties = propertiesToInclude,
			});
		await That(result.ToString()).IsEqualTo($"""
		                                          - include public fields and properties
		                                          - for EquivalencyOptionsExtensionsTests.MyClass:
		                                            - include public fields and {expectedVisibility} properties
		                                         """);
	}

	private sealed class MyClass;
}
