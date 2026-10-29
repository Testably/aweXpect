using System.Collections.Generic;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class AreAssignableTo
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsAreAssignable_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(DerivedClass1), typeof(UnrelatedClass),];

				async Task Act()
				{
					await That(subject).AtLeast(2).AreAssignableTo<BaseClass>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is assignable to ThatTypes.BaseClass for at least 2 items,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               ThatTypes.UnrelatedClass
					             ]

					             Collection:
					             [
					               ThatTypes.DerivedClass1,
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_WhenTheNumberOfAssignableItemsMatches_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(DerivedClass1), typeof(UnrelatedClass),];

				async Task Act()
				{
					await That(subject).Exactly(1).AreAssignableTo<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsAreAssignable_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClass1), typeof(UnrelatedClass),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).AtLeast(2).AreAssignableTo<BaseClass>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is assignable to ThatTypes.BaseClass for at least 2 items,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               ThatTypes.UnrelatedClass
					             ]

					             Collection:
					             [
					               ThatTypes.DerivedClass1,
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}
		}
#endif
	}

	public sealed partial class AreAssignableFrom
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsAreAssignableFromTarget_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(BaseClass), typeof(UnrelatedClass),];

				async Task Act()
				{
					await That(subject).AtLeast(2).AreAssignableFrom<DerivedClass1>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is assignable from ThatTypes.DerivedClass1 for at least 2 items,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               ThatTypes.UnrelatedClass
					             ]

					             Collection:
					             [
					               ThatTypes.BaseClass,
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}

			[Test]
			public async Task None_WhenNoItemIsAssignableFromTarget_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(DerivedClass1), typeof(UnrelatedClass),];

				async Task Act()
				{
					await That(subject).None().AreAssignableFrom<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsAreAssignableFromTarget_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(BaseClass), typeof(UnrelatedClass),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).AtLeast(2).AreAssignableFrom<DerivedClass1>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is assignable from ThatTypes.DerivedClass1 for at least 2 items,
					             but only 1 of 2 were

					             Not matching items:
					             [
					               ThatTypes.UnrelatedClass
					             ]

					             Collection:
					             [
					               ThatTypes.BaseClass,
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}
		}
#endif
	}
}
