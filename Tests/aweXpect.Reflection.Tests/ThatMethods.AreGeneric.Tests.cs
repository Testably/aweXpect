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
					             all are generic,
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
					             not all are generic,
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
					             all are generic,
					             but none of 1 were

					             Not matching items:
					             [
					               int ThatMethods.ClassWithMethods.NonGenericMethod1()
					             ]

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.NonGenericMethod1()
					             ]
					             """);
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
					             all are generic,
					             but only 4 of 18 were

					             Not matching items:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1(),
					               int ThatMethods.ClassWithMethods.PublicMethod2(),
					               int ThatMethods.ClassWithMethods.InternalMethod1(),
					               int ThatMethods.ClassWithMethods.InternalMethod2(),
					               int ThatMethods.ClassWithMethods.ProtectedMethod1(),
					               int ThatMethods.ClassWithMethods.ProtectedMethod2(),
					               int ThatMethods.ClassWithMethods.PrivateMethod1(),
					               int ThatMethods.ClassWithMethods.PrivateMethod2(),
					               int ThatMethods.ClassWithMethods.ProtectedInternalMethod1(),
					               int ThatMethods.ClassWithMethods.ProtectedInternalMethod2(),
					               (… and 4 more)
					             ]

					             Collection:
					             [
					               int ThatMethods.ClassWithMethods.PublicMethod1(),
					               int ThatMethods.ClassWithMethods.PublicMethod2(),
					               int ThatMethods.ClassWithMethods.InternalMethod1(),
					               int ThatMethods.ClassWithMethods.InternalMethod2(),
					               int ThatMethods.ClassWithMethods.ProtectedMethod1(),
					               int ThatMethods.ClassWithMethods.ProtectedMethod2(),
					               int ThatMethods.ClassWithMethods.PrivateMethod1(),
					               int ThatMethods.ClassWithMethods.PrivateMethod2(),
					               int ThatMethods.ClassWithMethods.ProtectedInternalMethod1(),
					               int ThatMethods.ClassWithMethods.ProtectedInternalMethod2(),
					               (… and 8 more)
					             ]
					             """);
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
					             Expected that methods matching methodInfo => methodInfo.Name.StartsWith(methodPrefix) in types matching t => t == typeof(ClassWithMethods) in assembly containing type ThatMethods.ClassWithMethods
					             not all are generic,
					             but all 1 were

					             Collection:
					             [
					               T ThatMethods.ClassWithMethods.GenericMethod1<T>(T value)
					             ]
					             """);
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
