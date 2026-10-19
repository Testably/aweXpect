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
			[Test]
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

			[Test]
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

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all not obsolete,
					             but it contained obsolete items

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
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

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             also contain an obsolete item,
					             but it only contained non-obsolete items

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
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
			[Test]
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

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             also contain an obsolete item,
					             but it only contained non-obsolete items

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
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

			[Test]
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

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all not obsolete,
					             but it contained obsolete items

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
