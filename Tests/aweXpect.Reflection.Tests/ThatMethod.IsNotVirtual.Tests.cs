using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotVirtual
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodIsNotVirtual_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.RegularMethod))!;

				async Task Act()
				{
					await That(subject).IsNotVirtual();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotVirtual();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not virtual,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenMethodIsVirtual_ShouldFail()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.VirtualMethod))!;

				async Task Act()
				{
					await That(subject).IsNotVirtual();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not virtual,
					              but it was virtual {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenMethodIsNotVirtual_ShouldFail()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.RegularMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotVirtual());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is virtual,
					             but it was non-virtual void AbstractClassWithMembers.RegularMethod()
					             """);
			}

			[Fact]
			public async Task WhenMethodIsVirtual_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.VirtualMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotVirtual());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
