using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class HaveAGetter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllPropertiesHaveAGetter_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithPropertyAccessors)
					.GetProperties().Where(property => property.CanRead);

				async Task Act()
				{
					await That(subject).HaveAGetter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainPropertiesWithoutAGetter_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithPropertyAccessors)
					.GetProperties();

				async Task Act()
				{
					await That(subject).HaveAGetter();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             all have a getter,
					             but it contained properties without a getter [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllPropertiesHaveAGetter_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithPropertyAccessors)
					.GetProperties().Where(property => property.CanRead);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveAGetter());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             not all have a getter,
					             but it only contained properties with a getter [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertiesContainPropertiesWithoutAGetter_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithPropertyAccessors)
					.GetProperties();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveAGetter());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
