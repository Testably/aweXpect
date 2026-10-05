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
		await That(result.GetOptionsFor(typeof(MyClass))).IsEquivalentTo(new
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
		await That(result.GetOptionsFor(typeof(MyClass))).IsEquivalentTo(new
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
	public async Task Generic_For_Ignoring_TypePredicate_ShouldSetOptionForType()
	{
		EquivalencyOptions options = new();

		EquivalencyOptions result = options.For<MyClass>(o => o.Ignoring(x => x == typeof(DateTime)));

		await That(result.MembersToIgnore).IsEmpty();
		await That(result.GetOptionsFor(typeof(MyClass))).IsEquivalentTo(new
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
		await That(result.GetOptionsFor(typeof(MyClass))).IsEquivalentTo(new
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

	[Fact]
	public async Task Generic_For_IgnoringFields_ShouldSetOptionForType()
	{
		EquivalencyOptions options = new();

		EquivalencyOptions result = options.For<MyClass>(o
			=> o.IgnoringFields((n, t) => n.EndsWith("At") && t == typeof(DateTime)));

		await That(result.MembersToIgnore).IsEmpty();
		await That(result.GetOptionsFor(typeof(MyClass))).IsEquivalentTo(new
			{
				MembersToIgnore = It.Is<MemberToIgnore[]>().That.HasCount(1),
			});
		await That(result.ToString()).IsEqualTo("""
		                                         - include public fields and properties
		                                         - for EquivalencyOptionsExtensionsTests.MyClass:
		                                           - include public fields and properties
		                                           - ignore fields: [(n, t) => n.EndsWith("At") && t == typeof(DateTime)]
		                                        """)
			.Because("the rendering has to say which kind of member the predicate is applied to");
	}

	[Theory]
	[AutoData]
	public async Task Generic_For_IgnoringMember_ShouldSetOptionForType(string memberToIgnore)
	{
		EquivalencyOptions options = new();

		EquivalencyOptions result = options.For<MyClass>(o => o.IgnoringMember(memberToIgnore));

		await That(result.MembersToIgnore).IsEmpty();
		await That(result.GetOptionsFor(typeof(MyClass))).IsEquivalentTo(new
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

	[Fact]
	public async Task Generic_For_IgnoringProperties_ShouldSetOptionForType()
	{
		EquivalencyOptions options = new();

		EquivalencyOptions result = options.For<MyClass>(o
			=> o.IgnoringProperties((n, t) => n.EndsWith("At") && t == typeof(DateTime)));

		await That(result.MembersToIgnore).IsEmpty();
		await That(result.GetOptionsFor(typeof(MyClass))).IsEquivalentTo(new
			{
				MembersToIgnore = It.Is<MemberToIgnore[]>().That.HasCount(1),
			});
		await That(result.ToString()).IsEqualTo("""
		                                         - include public fields and properties
		                                         - for EquivalencyOptionsExtensionsTests.MyClass:
		                                           - include public fields and properties
		                                           - ignore properties: [(n, t) => n.EndsWith("At") && t == typeof(DateTime)]
		                                        """)
			.Because("the rendering has to say which kind of member the predicate is applied to");
	}

	[Theory]
	[InlineData(IncludeMembers.None)]
	[InlineData(IncludeMembers.Internal)]
	public async Task Generic_For_IncludingFields_ShouldSetOptionForType(IncludeMembers fieldsToInclude)
	{
		EquivalencyOptions options = new();
		string expectedVisibility = fieldsToInclude switch
		{
			IncludeMembers.Public => "public",
			IncludeMembers.Internal => "internal",
			_ => "no",
		};

		EquivalencyOptions result = options.For<MyClass>(o => o.IncludingFields(fieldsToInclude));

		await That(result.Fields).IsEqualTo(IncludeMembers.Public);
		await That(result.GetOptionsFor(typeof(MyClass))).IsEquivalentTo(new
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
	public async Task Generic_For_IncludingProperties_ShouldSetOptionForType(IncludeMembers propertiesToInclude)
	{
		EquivalencyOptions options = new();
		string expectedVisibility = propertiesToInclude switch
		{
			IncludeMembers.Public => "public",
			IncludeMembers.Internal => "internal",
			_ => "no",
		};

		EquivalencyOptions result = options.For<MyClass>(o => o.IncludingProperties(propertiesToInclude));

		await That(result.Properties).IsEqualTo(IncludeMembers.Public);
		await That(result.GetOptionsFor(typeof(MyClass))).IsEquivalentTo(new
			{
				Properties = propertiesToInclude,
			});
		await That(result.ToString()).IsEqualTo($"""
		                                          - include public fields and properties
		                                          - for EquivalencyOptionsExtensionsTests.MyClass:
		                                            - include public fields and {expectedVisibility} properties
		                                         """);
	}

	[Fact]
	public async Task Ignoring_StringAndTypePredicate_WhenPredicateIsNull_ShouldThrowArgumentNullException()
	{
		EquivalencyOptions options = new();

		void Act()
			=> _ = options.Ignoring((Func<string, Type, bool>)null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("predicate").And
			.WithMessage("The 'predicate' cannot be null.").AsPrefix();
	}

	[Fact]
	public async Task Ignoring_StringPredicate_WhenPredicateIsNull_ShouldThrowArgumentNullException()
	{
		EquivalencyOptions options = new();

		void Act()
			=> _ = options.Ignoring((Func<string, bool>)null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("predicate").And
			.WithMessage("The 'predicate' cannot be null.").AsPrefix();
	}

	[Fact]
	public async Task Ignoring_TypePredicate_WhenPredicateIsNull_ShouldThrowArgumentNullException()
	{
		EquivalencyOptions options = new();

		void Act()
			=> _ = options.Ignoring((Func<Type, bool>)null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("predicate").And
			.WithMessage("The 'predicate' cannot be null.").AsPrefix();
	}

	[Fact]
	public async Task IgnoringFields_WhenPredicateIsNull_ShouldThrowArgumentNullException()
	{
		EquivalencyOptions options = new();

		void Act()
			=> _ = options.IgnoringFields(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("predicate").And
			.WithMessage("The 'predicate' cannot be null.").AsPrefix();
	}

	[Fact]
	public async Task IgnoringMember_WhenMemberIsEmpty_ShouldThrowArgumentException()
	{
		EquivalencyOptions options = new();

		void Act()
			=> _ = options.IgnoringMember("");

		await That(Act).Throws<ArgumentException>()
			.WithParamName("memberToIgnore").And
			.WithMessage("The 'memberToIgnore' cannot be empty.").AsPrefix();
	}

	[Fact]
	public async Task IgnoringMember_WhenMemberIsEmpty_ShouldThrowFromTheExpectationThatConfiguresIt()
	{
		MyClass subject = new();

		async Task Act()
			=> await That(subject).IsEquivalentTo(new MyClass(), o => o.IgnoringMember(""));

		await That(Act).Throws<ArgumentException>()
			.WithParamName("memberToIgnore");
	}

	[Fact]
	public async Task IgnoringMember_WhenMemberIsNull_ShouldThrowArgumentNullException()
	{
		EquivalencyOptions options = new();

		void Act()
			=> _ = options.IgnoringMember(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("memberToIgnore").And
			.WithMessage("The 'memberToIgnore' cannot be null.").AsPrefix();
	}

	[Fact]
	public async Task IgnoringProperties_WhenPredicateIsNull_ShouldThrowArgumentNullException()
	{
		EquivalencyOptions options = new();

		void Act()
			=> _ = options.IgnoringProperties(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("predicate").And
			.WithMessage("The 'predicate' cannot be null.").AsPrefix();
	}

	private sealed class MyClass;
}
