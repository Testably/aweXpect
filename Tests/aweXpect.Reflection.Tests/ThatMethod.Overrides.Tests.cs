using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class Overrides
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMethodDoesNotOverride_ShouldFail()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.VirtualMethod))!;

				async Task Act()
				{
					await That(subject).Overrides();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              overrides a base method,
					              but it did not override a base method {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).Overrides();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             overrides a base method,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenMethodOverrides_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithSealedMembers).GetMethod(nameof(ClassWithSealedMembers.VirtualMethod))!;

				async Task Act()
				{
					await That(subject).Overrides();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenMethodDoesNotOverride_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.VirtualMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Overrides());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodOverrides_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithSealedMembers).GetMethod(nameof(ClassWithSealedMembers.VirtualMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Overrides());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not override a base method,
					             but it did override a base method void ClassWithSealedMembers.VirtualMethod()
					             """);
			}
		}
	}
}
