using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsWritable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsWritable();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is writable,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenPropertyIsReadOnly_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithReadWriteProperties)
					.GetProperty(nameof(TestClassWithReadWriteProperties.ReadOnlyProperty))!;

				async Task Act()
				{
					await That(subject).IsWritable();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is writable,
					              but it was not writable {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenPropertyIsWritable_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClassWithReadWriteProperties)
					.GetProperty(nameof(TestClassWithReadWriteProperties.WriteOnlyProperty))!;

				async Task Act()
				{
					await That(subject).IsWritable();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyIsReadOnly_ShouldSucceed()
			{
				PropertyInfo subject = typeof(TestClassWithReadWriteProperties)
					.GetProperty(nameof(TestClassWithReadWriteProperties.ReadOnlyProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsWritable());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsWritable_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithReadWriteProperties)
					.GetProperty(nameof(TestClassWithReadWriteProperties.WriteOnlyProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsWritable());
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not writable,
					              but it was writable {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
