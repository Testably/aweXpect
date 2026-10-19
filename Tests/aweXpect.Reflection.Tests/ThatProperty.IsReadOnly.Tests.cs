using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsReadOnly
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsReadOnly();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is read-only,
					             but it was <null>
					             """);
			}

			[Test]
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

			[Test]
			public async Task WhenPropertyIsReadWrite_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithReadWriteProperties)
					.GetProperty(nameof(TestClassWithReadWriteProperties.ReadWriteProperty))!;

				async Task Act()
				{
					await That(subject).IsReadOnly();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is read-only,
					              but it was not read-only public string TestClassWithReadWriteProperties.ReadWriteProperty { get; set; }
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertyIsReadOnly_ShouldFail()
			{
				PropertyInfo subject = typeof(TestClassWithReadWriteProperties)
					.GetProperty(nameof(TestClassWithReadWriteProperties.ReadOnlyProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsReadOnly());
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not read-only,
					              but it was read-only public string TestClassWithReadWriteProperties.ReadOnlyProperty { get; }
					              """);
			}

			[Test]
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
