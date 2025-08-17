using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class HaveName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodInfosContainMethodInfoWithDifferentName_ShouldFail()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithMethods>().Methods();

				async Task Act()
					=> await That(subject).HaveName("PublicMethod");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods in types matching t => t == typeof(T) in assembly containing type ThatMethod.ClassWithMethods
					             all have name equal to "PublicMethod",
					             but it contained not matching items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodInfosHaveName_ShouldSucceed()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithSingleMethod>().Methods();

				async Task Act()
					=> await That(subject).HaveName("MyMethod");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodInfosMatchIgnoringCase_ShouldSucceed()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithSingleMethod>().Methods();

				async Task Act()
					=> await That(subject).HaveName("mYmethod").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodInfosMatchSuffix_ShouldSucceed()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithMethods>().Methods();

				async Task Act()
					=> await That(subject).HaveName("Method").AsSuffix();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenMethodsDoNotHaveName_ShouldSucceed()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithMethods>().Methods();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.HaveName("NonExistentMethod"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsHaveName_ShouldFail()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithSingleMethod>().Methods();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(they => they.HaveName("MyMethod"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods in types matching t => t == typeof(T) in assembly containing type ThatMethod.ClassWithSingleMethod
					             not all have name equal to "MyMethod",
					             but it only contained matching items *
					             """).AsWildcard();
			}
		}
	}
}
