using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreWriteOnly
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyWriteOnlyProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties().Where(property => property.CanWrite && !property.CanRead);

				async Task Act()
				{
					await That(subject).AreWriteOnly();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainNotWriteOnlyProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties();

				async Task Act()
				{
					await That(subject).AreWriteOnly();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all write-only,
					             but it contained not write-only properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyWriteOnlyProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties().Where(property => property.CanWrite && !property.CanRead);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreWriteOnly());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all write-only,
					             but it only contained write-only properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertiesContainNotWriteOnlyProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreWriteOnly());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
