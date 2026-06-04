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
			[Fact]
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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not obsolete,
					             but it contained obsolete items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain an obsolete item,
					             but it only contained non-obsolete items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
			[Fact]
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

			[Fact]
			public async Task WhenFieldsContainObsoleteFields_ShouldFail()
			{
				IAsyncEnumerable<FieldInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.ObsoleteField))!,
						typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.NonObsoleteField))!,
					}
					.ToTestAsyncEnumerable<FieldInfo?>();

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all not obsolete,
					             but it contained obsolete items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             also contain an obsolete item,
					             but it only contained non-obsolete items [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
