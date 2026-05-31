using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed partial class AreGeneric
	{
		public sealed class EnumerableTests
		{
			[Fact]
			public async Task WhenAllMethodsAreGeneric_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.GenericMethod1))!,
				];

				async Task Act()
				{
					await That(subject).AreGeneric();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodIsNotGeneric_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.NonGenericMethod1))!,
				];

				async Task Act()
				{
					await That(subject).AreGeneric();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             are all generic,
					             but it contained not matching methods [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyGenericMethods_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("GenericMethod1");

				async Task Act()
				{
					await That(subject).AreGeneric();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainNonGenericMethods_ShouldFail()
			{
				Filtered.Methods subject = GetMethods();

				async Task Act()
				{
					await That(subject).AreGeneric();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that methods matching methodInfo => methodInfo.Name.StartsWith(methodPrefix) in types matching t => t == typeof(ClassWithMethods) in assembly containing type ThatMethods.ClassWithMethods
					             are all generic,
					             but it contained not matching methods [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyGenericMethods_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("GenericMethod1");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreGeneric());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that *
					             are not all generic,
					             but it only contained generic methods [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodsContainNonGenericMethods_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreGeneric());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
