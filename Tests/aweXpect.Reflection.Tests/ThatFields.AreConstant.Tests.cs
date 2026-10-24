using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatFields
{
	public sealed class AreConstant
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFieldsContainNonConstantFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ConstantField))!,
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!,
				];

				async Task Act()
				{
					await That(subject).AreConstant();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are constant,
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
			public async Task WhenFilteringOnlyConstantFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ConstantField))!,
				];

				async Task Act()
				{
					await That(subject).AreConstant();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFieldsContainNonConstantFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ConstantField))!,
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreConstant());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFilteringOnlyConstantFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ConstantField))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreConstant());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all are constant,
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
			public async Task WhenFieldsContainNonConstantFields_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ConstantField))!, typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!,
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreConstant();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are constant,
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
			public async Task WhenFilteringOnlyConstantFields_Negated_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ConstantField))!,
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreConstant());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all are constant,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFilteringOnlyConstantFields_ShouldSucceed()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
				{
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ConstantField))!,
				}.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreConstant();
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
