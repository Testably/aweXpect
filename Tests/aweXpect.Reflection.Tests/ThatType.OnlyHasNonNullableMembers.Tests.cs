using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class OnlyHasNonNullableMembers
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeHasMixedMembers_ShouldFail()
			{
				Type subject = typeof(ClassWithMixedNullableMembers);

				async Task Act()
				{
					await That(subject).OnlyHasNonNullableMembers();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             only has non-nullable members,
					             but it contained nullable members [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenTypeHasNoMembers_ShouldSucceed()
			{
				Type subject = typeof(ClassWithoutMembers);

				async Task Act()
				{
					await That(subject).OnlyHasNonNullableMembers();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasNullableMembers_ShouldFail()
			{
				Type subject = typeof(ClassWithSingleNullableProperty);
				PropertyInfo property = subject
					.GetProperty(nameof(ClassWithSingleNullableProperty.NullableProperty))!;

				async Task Act()
				{
					await That(subject).OnlyHasNonNullableMembers();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              only has non-nullable members,
					              but it contained nullable members [
					                {Formatter.Format(property)}
					              ]
					              """);
			}

			[Fact]
			public async Task WhenTypeHasNullableEvent_ShouldFail()
			{
				Type subject = typeof(ClassWithSingleNullableEvent);
				EventInfo @event = subject
					.GetEvent(nameof(ClassWithSingleNullableEvent.NullableEvent))!;

				async Task Act()
				{
					await That(subject).OnlyHasNonNullableMembers();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              only has non-nullable members,
					              but it contained nullable members [
					                {Formatter.Format(@event)}
					              ]
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).OnlyHasNonNullableMembers();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             only has non-nullable members,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeOnlyHasNonNullableEvent_ShouldSucceed()
			{
				Type subject = typeof(ClassWithSingleNonNullableEvent);

				async Task Act()
				{
					await That(subject).OnlyHasNonNullableMembers();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeOnlyHasNonNullableMembers_ShouldSucceed()
			{
				Type subject = typeof(ClassWithNonNullableMembers);

				async Task Act()
				{
					await That(subject).OnlyHasNonNullableMembers();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeHasNullableMembers_ShouldSucceed()
			{
				Type subject = typeof(ClassWithMixedNullableMembers);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.OnlyHasNonNullableMembers());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeOnlyHasNonNullableMembers_ShouldFail()
			{
				Type subject = typeof(ClassWithSingleNonNullableProperty);
				PropertyInfo property = subject
					.GetProperty(nameof(ClassWithSingleNonNullableProperty.NonNullableProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.OnlyHasNonNullableMembers());
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              does not only have non-nullable members,
					              but it only contained non-nullable members [
					                {Formatter.Format(property)}
					              ]
					              """);
			}
		}
	}
}
