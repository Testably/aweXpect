using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
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
				public async Task AtIndex_ForMethodWithMatchingArgument_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>().AtIndex(index);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatMethod.BaseClass at index {{index}},
						               but it was generic void ThatMethod.ClassWithMethods.GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
						               """);
				}

				[Theory]
				[InlineData(0, true)]
				[InlineData(1, false)]
				[InlineData(2, false)]
				public async Task AtIndex_FromEnd_ForMethodWithMatchingArgument_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>().AtIndex(index).FromEnd();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatMethod.BaseClass at index {{index}} from end,
						               but it was generic void ThatMethod.ClassWithMethods.GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
						               """);
				}

				[Fact]
				public async Task ForMethodWithRestrictedMatchingArgument_ShouldSucceed()
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ForMethodWithRestrictedNotMatchingArgument_ShouldFail()
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<DerivedClass>();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that subject
						             is generic with argument of type ThatMethod.DerivedClass,
						             but it was generic void ThatMethod.ClassWithMethods.GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
						             """);
				}

				[Fact]
				public async Task ForMethodWithUnrestrictedArgument_ShouldFail()
				{
					MethodInfo? subject = GetMethod("GenericWithUnrestrictedArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that subject
						             is generic with argument of type ThatMethod.BaseClass,
						             but it was generic void ThatMethod.ClassWithMethods.GenericWithUnrestrictedArgumentMethod<TFoo>()
						             """);
				}
			}

			public sealed class GenericNamedTests
			{
				[Theory]
				[InlineData(0, false)]
				[InlineData(1, true)]
				[InlineData(2, false)]
				public async Task AtIndex_ForMethodWithMatchingArgument_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>("TBar").AtIndex(index);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatMethod.BaseClass and name equal to "TBar" at index {{index}},
						               but it was generic void ThatMethod.ClassWithMethods.GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
						               """);
				}

				[Theory]
				[InlineData(0, true)]
				[InlineData(1, false)]
				[InlineData(2, false)]
				public async Task AtIndex_FromEnd_ForMethodWithMatchingArgument_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>("TBar").AtIndex(index).FromEnd();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatMethod.BaseClass and name equal to "TBar" at index {{index}} from end,
						               but it was generic void ThatMethod.ClassWithMethods.GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
						               """);
				}

				[Fact]
				public async Task ForMethodWithRestrictedMatchingArgumentAndName_ShouldSucceed()
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>("TBar");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ForMethodWithRestrictedMatchingArgumentAndNotMatchingName_ShouldFail()
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>("Tbar");

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that subject
						             is generic with argument of type ThatMethod.BaseClass and name equal to "Tbar",
						             but it was generic void ThatMethod.ClassWithMethods.GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
						             """);
				}

				[Fact]
				public async Task ForMethodWithRestrictedNotMatchingArgumentAndMatchingName_ShouldFail()
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<DerivedClass>("TBar");

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that subject
						             is generic with argument of type ThatMethod.DerivedClass and name equal to "TBar",
						             but it was generic void ThatMethod.ClassWithMethods.GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
						             """);
				}

				[Fact]
				public async Task ForMethodWithUnrestrictedArgument_ShouldFail()
				{
					MethodInfo? subject = GetMethod("GenericWithUnrestrictedArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>("TBar");

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that subject
						             is generic with argument of type ThatMethod.BaseClass and name equal to "TBar",
						             but it was generic void ThatMethod.ClassWithMethods.GenericWithUnrestrictedArgumentMethod<TFoo>()
						             """);
				}

				[Theory]
				[InlineData("TBa", true)]
				[InlineData("Tba", false)]
				public async Task ShouldSupportAsPrefix(string prefix, bool expectSuccess)
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>(prefix).AsPrefix();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatMethod.BaseClass and name starting with "{{prefix}}",
						               but it was generic void ThatMethod.ClassWithMethods.GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
						               """);
				}

				[Theory]
				[InlineData("T[a-zA-Z]*r", true)]
				[InlineData("T[a-zA-Z]*o", false)]
				public async Task ShouldSupportAsRegex(string regex, bool expectSuccess)
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>(regex).AsRegex();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatMethod.BaseClass and name matching regex "{{regex}}",
						               but it was generic void ThatMethod.ClassWithMethods.GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
						               """);
				}

				[Theory]
				[InlineData("Bar", true)]
				[InlineData("bar", false)]
				public async Task ShouldSupportAsSuffix(string suffix, bool expectSuccess)
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>(suffix).AsSuffix();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatMethod.BaseClass and name ending with "{{suffix}}",
						               but it was generic void ThatMethod.ClassWithMethods.GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
						               """);
				}

				[Theory]
				[InlineData("T??r", true)]
				[InlineData("T??o", false)]
				public async Task ShouldSupportAsWildcard(string wildcard, bool expectSuccess)
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>(wildcard).AsWildcard();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument of type ThatMethod.BaseClass and name matching "{{wildcard}}",
						               but it was generic void ThatMethod.ClassWithMethods.GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
						               """);
				}

				[Theory]
				[InlineData(true, true)]
				[InlineData(false, false)]
				public async Task ShouldSupportIgnoringCase(bool ignoreCase, bool expectSuccess)
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument<BaseClass>("TBAR").IgnoringCase(ignoreCase);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that subject
						             is generic with argument of type ThatMethod.BaseClass and name equal to "TBAR",
						             but it was generic void ThatMethod.ClassWithMethods.GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
						             """);
				}

				[Fact]
				public async Task ShouldSupportUsingCustomComparer()
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

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
				public async Task AtIndex_ForMethodWithMatchingArgumentName_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument("TBar").AtIndex(index);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name equal to "TBar" at index {{index}},
						               but it was generic void ThatMethod.ClassWithMethods.GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
						               """);
				}

				[Theory]
				[InlineData(0, true)]
				[InlineData(1, false)]
				[InlineData(2, false)]
				public async Task AtIndex_FromEnd_ForMethodWithMatchingArgumentName_ShouldSucceedWhenIndexMatches(int index,
					bool expectSuccess)
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument("TBar").AtIndex(index).FromEnd();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name equal to "TBar" at index {{index}} from end,
						               but it was generic void ThatMethod.ClassWithMethods.GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
						               """);
				}

				[Fact]
				public async Task ForMethodWithMatchingArgumentName_ShouldSucceed()
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument("TBar");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ForMethodWithNotMatchingArgumentName_ShouldFail()
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument("Tbar");

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that subject
						             is generic with argument name equal to "Tbar",
						             but it was generic void ThatMethod.ClassWithMethods.GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
						             """);
				}

				[Theory]
				[InlineData("TBa", true)]
				[InlineData("Tba", false)]
				public async Task ShouldSupportAsPrefix(string prefix, bool expectSuccess)
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument(prefix).AsPrefix();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name starting with "{{prefix}}",
						               but it was generic void ThatMethod.ClassWithMethods.GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
						               """);
				}

				[Theory]
				[InlineData("T[a-zA-Z]*r", true)]
				[InlineData("T[a-zA-Z]*s", false)]
				public async Task ShouldSupportAsRegex(string regex, bool expectSuccess)
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument(regex).AsRegex();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name matching regex "{{regex}}",
						               but it was generic void ThatMethod.ClassWithMethods.GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
						               """);
				}

				[Theory]
				[InlineData("Bar", true)]
				[InlineData("bar", false)]
				public async Task ShouldSupportAsSuffix(string suffix, bool expectSuccess)
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument(suffix).AsSuffix();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name ending with "{{suffix}}",
						               but it was generic void ThatMethod.ClassWithMethods.GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
						               """);
				}

				[Theory]
				[InlineData("T??r", true)]
				[InlineData("T??s", false)]
				public async Task ShouldSupportAsWildcard(string wildcard, bool expectSuccess)
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument(wildcard).AsWildcard();

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage($$"""
						               Expected that subject
						               is generic with argument name matching "{{wildcard}}",
						               but it was generic void ThatMethod.ClassWithMethods.GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
						               """);
				}

				[Theory]
				[InlineData(true, true)]
				[InlineData(false, false)]
				public async Task ShouldSupportIgnoringCase(bool ignoreCase, bool expectSuccess)
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument("TBAR").IgnoringCase(ignoreCase);

					await That(Act).ThrowsException()
						.OnlyIf(!expectSuccess)
						.WithMessage("""
						             Expected that subject
						             is generic with argument name equal to "TBAR",
						             but it was generic void ThatMethod.ClassWithMethods.GenericWithRestrictedSecondArgumentMethod<TFoo, TBar>()
						             """);
				}

				[Fact]
				public async Task ShouldSupportUsingCustomComparer()
				{
					MethodInfo? subject = GetMethod("GenericWithRestrictedSecondArgumentMethod");

					async Task Act()
						=> await That(subject).IsGeneric().WithArgument("TBAr")
							.Using(new IgnoreCaseForVocalsComparer());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
