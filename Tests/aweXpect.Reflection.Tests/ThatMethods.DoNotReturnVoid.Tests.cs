using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class DoNotReturnVoid
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenCollectionIsEmpty_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject = [];

				async Task Act()
				{
					await That(subject).DoNotReturnVoid();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNoMethodReturnsVoid_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt)),
					typeof(TestClass).GetMethod(nameof(TestClass.GetString)),
				];

				async Task Act()
				{
					await That(subject).DoNotReturnVoid();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOneMethodIsNull_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt)),
					null,
				];

				async Task Act()
				{
					await That(subject).DoNotReturnVoid();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none return void,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               int ThatMethods.TestClass.GetInt(),
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenOneMethodReturnsVoid_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt)),
					typeof(TestClass).GetMethod(nameof(TestClass.VoidMethod)),
				];

				async Task Act()
				{
					await That(subject).DoNotReturnVoid();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none return void,
					             but 1 of 2 did

					             Matching items:
					             [
					               void ThatMethods.TestClass.VoidMethod()
					             ]

					             Collection:
					             [
					               int ThatMethods.TestClass.GetInt(),
					               void ThatMethods.TestClass.VoidMethod()
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNoMethodReturnsVoid_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt)),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotReturnVoid());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             at least one returns void,
					             but none of 1 did

					             Collection:
					             [
					               int ThatMethods.TestClass.GetInt()
					             ]
					             """);
			}

			[Test]
			public async Task WhenOneMethodReturnsVoid_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.VoidMethod)),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotReturnVoid());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenNoMethodReturnsVoid_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt)),
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoNotReturnVoid();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOneMethodReturnsVoid_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.VoidMethod)),
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoNotReturnVoid();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             none return void,
					             but 1 of 1 did

					             Matching items:
					             [
					               void ThatMethods.TestClass.VoidMethod()
					             ]

					             Collection:
					             [
					               void ThatMethods.TestClass.VoidMethod()
					             ]
					             """);
			}
		}
#endif
	}
}
