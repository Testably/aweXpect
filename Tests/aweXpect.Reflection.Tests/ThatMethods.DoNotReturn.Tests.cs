using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class DoNotReturn
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenCollectionIsEmpty_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject = [];

				async Task Act()
				{
					await That(subject).DoNotReturn<string>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNoMethodReturnsType_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt)),
					typeof(TestClass).GetMethod(nameof(TestClass.GetBool)),
				];

				async Task Act()
				{
					await That(subject).DoNotReturn<string>();
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
					await That(subject).DoNotReturn<string>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not return string,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               int ThatMethods.TestClass.GetInt(),
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task WhenOneMethodReturnsSubtype_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt)),
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummy)),
				];

				async Task Act()
				{
					await That(subject).DoNotReturn<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not return ThatMethods.DummyBase,
					             but it contained methods which return ThatMethods.DummyBase

					             Not matching items:
					             [
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]
					             """);
			}

			[Test]
			public async Task WhenOneMethodReturnsType_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetString)),
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt)),
				];

				async Task Act()
				{
					await That(subject).DoNotReturn<string>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not return string,
					             but it contained methods which return string

					             Not matching items:
					             [
					               string ThatMethods.TestClass.GetString()
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<MethodInfo?>? subject = null;

				async Task Act()
				{
					await That(subject).DoNotReturn<string>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not return string,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WithType_WhenOneMethodReturnsType_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetString)),
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt)),
				];
				Type unexpectedType = typeof(int);

				async Task Act()
				{
					await That(subject).DoNotReturn(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not return int,
					             but it contained methods which return int

					             Not matching items:
					             [
					               int ThatMethods.TestClass.GetInt()
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNoMethodReturnsType_ShouldFail()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt)),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotReturn<string>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             also contain a method which returns string,
					             but it only contained methods which do not return string

					             Collection:
					             [
					               int ThatMethods.TestClass.GetInt()
					             ]
					             """);
			}

			[Test]
			public async Task WhenOneMethodReturnsType_ShouldSucceed()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetString)),
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt)),
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.DoNotReturn<string>());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenNoMethodReturnsType_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt)),
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoNotReturn<string>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOneMethodReturnsType_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.GetString)),
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt)),
				}.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoNotReturn<string>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not return string,
					             but it contained methods which return string

					             Not matching items:
					             [
					               string ThatMethods.TestClass.GetString()
					             ]
					             """);
			}

			[Test]
			public async Task WithType_WhenOneMethodReturnsType_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.GetString)),
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt)),
				}.ToTestAsyncEnumerable<MethodInfo?>();
				Type unexpectedType = typeof(string);

				async Task Act()
				{
					await That(subject).DoNotReturn(unexpectedType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all do not return string,
					             but it contained methods which return string

					             Not matching items:
					             [
					               string ThatMethods.TestClass.GetString()
					             ]
					             """);
			}
		}
#endif
	}
}
