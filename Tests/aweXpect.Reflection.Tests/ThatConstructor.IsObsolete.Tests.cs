using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class IsObsolete
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenConstructorIsNotObsolete_ShouldFail()
			{
				ConstructorInfo subject =
					typeof(ClassWithObsoleteMembers).GetConstructor(new[]
					{
						typeof(int),
					})!;

				async Task Act()
				{
					await That(subject).IsObsolete();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is obsolete,
					              but it was non-obsolete ClassWithObsoleteMembers(int value)
					              """);
			}

			[Test]
			public async Task WhenConstructorIsNull_ShouldFail()
			{
				ConstructorInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsObsolete();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is obsolete,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenConstructorIsObsolete_ShouldSucceed()
			{
				ConstructorInfo subject =
					typeof(ClassWithObsoleteMembers).GetConstructor(Type.EmptyTypes)!;

				async Task Act()
				{
					await That(subject).IsObsolete();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenConstructorIsNotObsolete_ShouldSucceed()
			{
				ConstructorInfo subject =
					typeof(ClassWithObsoleteMembers).GetConstructor(new[]
					{
						typeof(int),
					})!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsObsolete());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenConstructorIsObsolete_ShouldFail()
			{
				ConstructorInfo subject =
					typeof(ClassWithObsoleteMembers).GetConstructor(Type.EmptyTypes)!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsObsolete());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					              Expected that subject
					              is not obsolete,
					              but it was obsolete ClassWithObsoleteMembers()
					              """);
			}
		}
	}
}
