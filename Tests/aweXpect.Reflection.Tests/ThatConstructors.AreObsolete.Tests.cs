using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class AreObsolete
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllConstructorsAreObsolete_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetConstructor(Type.EmptyTypes)!,
				];

				async Task Act()
				{
					await That(subject).AreObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenConstructorsContainNonObsoleteConstructors_ShouldFail()
			{
				IEnumerable<ConstructorInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetConstructor(Type.EmptyTypes)!,
					typeof(ClassWithObsoleteMembers).GetConstructor(new[]
					{
						typeof(int),
					})!,
				];

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
			public async Task WhenAllConstructorsAreObsolete_ShouldFail()
			{
				IEnumerable<ConstructorInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetConstructor(Type.EmptyTypes)!,
				];

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
			public async Task WhenConstructorsContainNonObsoleteConstructors_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetConstructor(Type.EmptyTypes)!,
					typeof(ClassWithObsoleteMembers).GetConstructor(new[]
					{
						typeof(int),
					})!,
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
			public async Task WhenAllConstructorsAreObsolete_Negated_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetConstructor(Type.EmptyTypes)!,
					}
					.ToTestAsyncEnumerable<ConstructorInfo?>();

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
			public async Task WhenAllConstructorsAreObsolete_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetConstructor(Type.EmptyTypes)!,
					}
					.ToTestAsyncEnumerable<ConstructorInfo?>();

				async Task Act()
				{
					await That(subject).AreObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenConstructorsContainNonObsoleteConstructors_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetConstructor(Type.EmptyTypes)!, typeof(ClassWithObsoleteMembers).GetConstructor(new[]
						{
							typeof(int),
						})!,
					}
					.ToTestAsyncEnumerable<ConstructorInfo?>();

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
