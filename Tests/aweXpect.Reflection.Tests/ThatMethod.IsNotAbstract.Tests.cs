using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodIsAbstract_ShouldFail()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.AbstractMethod))!;

				async Task Act()
					=> await That(subject).IsNotAbstract();

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not abstract,
					              but it was abstract {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenMethodIsNotAbstract_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.VirtualMethod))!;

				async Task Act()
					=> await That(subject).IsNotAbstract();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
					=> await That(subject).IsNotAbstract();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not abstract,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenMethodIsAbstract_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.AbstractMethod))!;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotAbstract());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodIsNotAbstract_ShouldFail()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.VirtualMethod))!;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotAbstract());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is abstract,
					             but it was non-abstract Void VirtualMethod()
					             """);
			}
		}
	}
}
