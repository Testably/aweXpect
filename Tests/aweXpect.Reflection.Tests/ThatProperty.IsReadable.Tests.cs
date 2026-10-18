using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsReadable
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsReadable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is readable,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenPropertyIsReadable_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClassWithReadWriteProperties)
					.GetProperty(nameof(TestClassWithReadWriteProperties.ReadOnlyProperty))!;

				async Task Act()
				{
					await That(subject).IsReadable();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsWriteOnly_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithReadWriteProperties)
					.GetProperty(nameof(TestClassWithReadWriteProperties.WriteOnlyProperty))!;

				async Task Act()
				{
					await That(subject).IsReadable();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is readable,
					              but it was not readable public string TestClassWithReadWriteProperties.WriteOnlyProperty { set; }
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertyIsReadable_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithReadWriteProperties)
					.GetProperty(nameof(TestClassWithReadWriteProperties.ReadOnlyProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsReadable());
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not readable,
					              but it was readable public string TestClassWithReadWriteProperties.ReadOnlyProperty { get; }
					              """);
			}

			[Test]
			public async Task WhenPropertyIsWriteOnly_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClassWithReadWriteProperties)
					.GetProperty(nameof(TestClassWithReadWriteProperties.WriteOnlyProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsReadable());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
