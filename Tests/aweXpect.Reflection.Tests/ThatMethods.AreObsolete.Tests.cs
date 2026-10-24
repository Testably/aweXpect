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
			[Test]
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

			[Test]
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
			[Test]
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

			[Test]
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
