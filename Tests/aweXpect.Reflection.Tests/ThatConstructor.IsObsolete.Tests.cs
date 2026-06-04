using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class IsObsolete
	{
		public sealed class Tests
		{
			[Fact]
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

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is obsolete,
					              but it was non-obsolete {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenConstructorIsNull_ShouldFail()
			{
				ConstructorInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsObsolete();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is obsolete,
					             but it was <null>
					             """);
			}

			[Fact]
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
			[Fact]
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

			[Fact]
			public async Task WhenConstructorIsObsolete_ShouldFail()
			{
				ConstructorInfo subject =
					typeof(ClassWithObsoleteMembers).GetConstructor(Type.EmptyTypes)!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsObsolete());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not obsolete,
					              but it was obsolete {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
