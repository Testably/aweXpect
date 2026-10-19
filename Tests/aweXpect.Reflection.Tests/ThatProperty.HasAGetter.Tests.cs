using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class HasAGetter
	{
		public sealed class Tests
		{
			[Test]
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

			[Test]
			public async Task WhenPropertyHasNoGetter_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithPropertyAccessors)
					.GetProperty(nameof(TestClassWithPropertyAccessors.WithSetterOnly))!;

				async Task Act()
				{
					await That(subject).HasAGetter();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              has a getter,
					              but it did not have a getter public string TestClassWithPropertyAccessors.WithSetterOnly { set; }
					              """);
			}

			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).HasAGetter();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             has a getter,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertyHasAGetter_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithPropertyAccessors)
					.GetProperty(nameof(TestClassWithPropertyAccessors.WithGetterOnly))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasAGetter());
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              does not have a getter,
					              but it had a getter public string TestClassWithPropertyAccessors.WithGetterOnly { get; }
					              """);
			}

			[Test]
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
