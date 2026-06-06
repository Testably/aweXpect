using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsObsolete
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodIsNotObsolete_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.NonObsoleteMethod))!;

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
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

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
			public async Task WhenMethodIsObsolete_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.ObsoleteMethod))!;

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
			public async Task WhenMethodIsNotObsolete_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.NonObsoleteMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsObsolete());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodIsObsolete_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.ObsoleteMethod))!;

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
