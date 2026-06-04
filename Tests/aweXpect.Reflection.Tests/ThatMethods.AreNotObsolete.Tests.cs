using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreNotObsolete
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllMethodsAreNotObsolete_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.NonObsoleteMethod))!,
				];

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainObsoleteMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.ObsoleteMethod))!,
					typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.NonObsoleteMethod))!,
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
			public async Task WhenAllMethodsAreNotObsolete_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.NonObsoleteMethod))!,
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
			public async Task WhenMethodsContainObsoleteMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.ObsoleteMethod))!,
					typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.NonObsoleteMethod))!,
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
			public async Task WhenAllMethodsAreNotObsolete_Negated_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.NonObsoleteMethod))!,
					}
					.ToTestAsyncEnumerable<MethodInfo?>();

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
			public async Task WhenAllMethodsAreNotObsolete_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.NonObsoleteMethod))!,
					}
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainObsoleteMethods_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.ObsoleteMethod))!, typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.NonObsoleteMethod))!,
					}
					.ToTestAsyncEnumerable<MethodInfo?>();

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
#endif
	}
}
