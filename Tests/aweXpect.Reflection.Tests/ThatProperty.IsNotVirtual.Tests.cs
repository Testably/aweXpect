using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsNotVirtual
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyIsNotVirtual_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(BaseClassWithMembers).GetProperty(nameof(BaseClassWithMembers.BaseProperty))!;

				async Task Act()
				{
					await That(subject).IsNotVirtual();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

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
			public async Task WhenPropertyIsVirtual_ShouldFail()
			{
				PropertyInfo subject =
					typeof(AbstractClassWithMembers).GetProperty(nameof(AbstractClassWithMembers.VirtualProperty))!;

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
			public async Task WhenPropertyIsNotVirtual_ShouldFail()
			{
				PropertyInfo subject =
					typeof(BaseClassWithMembers).GetProperty(nameof(BaseClassWithMembers.BaseProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotVirtual());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is virtual,
					             but it was non-virtual public int BaseClassWithMembers.BaseProperty { get; set; }
					             """);
			}

			[Fact]
			public async Task WhenPropertyIsVirtual_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(AbstractClassWithMembers).GetProperty(nameof(AbstractClassWithMembers.VirtualProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotVirtual());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
