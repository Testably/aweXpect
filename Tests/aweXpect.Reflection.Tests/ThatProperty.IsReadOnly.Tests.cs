using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsReadOnly
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsReadOnly();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is read-only,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenPropertyIsReadOnly_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClassWithReadWriteProperties)
					.GetProperty(nameof(TestClassWithReadWriteProperties.ReadOnlyProperty))!;

				async Task Act()
				{
					await That(subject).IsReadOnly();
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
					await That(subject).IsReadOnly();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is read-only,
					              but it was not read-only {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyIsReadOnly_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithReadWriteProperties)
					.GetProperty(nameof(TestClassWithReadWriteProperties.ReadOnlyProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsReadOnly());
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not read-only,
					              but it was read-only {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPropertyIsReadWrite_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClassWithReadWriteProperties)
					.GetProperty(nameof(TestClassWithReadWriteProperties.ReadWriteProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsReadOnly());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
