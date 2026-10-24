using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreWritable
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFilteringOnlyWritableProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties().Where(property => property.CanWrite);

				async Task Act()
				{
					await That(subject).AreWritable();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesContainNotWritableProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties();

				async Task Act()
				{
					await That(subject).AreWritable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             all are writable,
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
			public async Task WhenFilteringOnlyWritableProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties().Where(property => property.CanWrite);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreWritable());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             not all are writable,
					             but all * were

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenPropertiesContainNotWritableProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreWritable());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
