using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class DoesNotOverride
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyDoesNotOverride_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(AbstractClassWithMembers).GetProperty(nameof(AbstractClassWithMembers.VirtualProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotOverride();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).DoesNotOverride();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             does not override a base property,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenPropertyOverrides_ShouldFail()
			{
				PropertyInfo subject =
					typeof(ClassWithSealedMembers).GetProperty(nameof(ClassWithSealedMembers.VirtualProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotOverride();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              does not override a base property,
					              but it did override a base property {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyDoesNotOverride_ShouldFail()
			{
				PropertyInfo subject =
					typeof(AbstractClassWithMembers).GetProperty(nameof(AbstractClassWithMembers.VirtualProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotOverride());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             overrides a base property,
					             but it did not override a base property public string AbstractClassWithMembers.VirtualProperty { get; set; }
					             """);
			}

			[Fact]
			public async Task WhenPropertyOverrides_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(ClassWithSealedMembers).GetProperty(nameof(ClassWithSealedMembers.VirtualProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotOverride());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
