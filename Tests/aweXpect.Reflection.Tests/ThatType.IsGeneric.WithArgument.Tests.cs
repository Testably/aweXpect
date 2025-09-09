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
				[Theory]
				[InlineData(0, false)]
				[InlineData(1, true)]
				[InlineData(2, false)]
				public async Task AtIndex_ForTypeWithMatchingArgument_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>().AtIndex(index);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatType.BaseClass at index {{index}},
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Theory]
				[InlineData(0, true)]
				[InlineData(1, false)]
				[InlineData(2, false)]
				public async Task AtIndex_FromEnd_ForTypeWithMatchingArgument_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>().AtIndex(index).FromEnd();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatType.BaseClass at index {{index}} from end,
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Fact]
				public async Task ForTypeWithRestrictedMatchingArgument_ShouldSucceed()
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ForTypeWithRestrictedNotMatchingArgument_ShouldFail()
				{
					Type subject = typeof(PublicGenericClass<int>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<DerivedClass>();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that subject
						             is generic with argument of type ThatType.DerivedClass,
						             but it was generic PublicGenericClass<int>
						             """);
				}

				[Fact]
				public async Task ForTypeWithUnrestrictedArgument_ShouldFail()
				{
					Type subject = typeof(PublicGenericClass<string>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that subject
						             is generic with argument of type ThatType.BaseClass,
						             but it was generic PublicGenericClass<string>
						             """);
				}
			}

			public sealed class GenericNamedTests
			{
				[Theory]
				[InlineData(0, false)]
				[InlineData(1, true)]
				[InlineData(2, false)]
				public async Task AtIndex_ForTypeWithMatchingArgument_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>("TBar").AtIndex(index);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatType.BaseClass and name equal to "TBar" at index {{index}},
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Theory]
				[InlineData(0, true)]
				[InlineData(1, false)]
				[InlineData(2, false)]
				public async Task AtIndex_FromEnd_ForTypeWithMatchingArgument_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>("TBar").AtIndex(index).FromEnd();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatType.BaseClass and name equal to "TBar" at index {{index}} from end,
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Fact]
				public async Task ForTypeWithRestrictedArgument_ShouldFail()
				{
					Type subject = typeof(PublicGenericClass<int>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>("TBar");

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that subject
						             is generic with argument of type ThatType.BaseClass and name equal to "TBar",
						             but it was generic PublicGenericClass<int>
						             """);
				}

				[Fact]
				public async Task ForTypeWithRestrictedMatchingArgumentAndName_ShouldSucceed()
				{
					Type subject = typeof(GenericClassWithTwoArguments<string, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>("TBar");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ForTypeWithRestrictedMatchingArgumentAndNotMatchingName_ShouldFail()
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>("Tbar");

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that subject
						             is generic with argument of type ThatType.BaseClass and name equal to "Tbar",
						             but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						             """);
				}

				[Fact]
				public async Task ForTypeWithRestrictedNotMatchingArgumentAndMatchingName_ShouldFail()
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<DerivedClass>("TBar");

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that subject
						             is generic with argument of type ThatType.DerivedClass and name equal to "TBar",
						             but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						             """);
				}

				[Theory]
				[InlineData("T", false)]
				[InlineData("TBar", true)]
				public async Task ForTypeWithUnrestrictedArgument_ShouldSucceedWhenTypeAndNameMatches(
					string name, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<,>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>(name);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatType.BaseClass and name equal to "{{name}}",
						               but it was generic ThatType.GenericClassWithTwoArguments<, >
						               """);
				}

				[Theory]
				[InlineData("TBa", true)]
				[InlineData("Tba", false)]
				public async Task ShouldSupportAsPrefix(string prefix, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>(prefix).AsPrefix();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatType.BaseClass and name starting with "{{prefix}}",
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Theory]
				[InlineData("T[a-zA-Z]*r", true)]
				[InlineData("T[a-zA-Z]*o", false)]
				public async Task ShouldSupportAsRegex(string regex, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>(regex).AsRegex();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatType.BaseClass and name matching regex "{{regex}}",
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Theory]
				[InlineData("Bar", true)]
				[InlineData("bar", false)]
				public async Task ShouldSupportAsSuffix(string suffix, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>(suffix).AsSuffix();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatType.BaseClass and name ending with "{{suffix}}",
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Theory]
				[InlineData("T??r", true)]
				[InlineData("T??o", false)]
				public async Task ShouldSupportAsWildcard(string wildcard, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>(wildcard).AsWildcard();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatType.BaseClass and name matching "{{wildcard}}",
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Theory]
				[InlineData(true, true)]
				[InlineData(false, false)]
				public async Task ShouldSupportIgnoringCase(bool ignoreCase, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>("TBAR").IgnoringCase(ignoreCase);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that subject
						             is generic with argument of type ThatType.BaseClass and name equal to "TBAR",
						             but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						             """);
				}

				[Fact]
				public async Task ShouldSupportUsingCustomComparer()
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>("TBAr")
							.Using(new IgnoreCaseForVocalsComparer());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NamedTests
			{
				[Theory]
				[InlineData(0, false)]
				[InlineData(1, true)]
				[InlineData(2, false)]
				public async Task AtIndex_ForTypeWithMatchingArgumentName_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument("TBar").AtIndex(index);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name equal to "TBar" at index {{index}},
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Theory]
				[InlineData(0, true)]
				[InlineData(1, false)]
				[InlineData(2, false)]
				public async Task AtIndex_FromEnd_ForTypeWithMatchingArgumentName_ShouldSucceedWhenIndexMatches(
					int index,
					bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument("TBar").AtIndex(index).FromEnd();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name equal to "TBar" at index {{index}} from end,
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Fact]
				public async Task ForTypeWithMatchingArgumentName_ShouldSucceed()
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument("TBar");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ForTypeWithNotMatchingArgumentName_ShouldFail()
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument("Tbar");

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that subject
						             is generic with argument name equal to "Tbar",
						             but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						             """);
				}

				[Theory]
				[InlineData("T", true)]
				[InlineData("TBar", false)]
				public async Task ForTypeWithUnrestrictedArgument_ShouldSucceedWhenNameMatches(
					string name, bool expectSuccess)
				{
					Type subject = typeof(PublicGenericClass<>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument(name);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name equal to "{{name}}",
						               but it was generic PublicGenericClass<>
						               """);
				}

				[Theory]
				[InlineData("TBa", true)]
				[InlineData("Tba", false)]
				public async Task ShouldSupportAsPrefix(string prefix, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument(prefix).AsPrefix();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name starting with "{{prefix}}",
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Theory]
				[InlineData("T[a-zA-Z]*r", true)]
				[InlineData("T[a-zA-Z]*s", false)]
				public async Task ShouldSupportAsRegex(string regex, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument(regex).AsRegex();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name matching regex "{{regex}}",
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Theory]
				[InlineData("Bar", true)]
				[InlineData("bar", false)]
				public async Task ShouldSupportAsSuffix(string suffix, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument(suffix).AsSuffix();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name ending with "{{suffix}}",
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Theory]
				[InlineData("T??r", true)]
				[InlineData("T??s", false)]
				public async Task ShouldSupportAsWildcard(string wildcard, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument(wildcard).AsWildcard();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name matching "{{wildcard}}",
						               but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						               """);
				}

				[Theory]
				[InlineData(true, true)]
				[InlineData(false, false)]
				public async Task ShouldSupportIgnoringCase(bool ignoreCase, bool expectSuccess)
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument("TBAR").IgnoringCase(ignoreCase);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that subject
						             is generic with argument name equal to "TBAR",
						             but it was generic ThatType.GenericClassWithTwoArguments<int, ThatType.BaseClass>
						             """);
				}

				[Fact]
				public async Task ShouldSupportUsingCustomComparer()
				{
					Type subject = typeof(GenericClassWithTwoArguments<int, BaseClass>);

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument("TBAr")
							.Using(new IgnoreCaseForVocalsComparer());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
