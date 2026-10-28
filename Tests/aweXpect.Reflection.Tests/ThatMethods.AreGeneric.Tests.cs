using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed partial class AreGeneric
	{
		public sealed class EnumerableTests
		{
			[Test]
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

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<MethodInfo?> subject = [typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.GenericMethod1)), null,];

				async Task Act()
				{
					await That(subject).AreGeneric();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.AreGeneric());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all generic,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               T ThatMethods.ClassWithMethods.GenericMethod1<T>(T value),
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are not all generic,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               T ThatMethods.ClassWithMethods.GenericMethod1<T>(T value),
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
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

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all generic,
					             but it contained not matching methods

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class Tests
		{
			[Test]
			public async Task WhenFilteringOnlyGenericMethods_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("GenericMethod1");

				async Task Act()
				{
					await That(subject).AreGeneric();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsContainNonGenericMethods_ShouldFail()
			{
				Filtered.Methods subject = GetMethods();

				async Task Act()
				{
					await That(subject).AreGeneric();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that methods matching methodInfo => methodInfo.Name.StartsWith(methodPrefix) in types matching t => t == typeof(ClassWithMethods) in assembly containing type ThatMethods.ClassWithMethods
					             are all generic,
					             but it contained not matching methods

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFilteringOnlyGenericMethods_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("GenericMethod1");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreGeneric());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that *
					             are not all generic,
					             but it only contained generic methods

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
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
