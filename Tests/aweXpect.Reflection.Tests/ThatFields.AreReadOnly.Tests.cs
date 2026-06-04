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
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all read-only,
					             but it contained non-read-only fields [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all read-only,
					             but it only contained read-only fields [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenFieldsContainNonReadOnlyFields_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new FieldInfo[]
				{
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ReadOnlyField))!,
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!,
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreReadOnly();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all read-only,
					             but it contained non-read-only fields [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenFilteringOnlyReadOnlyFields_ShouldSucceed()
			{
				IAsyncEnumerable<FieldInfo?> subject = new FieldInfo[]
				{
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ReadOnlyField))!,
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers
						.StaticReadOnlyField))!,
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreReadOnly();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFilteringOnlyReadOnlyFields_Negated_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new FieldInfo[]
				{
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ReadOnlyField))!,
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers
						.StaticReadOnlyField))!,
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreReadOnly());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all read-only,
					             but it only contained read-only fields [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
