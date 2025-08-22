using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsNotSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyIsNotSealed_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(AbstractClassWithMembers).GetProperty(nameof(AbstractClassWithMembers.VirtualProperty))!;

				async Task Act()
					=> await That(subject).IsNotSealed();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
					=> await That(subject).IsNotSealed();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not sealed,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenPropertyIsSealed_ShouldFail()
			{
				PropertyInfo subject =
					typeof(ClassWithSealedMembers).GetProperty(nameof(ClassWithSealedMembers.VirtualProperty))!;

				async Task Act()
					=> await That(subject).IsNotSealed();

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not sealed,
					              but it was sealed {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyIsNotSealed_ShouldFail()
			{
				PropertyInfo subject =
					typeof(AbstractClassWithMembers).GetProperty(nameof(AbstractClassWithMembers.VirtualProperty))!;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotSealed());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is sealed,
					             but it was non-sealed public string AbstractClassWithMembers.VirtualProperty { get; set; }
					             """);
			}

			[Fact]
			public async Task WhenPropertyIsSealed_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(ClassWithSealedMembers).GetProperty(nameof(ClassWithSealedMembers.VirtualProperty))!;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotSealed());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
