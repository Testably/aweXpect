using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class HasAnInitSetter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyHasAnInitSetter_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClassWithPropertyAccessors)
					.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndInitSetter))!;

				async Task Act()
				{
					await That(subject).HasAnInitSetter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyHasARegularSetter_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithPropertyAccessors)
					.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndSetter))!;

				async Task Act()
				{
					await That(subject).HasAnInitSetter();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              has an init setter,
					              but it did not have an init setter {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).HasAnInitSetter();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             has an init setter,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyHasAnInitSetter_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithPropertyAccessors)
					.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndInitSetter))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasAnInitSetter());
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              does not have an init setter,
					              but it had an init setter {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPropertyHasARegularSetter_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClassWithPropertyAccessors)
					.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterAndSetter))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasAnInitSetter());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
