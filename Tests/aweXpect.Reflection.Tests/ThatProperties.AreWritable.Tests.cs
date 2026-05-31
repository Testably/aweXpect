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
			[Fact]
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

			[Fact]
			public async Task WhenPropertiesContainNotWritableProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties();

				async Task Act()
				{
					await That(subject).AreWritable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all writable,
					             but it contained not writable properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyWritableProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties().Where(property => property.CanWrite);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreWritable());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all writable,
					             but it only contained writable properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
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
