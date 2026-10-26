using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed partial class IsGeneric
	{
		public sealed class WithArgument
		{
			public sealed class GenericTests
			{
				[Test]
				[Arguments(0, false)]
				[Arguments(1, true)]
				[Arguments(2, false)]
				public async Task AtIndex_ForTypeWithMatchingArgument_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<BaseClass>().AtIndex(index);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatType.BaseClass at index {{index}},
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Test]
				[Arguments(0, true)]
				[Arguments(1, false)]
				[Arguments(2, false)]
				public async Task AtIndex_FromEnd_ForTypeWithMatchingArgument_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<BaseClass>().AtIndexFromEnd(index);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatType.BaseClass at index {{index}} from end,
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Test]
				public async Task AtIndex_WhenIndexFromEndIsAlreadySpecified_ShouldThrowInvalidOperationException()
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<BaseClass>().AtIndexFromEnd(0).AtIndex(1);
					}

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("AtIndex cannot be combined with AtIndexFromEnd.");
				}

				[Test]
				public async Task AtIndex_WhenIndexIsAlreadySpecified_ShouldUseTheLastIndex()
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<BaseClass>().AtIndex(1).AtIndex(2);
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is generic with argument of type ThatType.BaseClass at index 2,
						             but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						             """);
				}

				[Test]
				public async Task AtIndex_WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException()
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<BaseClass>().AtIndexFromEnd(-1);
					}

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("index").And
						.WithMessage("The index must not be negative.").AsPrefix();
				}

				[Test]
				public async Task ForTypeWithRestrictedMatchingArgument_ShouldSucceed()
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<BaseClass>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task ForTypeWithRestrictedNotMatchingArgument_ShouldFail()
				{
					Type subject = typeof(PublicGenericClass<int>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<DerivedClass>();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that subject
						             is generic with argument of type ThatType.DerivedClass,
						             but it was generic PublicGenericClass<int>
						             """);
				}

				[Test]
				public async Task ForTypeWithUnrestrictedArgument_ShouldFail()
				{
					Type subject = typeof(PublicGenericClass<string>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<BaseClass>();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that subject
						             is generic with argument of type ThatType.BaseClass,
						             but it was generic PublicGenericClass<string>
						             """);
				}
			}

			public sealed class GenericNamedTests
			{
				[Test]
				[Arguments(0, false)]
				[Arguments(1, true)]
				[Arguments(2, false)]
				public async Task AtIndex_ForTypeWithMatchingArgument_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<BaseClass>("TBar").AtIndex(index);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatType.BaseClass and name equal to "TBar" at index {{index}},
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Test]
				[Arguments(0, true)]
				[Arguments(1, false)]
				[Arguments(2, false)]
				public async Task AtIndex_FromEnd_ForTypeWithMatchingArgument_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<BaseClass>("TBar").AtIndexFromEnd(index);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatType.BaseClass and name equal to "TBar" at index {{index}} from end,
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Test]
				public async Task ForTypeWithRestrictedArgument_ShouldFail()
				{
					Type subject = typeof(PublicGenericClass<int>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<BaseClass>("TBar");
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that subject
						             is generic with argument of type ThatType.BaseClass and name equal to "TBar",
						             but it was generic PublicGenericClass<int>
						             """);
				}

				[Test]
				public async Task ForTypeWithRestrictedMatchingArgumentAndName_ShouldSucceed()
				{
					Type subject = typeof(GenericClassWithTwoArguments<string, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<BaseClass>("TBar");
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task ForTypeWithRestrictedMatchingArgumentAndNotMatchingName_ShouldFail()
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<BaseClass>("Tbar");
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that subject
						             is generic with argument of type ThatType.BaseClass and name equal to "Tbar",
						             but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						             """);
				}

				[Test]
				public async Task ForTypeWithRestrictedNotMatchingArgumentAndMatchingName_ShouldFail()
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<DerivedClass>("TBar");
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that subject
						             is generic with argument of type ThatType.DerivedClass and name equal to "TBar",
						             but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						             """);
				}

				[Test]
				[Arguments("T", false)]
				[Arguments("TBar", true)]
				public async Task ForTypeWithUnrestrictedArgument_ShouldSucceedWhenTypeAndNameMatches(
					string name, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<,>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<BaseClass>(name);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($"""
						              Expected that subject
						              is generic with argument of type ThatType.BaseClass and name equal to "{name}",
						              but it was generic ThatType.GenericClassWithTwoArguments<,>
						              """);
				}

				[Test]
				[Arguments("TBa", true)]
				[Arguments("Tba", false)]
				public async Task ShouldSupportAsPrefix(string prefix, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<BaseClass>(prefix).AsPrefix();
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatType.BaseClass and name starting with "{{prefix}}",
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Test]
				[Arguments("T[a-zA-Z]*r", true)]
				[Arguments("T[a-zA-Z]*o", false)]
				public async Task ShouldSupportAsRegex(string regex, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<BaseClass>(regex).AsRegex();
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatType.BaseClass and name matching regex "{{regex}}",
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Test]
				[Arguments("Bar", true)]
				[Arguments("bar", false)]
				public async Task ShouldSupportAsSuffix(string suffix, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<BaseClass>(suffix).AsSuffix();
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatType.BaseClass and name ending with "{{suffix}}",
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Test]
				[Arguments("T??r", true)]
				[Arguments("T??o", false)]
				public async Task ShouldSupportAsWildcard(string wildcard, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<BaseClass>(wildcard).AsWildcard();
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatType.BaseClass and name matching "{{wildcard}}",
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Test]
				[Arguments(true, true)]
				[Arguments(false, false)]
				public async Task ShouldSupportIgnoringCase(bool ignoreCase, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<BaseClass>("TBAR").IgnoringCase(ignoreCase);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that subject
						             is generic with argument of type ThatType.BaseClass and name equal to "TBAR",
						             but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						             """);
				}

				[Test]
				public async Task ShouldSupportUsingCustomComparer()
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument<BaseClass>("TBAr")
							.Using(new IgnoreCaseForVocalsComparer());
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NamedTests
			{
				[Test]
				[Arguments(0, false)]
				[Arguments(1, true)]
				[Arguments(2, false)]
				public async Task AtIndex_ForTypeWithMatchingArgumentName_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument("TBar").AtIndex(index);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name equal to "TBar" at index {{index}},
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Test]
				[Arguments(0, true)]
				[Arguments(1, false)]
				[Arguments(2, false)]
				public async Task AtIndex_FromEnd_ForTypeWithMatchingArgumentName_ShouldSucceedWhenIndexMatches(
					int index,
					bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument("TBar").AtIndexFromEnd(index);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name equal to "TBar" at index {{index}} from end,
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Test]
				public async Task ForTypeWithMatchingArgumentName_ShouldSucceed()
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument("TBar");
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task ForTypeWithNotMatchingArgumentName_ShouldFail()
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument("Tbar");
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that subject
						             is generic with argument name equal to "Tbar",
						             but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						             """);
				}

				[Test]
				[Arguments("T", true)]
				[Arguments("TBar", false)]
				public async Task ForTypeWithUnrestrictedArgument_ShouldSucceedWhenNameMatches(
					string name, bool expectSuccess)
				{
					Type subject = typeof(PublicGenericClass<>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument(name);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name equal to "{{name}}",
						               but it was generic PublicGenericClass<>
						               """);
				}

				[Test]
				[Arguments("tba", true)]
				[Arguments("tbx", false)]
				public async Task ShouldSupportIgnoringCaseBecauseAndAsPrefix(string prefix, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument(prefix).IgnoringCase().Because("we need it")
							.AsPrefix();
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name starting with "{{prefix}}" ignoring case, because we need it,
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """)
						.Because("Because(…) must return the result, so that the string options can still follow it");
				}

				[Test]
				[Arguments("TBa", true)]
				[Arguments("Tba", false)]
				public async Task ShouldSupportAsPrefix(string prefix, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument(prefix).AsPrefix();
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name starting with "{{prefix}}",
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Test]
				[Arguments("T[a-zA-Z]*r", true)]
				[Arguments("T[a-zA-Z]*s", false)]
				public async Task ShouldSupportAsRegex(string regex, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument(regex).AsRegex();
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name matching regex "{{regex}}",
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Test]
				[Arguments("Bar", true)]
				[Arguments("bar", false)]
				public async Task ShouldSupportAsSuffix(string suffix, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument(suffix).AsSuffix();
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name ending with "{{suffix}}",
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Test]
				[Arguments("T??r", true)]
				[Arguments("T??s", false)]
				public async Task ShouldSupportAsWildcard(string wildcard, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument(wildcard).AsWildcard();
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name matching "{{wildcard}}",
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Test]
				[Arguments(true, true)]
				[Arguments(false, false)]
				public async Task ShouldSupportIgnoringCase(bool ignoreCase, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument("TBAR").IgnoringCase(ignoreCase);
					}

					await That(Act).Throws()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that subject
						             is generic with argument name equal to "TBAR",
						             but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						             """);
				}

				[Test]
				public async Task ShouldSupportUsingCustomComparer()
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
					{
						await That(subject).IsGeneric().WithArgument("TBAr")
							.Using(new IgnoreCaseForVocalsComparer());
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
