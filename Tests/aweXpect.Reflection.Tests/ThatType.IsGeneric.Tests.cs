using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed partial class IsGeneric
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenArgumentComparerThrows_ShouldFail()
			{
				Type subject = typeof(PublicGenericClass<int>);

				async Task Act()
				{
					await That(subject).IsGeneric().WithArgument("T").Using(new ThrowingComparer());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is generic with argument name equal to "T" using ThatType.IsGeneric.ThrowingComparer,
					             but the comparer did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the comparer must fail the expectation instead of aborting the evaluation");
			}

			[Test]
			public async Task WhenTypeIsGeneric_ShouldSucceed()
			{
				Type subject = typeof(PublicGenericClass<int>);

				async Task Act()
				{
					await That(subject).IsGeneric();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNotGeneric_ShouldFail()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
				{
					await That(subject).IsGeneric();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is generic,
					             but it was non-generic PublicClass
					             """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsGeneric();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is generic,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenArgumentComparerThrows_ShouldFail()
			{
				Type subject = typeof(PublicGenericClass<int>);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsGeneric().WithArgument("T").Using(new ThrowingComparer()));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not generic,
					             but the comparer did throw an InvalidOperationException:
					               boom
					             """)
					.Because("a comparer that throws answers neither the expectation nor its negation");
			}

			[Test]
			public async Task WhenTypeIsGeneric_ShouldFail()
			{
				Type subject = typeof(PublicGenericClass<int>);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsGeneric());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not generic,
					             but it was generic PublicGenericClass<int>
					             """);
			}

			[Test]
			public async Task WhenTypeIsNotGeneric_ShouldSucceed()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsGeneric());
				}

				await That(Act).DoesNotThrow();
			}
		}

		private sealed class ThrowingComparer : IEqualityComparer<string>
		{
			public bool Equals(string? x, string? y) => throw new InvalidOperationException("boom");

			public int GetHashCode(string obj) => obj.GetHashCode();
		}
	}
}
