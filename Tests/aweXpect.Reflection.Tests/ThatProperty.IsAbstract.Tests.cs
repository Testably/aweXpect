using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyIsAbstract_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(AbstractClassWithMembers).GetProperty(nameof(AbstractClassWithMembers.AbstractProperty))!;

				async Task Act()
					=> await That(subject).IsAbstract();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsNotAbstract_ShouldFail()
			{
				PropertyInfo subject =
					typeof(AbstractClassWithMembers).GetProperty(nameof(AbstractClassWithMembers.VirtualProperty))!;

				async Task Act()
					=> await That(subject).IsAbstract();

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is abstract,
					              but it was non-abstract {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
					=> await That(subject).IsAbstract();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is abstract,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyIsAbstract_ShouldFail()
			{
				PropertyInfo subject =
					typeof(AbstractClassWithMembers).GetProperty(nameof(AbstractClassWithMembers.AbstractProperty))!;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsAbstract());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not abstract,
					             but it was abstract System.String AbstractProperty
					             """);
			}

			[Fact]
			public async Task WhenPropertyIsNotAbstract_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(AbstractClassWithMembers).GetProperty(nameof(AbstractClassWithMembers.VirtualProperty))!;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsAbstract());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
