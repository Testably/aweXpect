using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreNotReadOnly
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFieldsContainReadOnlyFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ReadOnlyField))!,
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!,
				];

				async Task Act()
				{
					await That(subject).AreNotReadOnly();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are read-only,
					             but * of * were

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyNonReadOnlyFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!,
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ConstantField))!,
				];

				async Task Act()
				{
					await That(subject).AreNotReadOnly();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFieldsContainReadOnlyFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ReadOnlyField))!,
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotReadOnly());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyNonReadOnlyFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!,
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ConstantField))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotReadOnly());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one is read-only,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenFieldsContainReadOnlyFields_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ReadOnlyField))!, typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!,
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreNotReadOnly();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are read-only,
					             but * of * were

					             Matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyNonReadOnlyFields_ShouldSucceed()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!, typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ConstantField))!,
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreNotReadOnly();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
