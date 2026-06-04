using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

#pragma warning disable CS0612, CS0618 // Intentional reference to an obsolete test fixture member
public sealed partial class ThatProperties
{
	public sealed class AreNotObsolete
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllPropertiesAreNotObsolete_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.NonObsoleteProperty))!,
				];

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainObsoleteProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.ObsoleteProperty))!,
					typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.NonObsoleteProperty))!,
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
			public async Task WhenAllPropertiesAreNotObsolete_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.NonObsoleteProperty))!,
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
			public async Task WhenPropertiesContainObsoleteProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.ObsoleteProperty))!,
					typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.NonObsoleteProperty))!,
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
			public async Task WhenAllPropertiesAreNotObsolete_ShouldSucceed()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.NonObsoleteProperty))!,
					}
					.ToTestAsyncEnumerable<PropertyInfo?>();

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainObsoleteProperties_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.ObsoleteProperty))!,
						typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.NonObsoleteProperty))!,
					}
					.ToTestAsyncEnumerable<PropertyInfo?>();

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
			public async Task WhenAllPropertiesAreNotObsolete_Negated_ShouldFail()
			{
				IAsyncEnumerable<PropertyInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.NonObsoleteProperty))!,
					}
					.ToTestAsyncEnumerable<PropertyInfo?>();

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
