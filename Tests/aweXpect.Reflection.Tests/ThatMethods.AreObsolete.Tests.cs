using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class AreObsolete
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllMethodsAreObsolete_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.ObsoleteMethod))!,
				];

				async Task Act()
				{
					await That(subject).AreObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainNonObsoleteMethods_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.ObsoleteMethod))!,
					typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.NonObsoleteMethod))!,
				];

				async Task Act()
				{
					await That(subject).AreObsolete();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all obsolete,
					             but it contained non-obsolete items [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllMethodsAreObsolete_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.ObsoleteMethod))!,
				];

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreObsolete());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all obsolete,
					             but it only contained obsolete items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMethodsContainNonObsoleteMethods_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.ObsoleteMethod))!,
					typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.NonObsoleteMethod))!,
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
			[Fact]
			public async Task WhenAllMethodsAreObsolete_Negated_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.ObsoleteMethod))!,
					}
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreObsolete());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all obsolete,
					             but it only contained obsolete items [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenAllMethodsAreObsolete_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.ObsoleteMethod))!,
					}
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsContainNonObsoleteMethods_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.ObsoleteMethod))!, typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.NonObsoleteMethod))!,
					}
					.ToTestAsyncEnumerable<MethodInfo?>();

				async Task Act()
				{
					await That(subject).AreObsolete();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all obsolete,
					             but it contained non-obsolete items [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
