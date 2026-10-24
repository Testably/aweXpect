using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreReadOnly
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFilteringOnlyReadOnlyProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties().Where(property => property.CanRead && !property.CanWrite);

				async Task Act()
				{
					await That(subject).AreReadOnly();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesContainNotReadOnlyProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties();

				async Task Act()
				{
					await That(subject).AreReadOnly();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are read-only,
					             but only * of * were

					             Not matching items:
					             [
					               *
					             ]

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFilteringOnlyReadOnlyProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties().Where(property => property.CanRead && !property.CanWrite);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreReadOnly());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all are read-only,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenPropertiesContainNotReadOnlyProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreReadOnly());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
