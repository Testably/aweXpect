using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodIsNotSealed_ShouldFail()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.VirtualMethod))!;

				async Task Act()
					=> await That(subject).IsSealed();

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is sealed,
					              but it was non-sealed {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
					=> await That(subject).IsSealed();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is sealed,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenMethodIsSealed_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithSealedMembers).GetMethod(nameof(ClassWithSealedMembers.VirtualMethod))!;

				async Task Act()
					=> await That(subject).IsSealed();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenMethodIsNotSealed_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.VirtualMethod))!;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsSealed());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodIsSealed_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithSealedMembers).GetMethod(nameof(ClassWithSealedMembers.VirtualMethod))!;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsSealed());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not sealed,
					             but it was sealed void ClassWithSealedMembers.VirtualMethod()
					             """);
			}
		}
	}
}
