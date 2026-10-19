using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreReadable
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFilteringOnlyReadableProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties().Where(property => property.CanRead);

				async Task Act()
				{
					await That(subject).AreReadable();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertiesContainNotReadableProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties();

				async Task Act()
				{
					await That(subject).AreReadable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are all readable,
					             but it contained not readable properties

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
			public async Task WhenFilteringOnlyReadableProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties().Where(property => property.CanRead);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreReadable());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             are not all readable,
					             but it only contained readable properties

					             Collection:
					             [
					               *
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenPropertiesContainNotReadableProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreReadable());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
