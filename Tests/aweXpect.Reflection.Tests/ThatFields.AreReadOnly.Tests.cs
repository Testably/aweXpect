using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreReadOnly
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFieldsContainNonReadOnlyFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ReadOnlyField))!,
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!,
				];

				async Task Act()
				{
					await That(subject).AreReadOnly();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are read-only,
					             but only * of * were

					             Not matching items:
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
			public async Task WhenFilteringOnlyReadOnlyFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ReadOnlyField))!,
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers
						.StaticReadOnlyField))!,
				];

				async Task Act()
				{
					await That(subject).AreReadOnly();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFieldsContainNonReadOnlyFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ReadOnlyField))!,
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreReadOnly());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyReadOnlyFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ReadOnlyField))!,
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers
						.StaticReadOnlyField))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreReadOnly());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all are read-only,
					             but all * were

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
			public async Task WhenFieldsContainNonReadOnlyFields_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ReadOnlyField))!, typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!,
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreReadOnly();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are read-only,
					             but only * of * were

					             Not matching items:
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
			public async Task WhenFilteringOnlyReadOnlyFields_Negated_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ReadOnlyField))!, typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers
						.StaticReadOnlyField))!,
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreReadOnly());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all are read-only,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyReadOnlyFields_ShouldSucceed()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ReadOnlyField))!, typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers
						.StaticReadOnlyField))!,
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreReadOnly();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
