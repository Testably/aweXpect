using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperties
{
	public sealed class AreReadWrite
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFilteringOnlyReadWriteProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties().Where(property => property.CanRead && property.CanWrite);

				async Task Act()
				{
					await That(subject).AreReadWrite();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertiesContainNotReadWriteProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties();

				async Task Act()
				{
					await That(subject).AreReadWrite();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are all read-write,
					             but it contained not read-write properties [
					               *
					             ]
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFilteringOnlyReadWriteProperties_ShouldFail()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties().Where(property => property.CanRead && property.CanWrite);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreReadWrite());
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             are not all read-write,
					             but it only contained read-write properties [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenPropertiesContainNotReadWriteProperties_ShouldSucceed()
			{
				IEnumerable<PropertyInfo> subject = typeof(TestClassWithReadWriteProperties)
					.GetProperties();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreReadWrite());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
