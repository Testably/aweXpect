using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreNotGeneric
	{
		public sealed class EnumerableTests
		{
			[Test]
			public async Task WhenAllMethodsAreNotGeneric_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.NonGenericMethod1))!,
				];

				async Task Act()
				{
					await That(subject).AreNotGeneric();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<MethodInfo?> subject = [typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.NonGenericMethod1)), null,];

				async Task Act()
				{
					await That(subject).AreNotGeneric();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotGeneric());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not generic,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.NonGenericMethod1(),
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             also contain a generic method,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.NonGenericMethod1(),
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenMethodIsGeneric_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithMethods).GetMethod(nameof(ClassWithMethods.GenericMethod1))!,
				];

				async Task Act()
				{
					await That(subject).AreNotGeneric();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             are all not generic,
					             but it contained generic methods

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
			public async Task WhenFilteringOnlyGenericMethods_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("GenericMethod");

				async Task Act()
				{
					await That(subject).AreNotGeneric();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that methods matching methodInfo => methodInfo.Name.StartsWith(methodPrefix) in types matching t => t == typeof(ClassWithMethods) in assembly containing type ThatMethods.ClassWithMethods
					             are all not generic,
					             but it contained generic methods

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenMethodsContainNonGenericMethods_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("NonGenericMethod");

				async Task Act()
				{
					await That(subject).AreNotGeneric();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFilteringOnlyGenericMethods_ShouldSucceed()
			{
				Filtered.Methods subject = GetMethods("GenericMethod");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotGeneric());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsContainNonGenericMethods_ShouldFail()
			{
				Filtered.Methods subject = GetMethods("NonGenericMethod");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotGeneric());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that *
					             also contain a generic method,
					             but it only contained non-generic methods

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
	}
}
