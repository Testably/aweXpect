using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

#pragma warning disable CS0612, CS0618 // Intentional reference to an obsolete test fixture member
public sealed partial class ThatFields
{
	public sealed class AreObsolete
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllFieldsAreObsolete_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.ObsoleteField))!,
				];

				async Task Act()
				{
					await That(subject).AreObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldsContainNonObsoleteFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.ObsoleteField))!,
					typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.NonObsoleteField))!,
				];

				async Task Act()
				{
					await That(subject).AreObsolete();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are obsolete,
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
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllFieldsAreObsolete_ShouldFail()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.ObsoleteField))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreObsolete());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all are obsolete,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFieldsContainNonObsoleteFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.ObsoleteField))!,
					typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.NonObsoleteField))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreObsolete());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenAllFieldsAreObsolete_Negated_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.ObsoleteField))!,
					}
					.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreObsolete());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all are obsolete,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenAllFieldsAreObsolete_ShouldSucceed()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.ObsoleteField))!,
					}
					.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldsContainNonObsoleteFields_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.ObsoleteField))!, typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.NonObsoleteField))!,
					}
					.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreObsolete();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are obsolete,
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
		}
#endif
	}
}
