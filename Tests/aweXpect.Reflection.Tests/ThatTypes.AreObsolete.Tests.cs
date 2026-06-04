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
			[Fact]
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

			[Fact]
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
			[Fact]
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

			[Fact]
			public async Task WhenTypesContainNonObsoleteTypes_ShouldFail()
			{
#pragma warning disable CS0612, CS0618
				IAsyncEnumerable<Type?> subject = new[]
					{
						typeof(ObsoleteClass),
						typeof(ClassWithObsoleteMembers),
					}
					.ToTestAsyncEnumerable<Type?>();
#pragma warning restore CS0612, CS0618

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

			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all obsolete,
					             but it only contained obsolete items [
					               *
					             ]
					             """).AsWildcard();
			}
		}
#endif
	}
}
