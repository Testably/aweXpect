using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsReadable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsReadable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is readable,
					             but it was <null>
					             """);
			}

			[Fact]
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

			[Fact]
			public async Task WhenPropertyIsWriteOnly_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithReadWriteProperties)
					.GetProperty(nameof(TestClassWithReadWriteProperties.WriteOnlyProperty))!;

				async Task Act()
				{
					await That(subject).IsReadable();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is readable,
					              but it was not readable {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyIsReadable_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithReadWriteProperties)
					.GetProperty(nameof(TestClassWithReadWriteProperties.ReadOnlyProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsReadable());
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not readable,
					              but it was readable {Formatter.Format(subject)}
					              """);
			}

			[Fact]
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
