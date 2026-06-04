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
			[Fact]
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

			[Fact]
			public async Task WhenConstructorsContainNonObsoleteConstructors_ShouldFail()
			{
				IEnumerable<ConstructorInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetConstructor(Type.EmptyTypes)!,
					typeof(ClassWithObsoleteMembers).GetConstructor(new[] { typeof(int) })!,
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
			public async Task WhenConstructorsContainNonObsoleteConstructors_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> subject =
				[
					typeof(ClassWithObsoleteMembers).GetConstructor(Type.EmptyTypes)!,
					typeof(ClassWithObsoleteMembers).GetConstructor(new[] { typeof(int) })!,
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

			[Fact]
			public async Task WhenConstructorsContainNonObsoleteConstructors_ShouldFail()
			{
				IAsyncEnumerable<ConstructorInfo?> subject = new[]
					{
						typeof(ClassWithObsoleteMembers).GetConstructor(Type.EmptyTypes)!,
						typeof(ClassWithObsoleteMembers).GetConstructor(new[] { typeof(int) })!,
					}
					.ToTestAsyncEnumerable<ConstructorInfo?>();

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
