using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class HaveName
	{
		public sealed class EnumerableTests
		{
			[Fact]
			public async Task WhenAllMethodsHaveName_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1))!,
				];

				async Task Act()
				{
					await That(subject).HaveName("PublicMethod1");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSelectorMatchesEachName_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1))!,
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod2))!,
				];

				async Task Act()
				{
					await That(subject).HaveName(method => method.Name);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMultipleMethodsDoNotMatchSelector_ShouldListAllSeparatedByComma()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1))!,
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod2))!,
				];

				async Task Act()
				{
					await That(subject).HaveName(method => method.Name + "X");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all have name matching method => method.Name + "X",
					             but it contained not matching items [
					               int ThatMethods.ClassWithMethods.PublicMethod1() with name "PublicMethod1" instead of "PublicMethod1X",
					               int ThatMethods.ClassWithMethods.PublicMethod2() with name "PublicMethod2" instead of "PublicMethod2X"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSelectorMatchesEachName_Negated_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.PublicMethod1))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName(method => method.Name));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             not all have name matching method => method.Name,
					             but it only contained matching items [
					               int ThatMethods.ClassWithMethods.PublicMethod1()
					             ]
					             """);
			}
		}

		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodInfosContainMethodInfoWithDifferentName_ShouldFail()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithMethods>().Methods();

				async Task Act()
				{
					await That(subject).HaveName("PublicMethod");
				}

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
				{
					await That(subject).HaveName("MyMethod");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodInfosMatchIgnoringCase_ShouldSucceed()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithSingleMethod>().Methods();

				async Task Act()
				{
					await That(subject).HaveName("mYmethod").IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodInfosMatchSuffix_ShouldSucceed()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithMethods>().Methods();

				async Task Act()
				{
					await That(subject).HaveName("Method").AsSuffix();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSelectorMatchesAsPrefix_ShouldSucceed()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithSingleMethod>().Methods();

				async Task Act()
				{
					await That(subject).HaveName(_ => "My").AsPrefix();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSelectorMatchesIgnoringCase_ShouldSucceed()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithSingleMethod>().Methods();

				async Task Act()
				{
					await That(subject).HaveName(method => method.Name.ToUpperInvariant()).IgnoringCase();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSelectorReturnsDifferentName_ShouldFail()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithSingleMethod>().Methods();

				async Task Act()
				{
					await That(subject).HaveName(method => method.Name + "X");
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods in types matching t => t == typeof(T) in assembly containing type ThatMethod.ClassWithSingleMethod
					             all have name matching method => method.Name + "X",
					             but it contained not matching items [
					               * with name "MyMethod" instead of "MyMethodX"
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenSelectorReturnsEachMethodsOwnName_ShouldSucceed()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithMethods>().Methods();

				async Task Act()
				{
					await That(subject).HaveName(method => method.Name);
				}

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
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName("NonExistentMethod"));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsHaveName_ShouldFail()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithSingleMethod>().Methods();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName("MyMethod"));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods in types matching t => t == typeof(T) in assembly containing type ThatMethod.ClassWithSingleMethod
					             not all have name equal to "MyMethod",
					             but it only contained matching items *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenSelectorMatchesEveryName_ShouldFail()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithSingleMethod>().Methods();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName(method => method.Name));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods in types matching t => t == typeof(T) in assembly containing type ThatMethod.ClassWithSingleMethod
					             not all have name matching method => method.Name,
					             but it only contained matching items *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenSelectorMatchesNoName_ShouldSucceed()
			{
				Filtered.Methods subject = GetTypes<ThatMethod.ClassWithSingleMethod>().Methods();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveName(method => method.Name + "X"));
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
