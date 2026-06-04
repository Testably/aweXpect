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
			[Fact]
			public async Task WhenAllConstructorsAreNotObsolete_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetConstructor(new[] { typeof(int) })!,
				];

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenConstructorsContainObsoleteConstructors_ShouldFail()
			{
				IEnumerable<ConstructorInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetConstructor(Type.EmptyTypes)!,
					typeof(ClassWithObsoleteMembers).GetConstructor(new[] { typeof(int) })!,
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
			public async Task WhenAllConstructorsAreNotObsolete_ShouldFail()
			{
				IEnumerable<ConstructorInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetConstructor(new[] { typeof(int) })!,
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
			public async Task WhenConstructorsContainObsoleteConstructors_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetConstructor(Type.EmptyTypes)!,
					typeof(ClassWithObsoleteMembers).GetConstructor(new[] { typeof(int) })!,
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
			public async Task WhenAllConstructorsAreNotObsolete_ShouldSucceed()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetConstructor(new[] { typeof(int) })!,
					}
					.ToTestAsyncEnumerable<ConstructorInfo?>();

				async Task Act()
				{
					await That(subject).AreNotObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenConstructorsContainObsoleteConstructors_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetConstructor(Type.EmptyTypes)!,
						typeof(ClassWithObsoleteMembers).GetConstructor(new[] { typeof(int) })!,
					}
					.ToTestAsyncEnumerable<ConstructorInfo?>();

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

			[Fact]
			public async Task WhenAllConstructorsAreNotObsolete_Negated_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetConstructor(new[] { typeof(int) })!,
					}
					.ToTestAsyncEnumerable<ConstructorInfo?>();

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
		}
#endif
	}
}
