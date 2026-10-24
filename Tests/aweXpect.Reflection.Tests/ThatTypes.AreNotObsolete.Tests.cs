using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreNotObsolete
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllTypesAreNotObsolete_ShouldSucceed()
			{
				IEnumerable<Type> subject =
				[
					typeof(ClassWithObsoleteMembers),
				];

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesContainObsoleteTypes_ShouldFail()
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
			public async Task WhenAllTypesAreNotObsolete_ShouldFail()
			{
				IEnumerable<Type> subject =
				[
					typeof(ClassWithObsoleteMembers),
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
			public async Task WhenTypesContainObsoleteTypes_ShouldSucceed()
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
					await That(subject).DoesNotComplyWith(they => they.AreNotObsolete());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenAllTypesAreNotObsolete_Negated_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers),
					}
					.ToTestAsyncEnumerable<Type?>();

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
			public async Task WhenAllTypesAreNotObsolete_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers),
					}
					.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesContainObsoleteTypes_ShouldFail()
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
