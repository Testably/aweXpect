using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class AreNotObsolete
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllConstructorsAreNotObsolete_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetConstructor(new[]
					{
						typeof(int),
					})!,
				];

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenConstructorsContainObsoleteConstructors_ShouldFail()
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
			public async Task WhenAllConstructorsAreNotObsolete_ShouldFail()
			{
				IEnumerable<ConstructorInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetConstructor(new[]
					{
						typeof(int),
					})!,
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
			public async Task WhenConstructorsContainObsoleteConstructors_ShouldSucceed()
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
					await That(subject).DoesNotComplyWith(they => they.AreNotObsolete());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Test]
			public async Task WhenAllConstructorsAreNotObsolete_Negated_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetConstructor(new[]
						{
							typeof(int),
						})!,
					}
					.ToTestAsyncEnumerable<ConstructorInfo?>();

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
			public async Task WhenAllConstructorsAreNotObsolete_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetConstructor(new[]
						{
							typeof(int),
						})!,
					}
					.ToTestAsyncEnumerable<ConstructorInfo?>();

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenConstructorsContainObsoleteConstructors_ShouldFail()
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
