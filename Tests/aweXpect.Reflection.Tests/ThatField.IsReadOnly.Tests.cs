using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsReadOnly
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFieldIsNotReadOnly_ShouldFail()
			{
				FieldInfo subject =
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!;

				async Task Act()
				{
					await That(subject).IsReadOnly();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is read-only,
					              but it was non-read-only {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenFieldIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

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
			public async Task WhenFieldIsReadOnly_ShouldSucceed()
			{
				FieldInfo subject =
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ReadOnlyField))!;

				async Task Act()
				{
					await That(subject).IsReadOnly();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFieldIsNotReadOnly_ShouldSucceed()
			{
				FieldInfo subject =
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsReadOnly());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldIsReadOnly_ShouldFail()
			{
				FieldInfo subject =
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ReadOnlyField))!;

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
		}
	}
}
