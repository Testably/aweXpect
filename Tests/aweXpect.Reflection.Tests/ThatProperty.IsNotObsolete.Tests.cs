using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

#pragma warning disable CS0612, CS0618 // Intentional reference to an obsolete test fixture member
public sealed partial class ThatProperty
{
	public sealed class IsNotObsolete
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyIsNotObsolete_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.NonObsoleteProperty))!;

				async Task Act()
				{
					await That(subject).IsNotObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotObsolete();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not obsolete,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenPropertyIsObsolete_ShouldFail()
			{
				PropertyInfo subject =
					typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.ObsoleteProperty))!;

				async Task Act()
				{
					await That(subject).IsNotObsolete();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not obsolete,
					              but it was obsolete {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyIsNotObsolete_ShouldFail()
			{
				PropertyInfo subject =
					typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.NonObsoleteProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotObsolete());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is obsolete,
					              but it was non-obsolete {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPropertyIsObsolete_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.ObsoleteProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotObsolete());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
