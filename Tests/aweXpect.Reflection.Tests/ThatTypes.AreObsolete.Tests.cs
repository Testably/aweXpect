using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreObsolete
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllTypesAreObsolete_ShouldSucceed()
			{
#pragma warning disable CS0612, CS0618
				IEnumerable<Type> subject =
				[
					typeof(ObsoleteClass),
				];
#pragma warning restore CS0612, CS0618

				async Task Act()
				{
					await That(subject).AreObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesContainNonObsoleteTypes_ShouldFail()
			{
#pragma warning disable CS0612, CS0618
				IEnumerable<Type> subject =
				[
					typeof(ObsoleteClass),
					typeof(ClassWithObsoleteMembers),
				];
#pragma warning restore CS0612, CS0618

				async Task Act()
				{
					await That(subject).AreObsolete();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all obsolete,
					             but it contained non-obsolete items

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
			public async Task WhenAllTypesAreObsolete_ShouldFail()
			{
#pragma warning disable CS0612, CS0618
				IEnumerable<Type> subject =
				[
					typeof(ObsoleteClass),
				];
#pragma warning restore CS0612, CS0618

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreObsolete());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are not all obsolete,
					             but it only contained obsolete items

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenTypesContainNonObsoleteTypes_ShouldSucceed()
			{
#pragma warning disable CS0612, CS0618
				IEnumerable<Type> subject =
				[
					typeof(ObsoleteClass),
					typeof(ClassWithObsoleteMembers),
				];
#pragma warning restore CS0612, CS0618

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
			public async Task WhenAllTypesAreObsolete_Negated_ShouldFail()
			{
#pragma warning disable CS0612, CS0618
				IAsyncEnumerable<Type?> subject = new[]
					{
						typeof(ObsoleteClass),
					}
					.ToTestAsyncEnumerable<Type?>();
#pragma warning restore CS0612, CS0618

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreObsolete());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are not all obsolete,
					             but it only contained obsolete items

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenAllTypesAreObsolete_ShouldSucceed()
			{
#pragma warning disable CS0612, CS0618
				IAsyncEnumerable<Type?> subject = new[]
					{
						typeof(ObsoleteClass),
					}
					.ToTestAsyncEnumerable<Type?>();
#pragma warning restore CS0612, CS0618

				async Task Act()
				{
					await That(subject).AreObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesContainNonObsoleteTypes_ShouldFail()
			{
#pragma warning disable CS0612, CS0618
				IAsyncEnumerable<Type?> subject = new[]
					{
						typeof(ObsoleteClass), typeof(ClassWithObsoleteMembers),
					}
					.ToTestAsyncEnumerable<Type?>();
#pragma warning restore CS0612, CS0618

				async Task Act()
				{
					await That(subject).AreObsolete();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all obsolete,
					             but it contained non-obsolete items

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
