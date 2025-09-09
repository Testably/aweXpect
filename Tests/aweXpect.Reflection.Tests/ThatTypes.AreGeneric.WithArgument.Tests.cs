using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class AreGeneric
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
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument<BaseClass>().AtIndex(index);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               are all generic with argument of type ThatTypes.BaseClass at index {{index}},
						               but it contained not matching types [
						                 ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						               ]
						               """);
				}

				[Theory]
				[InlineData(0, true)]
				[InlineData(1, false)]
				[InlineData(2, false)]
				public async Task AtIndex_FromEnd_ForTypeWithMatchingArgument_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument<BaseClass>().AtIndex(index)
							.FromEnd();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               are all generic with argument of type ThatTypes.BaseClass at index {{index}} from end,
						               but it contained not matching types [
						                 ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						               ]
						               """);
				}

				[Fact]
				public async Task ForTypeWithRestrictedMatchingArgument_ShouldSucceed()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument<BaseClass>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ForTypeWithRestrictedNotMatchingArgument_ShouldFail()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument<ThatMethod.DerivedClass>();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						             are all generic with argument of type ThatMethod.DerivedClass,
						             but it contained not matching types [
						               ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						             ]
						             """);
				}

				[Fact]
				public async Task ForTypeWithUnrestrictedArgument_ShouldFail()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithOneArgument<int>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument<BaseClass>();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that in types [ThatTypes.GenericClassWithOneArgument<int>]
						             are all generic with argument of type ThatTypes.BaseClass,
						             but it contained not matching types [
						               ThatTypes.GenericClassWithOneArgument<int>
						             ]
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
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument<BaseClass>("TBar").AtIndex(index);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               are all generic with argument of type ThatTypes.BaseClass and name equal to "TBar" at index {{index}},
						               but it contained not matching types [
						                 ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						               ]
						               """);
				}

				[Theory]
				[InlineData(0, true)]
				[InlineData(1, false)]
				[InlineData(2, false)]
				public async Task AtIndex_FromEnd_ForTypeWithMatchingArgument_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument<BaseClass>("TBar").AtIndex(index)
							.FromEnd();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               are all generic with argument of type ThatTypes.BaseClass and name equal to "TBar" at index {{index}} from end,
						               but it contained not matching types [
						                 ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						               ]
						               """);
				}

				[Fact]
				public async Task ForTypeWithRestrictedMatchingArgumentAndName_ShouldSucceed()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument<BaseClass>("TBar");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ForTypeWithRestrictedMatchingArgumentAndNotMatchingName_ShouldFail()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument<BaseClass>("Tbar");

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						             are all generic with argument of type ThatTypes.BaseClass and name equal to "Tbar",
						             but it contained not matching types [
						               ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						             ]
						             """);
				}

				[Fact]
				public async Task ForTypeWithRestrictedNotMatchingArgumentAndMatchingName_ShouldFail()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument<ThatMethod.DerivedClass>("TBar");

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						             are all generic with argument of type ThatMethod.DerivedClass and name equal to "TBar",
						             but it contained not matching types [
						               ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						             ]
						             """);
				}

				[Fact]
				public async Task ForTypeWithUnrestrictedArgument_ShouldFail()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithOneArgument<int>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument<BaseClass>("TBar");

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that in types [ThatTypes.GenericClassWithOneArgument<int>]
						             are all generic with argument of type ThatTypes.BaseClass and name equal to "TBar",
						             but it contained not matching types [
						               ThatTypes.GenericClassWithOneArgument<int>
						             ]
						             """);
				}

				[Theory]
				[InlineData("TBa", true)]
				[InlineData("Tba", false)]
				public async Task ShouldSupportAsPrefix(string prefix, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument<BaseClass>(prefix).AsPrefix();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               are all generic with argument of type ThatTypes.BaseClass and name starting with "{{prefix}}",
						               but it contained not matching types [
						                 ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						               ]
						               """);
				}

				[Theory]
				[InlineData("T[a-zA-Z]*r", true)]
				[InlineData("T[a-zA-Z]*o", false)]
				public async Task ShouldSupportAsRegex(string regex, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument<BaseClass>(regex).AsRegex();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               are all generic with argument of type ThatTypes.BaseClass and name matching regex "{{regex}}",
						               but it contained not matching types [
						                 ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						               ]
						               """);
				}

				[Theory]
				[InlineData("Bar", true)]
				[InlineData("bar", false)]
				public async Task ShouldSupportAsSuffix(string suffix, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument<BaseClass>(suffix).AsSuffix();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               are all generic with argument of type ThatTypes.BaseClass and name ending with "{{suffix}}",
						               but it contained not matching types [
						                 ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						               ]
						               """);
				}

				[Theory]
				[InlineData("T??r", true)]
				[InlineData("T??o", false)]
				public async Task ShouldSupportAsWildcard(string wildcard, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument<BaseClass>(wildcard).AsWildcard();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               are all generic with argument of type ThatTypes.BaseClass and name matching "{{wildcard}}",
						               but it contained not matching types [
						                 ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						               ]
						               """);
				}

				[Theory]
				[InlineData(true, true)]
				[InlineData(false, false)]
				public async Task ShouldSupportIgnoringCase(bool ignoreCase, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument<BaseClass>("TBAR")
							.IgnoringCase(ignoreCase);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						             are all generic with argument of type ThatTypes.BaseClass and name equal to "TBAR",
						             but it contained not matching types [
						               ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						             ]
						             """);
				}

				[Fact]
				public async Task ShouldSupportUsingCustomComparer()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument<BaseClass>("TBAr")
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
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument("TBar").AtIndex(index);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               are all generic with argument name equal to "TBar" at index {{index}},
						               but it contained not matching types [
						                 ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						               ]
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
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument("TBar").AtIndex(index).FromEnd();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               are all generic with argument name equal to "TBar" at index {{index}} from end,
						               but it contained not matching types [
						                 ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						               ]
						               """);
				}

				[Fact]
				public async Task ForTypeWithMatchingArgumentName_ShouldSucceed()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument("TBar");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ForTypeWithNotMatchingArgumentName_ShouldFail()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument("Tbar");

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						             are all generic with argument name equal to "Tbar",
						             but it contained not matching types [
						               ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						             ]
						             """);
				}

				[Theory]
				[InlineData("TBa", true)]
				[InlineData("Tba", false)]
				public async Task ShouldSupportAsPrefix(string prefix, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument(prefix).AsPrefix();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               are all generic with argument name starting with "{{prefix}}",
						               but it contained not matching types [
						                 ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						               ]
						               """);
				}

				[Theory]
				[InlineData("T[a-zA-Z]*r", true)]
				[InlineData("T[a-zA-Z]*s", false)]
				public async Task ShouldSupportAsRegex(string regex, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument(regex).AsRegex();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               are all generic with argument name matching regex "{{regex}}",
						               but it contained not matching types [
						                 ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						               ]
						               """);
				}

				[Theory]
				[InlineData("Bar", true)]
				[InlineData("bar", false)]
				public async Task ShouldSupportAsSuffix(string suffix, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument(suffix).AsSuffix();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               are all generic with argument name ending with "{{suffix}}",
						               but it contained not matching types [
						                 ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						               ]
						               """);
				}

				[Theory]
				[InlineData("T??r", true)]
				[InlineData("T??s", false)]
				public async Task ShouldSupportAsWildcard(string wildcard, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument(wildcard).AsWildcard();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						               are all generic with argument name matching "{{wildcard}}",
						               but it contained not matching types [
						                 ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						               ]
						               """);
				}

				[Theory]
				[InlineData(true, true)]
				[InlineData(false, false)]
				public async Task ShouldSupportIgnoringCase(bool ignoreCase, bool expectSuccess)
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument("TBAR").IgnoringCase(ignoreCase);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that in types [ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>]
						             are all generic with argument name equal to "TBAR",
						             but it contained not matching types [
						               ThatTypes.GenericClassWithTwoArguments<int, ThatTypes.BaseClass>
						             ]
						             """);
				}

				[Fact]
				public async Task ShouldSupportUsingCustomComparer()
				{
					Filtered.Types subject = In.Types(typeof(GenericClassWithTwoArguments<int, BaseClass>));

					async Task Act()
						=> await That(subject).AreGeneric().WithArgument("TBAr")
							.Using(new IgnoreCaseForVocalsComparer());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
