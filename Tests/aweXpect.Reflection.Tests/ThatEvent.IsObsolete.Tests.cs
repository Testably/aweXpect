using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsObsolete
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventIsNotObsolete_ShouldFail()
			{
				EventInfo subject =
					typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.NonObsoleteEvent))!;

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
			public async Task WhenEventIsNull_ShouldFail()
			{
				EventInfo? subject = null;

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
			public async Task WhenEventIsObsolete_ShouldSucceed()
			{
				EventInfo subject =
					typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.ObsoleteEvent))!;

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
			public async Task WhenEventIsNotObsolete_ShouldSucceed()
			{
				EventInfo subject =
					typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.NonObsoleteEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsObsolete());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventIsObsolete_ShouldFail()
			{
				EventInfo subject =
					typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.ObsoleteEvent))!;

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
