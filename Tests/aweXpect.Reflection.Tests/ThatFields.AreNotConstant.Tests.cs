using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreNotConstant
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFieldsContainConstantFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ConstantField))!,
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!,
				];

				async Task Act()
				{
					await That(subject).AreNotConstant();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all not constant,
					             but it contained constant fields

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyNonConstantFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!,
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ReadOnlyField))!,
				];

				async Task Act()
				{
					await That(subject).AreNotConstant();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFieldsContainConstantFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ConstantField))!,
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotConstant());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyNonConstantFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!,
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ReadOnlyField))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotConstant());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             also contain a constant field,
					             but it only contained non-constant fields

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
			public async Task WhenFieldsContainConstantFields_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ConstantField))!, typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!,
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreNotConstant();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all not constant,
					             but it contained constant fields

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyNonConstantFields_ShouldSucceed()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!, typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ReadOnlyField))!,
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreNotConstant();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
