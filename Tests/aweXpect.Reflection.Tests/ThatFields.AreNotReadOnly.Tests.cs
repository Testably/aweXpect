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
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not read-only,
					             but it contained read-only fields [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain a read-only field,
					             but it only contained non-read-only fields [
					               *
					             ]
					             """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not read-only,
					             but it contained read-only fields [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
