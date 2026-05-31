using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsWriteOnly
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyIsWriteOnly_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClassWithReadWriteProperties)
					.GetProperty(nameof(TestClassWithReadWriteProperties.WriteOnlyProperty))!;

				async Task Act()
				{
					await That(subject).IsWriteOnly();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsReadWrite_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithReadWriteProperties)
					.GetProperty(nameof(TestClassWithReadWriteProperties.ReadWriteProperty))!;

				async Task Act()
				{
					await That(subject).IsWriteOnly();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is write-only,
					              but it was not write-only {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsWriteOnly();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is write-only,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyIsWriteOnly_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithReadWriteProperties)
					.GetProperty(nameof(TestClassWithReadWriteProperties.WriteOnlyProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsWriteOnly());
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not write-only,
					              but it was write-only {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPropertyIsReadWrite_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClassWithReadWriteProperties)
					.GetProperty(nameof(TestClassWithReadWriteProperties.ReadWriteProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsWriteOnly());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
