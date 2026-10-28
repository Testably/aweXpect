using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class DoNotReturnExactly
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenCollectionIsEmpty_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject = [];

				async Task Act()
				{
					await That(subject).DoNotReturnExactly<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOneMethodIsNull_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummy)),
					null,
				];

				async Task Act()
				{
					await That(subject).DoNotReturnExactly<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not return exactly ThatMethods.DummyBase,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy(),
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenOneMethodReturnsExactlyType_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummyBase)),
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummy)),
				];

				async Task Act()
				{
					await That(subject).DoNotReturnExactly<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not return exactly ThatMethods.DummyBase,
					             but it contained methods which return exactly ThatMethods.DummyBase

					             Not matching items:
					             [
					               ThatMethods.DummyBase ThatMethods.TestClass.GetDummyBase()
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectOnlyContainsMethodsReturningSubtypes_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummy)),
				];

				async Task Act()
				{
					await That(subject).DoNotReturnExactly<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithType_WhenOneMethodReturnsExactlyType_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummyBase)),
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummy)),
				];
				Type unexpectedType = typeof(DummyBase);

				async Task Act()
				{
					await That(subject).DoNotReturnExactly(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not return exactly ThatMethods.DummyBase,
					             but it contained methods which return exactly ThatMethods.DummyBase

					             Not matching items:
					             [
					               ThatMethods.DummyBase ThatMethods.TestClass.GetDummyBase()
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenOneMethodReturnsExactlyType_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummyBase)),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotReturnExactly<DummyBase>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectOnlyContainsMethodsReturningSubtypes_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummy)),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotReturnExactly<DummyBase>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             also contain a method which returns exactly ThatMethods.DummyBase,
					             but it only contained methods which do not return exactly ThatMethods.DummyBase

					             Collection:
					             [
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]
					             """);
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenOneMethodReturnsExactlyType_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummyBase)),
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoNotReturnExactly<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not return exactly ThatMethods.DummyBase,
					             but it contained methods which return exactly ThatMethods.DummyBase

					             Not matching items:
					             [
					               ThatMethods.DummyBase ThatMethods.TestClass.GetDummyBase()
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectOnlyContainsMethodsReturningSubtypes_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummy)),
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoNotReturnExactly<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithType_WhenOneMethodReturnsExactlyType_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummyBase)),
				}.ToTestAsyncEnumerable<MethodInfo?>();
				Type unexpectedType = typeof(DummyBase);

				async Task Act()
				{
					await That(subject).DoNotReturnExactly(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not return exactly ThatMethods.DummyBase,
					             but it contained methods which return exactly ThatMethods.DummyBase

					             Not matching items:
					             [
					               ThatMethods.DummyBase ThatMethods.TestClass.GetDummyBase()
					             ]
					             """);
			}
		}
#endif
	}
}
