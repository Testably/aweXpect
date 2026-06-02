using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class HasAGetter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyHasAGetter_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClassWithPropertyAccessors)
					.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterOnly))!;

				async Task Act()
				{
					await That(subject).HasAGetter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyHasNoGetter_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithPropertyAccessors)
					.GetProperty(nameof(TestClassWithPropertyAccessors.WithSetterOnly))!;

				async Task Act()
				{
					await That(subject).HasAGetter();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              has a getter,
					              but it did not have a getter {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).HasAGetter();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             has a getter,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyHasAGetter_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithPropertyAccessors)
					.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterOnly))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasAGetter());
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              does not have a getter,
					              but it had a getter {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPropertyHasNoGetter_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClassWithPropertyAccessors)
					.GetProperty(nameof(TestClassWithPropertyAccessors.WithSetterOnly))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasAGetter());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
