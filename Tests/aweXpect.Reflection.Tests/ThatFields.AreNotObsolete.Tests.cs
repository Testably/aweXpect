using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

#pragma warning disable CS0612, CS0618 // Intentional reference to an obsolete test fixture member
public sealed partial class ThatFields
{
	public sealed class AreNotObsolete
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllFieldsAreNotObsolete_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.NonObsoleteField))!,
				];

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldsContainObsoleteFields_ShouldFail()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.ObsoleteField))!,
					typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.NonObsoleteField))!,
				];

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are obsolete,
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
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllFieldsAreNotObsolete_ShouldFail()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.NonObsoleteField))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotObsolete());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one is obsolete,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenFieldsContainObsoleteFields_ShouldSucceed()
			{
				IEnumerable<FieldInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.ObsoleteField))!,
					typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.NonObsoleteField))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotObsolete());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenAllFieldsAreNotObsolete_Negated_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.NonObsoleteField))!,
					}
					.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotObsolete());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             at least one is obsolete,
					             but none of * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenAllFieldsAreNotObsolete_ShouldSucceed()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.NonObsoleteField))!,
					}
					.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldsContainObsoleteFields_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.ObsoleteField))!, typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.NonObsoleteField))!,
					}
					.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             none are obsolete,
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
		}
#endif
	}
}
