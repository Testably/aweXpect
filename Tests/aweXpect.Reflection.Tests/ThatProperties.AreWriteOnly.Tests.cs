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
			[Test]
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

			[Test]
			public async Task WhenPropertiesContainNotWriteOnlyProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties();

				async Task Act()
				{
					await That(subject).AreWriteOnly();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all write-only,
					             but it contained not write-only properties

					             Not matching items:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFilteringOnlyWriteOnlyProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties().Where(property => property.CanWrite && !property.CanRead);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreWriteOnly());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are not all write-only,
					             but it only contained write-only properties

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
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
