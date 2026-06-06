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
			[Fact]
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

			[Fact]
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
			[Fact]
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

			[Fact]
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
