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

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             only has non-nullable members,
					             but it contained nullable members

					             Nullable members:
					             [
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

				async Task Act()
				{
					await That(subject).OnlyHasNonNullableMembers();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              only has non-nullable members,
					              but it contained nullable members

					              Nullable members:
					              [
					                public string ClassWithSingleNullableProperty.NullableProperty { get; set; }
					              ]
					              """);
			}

			[Fact]
			public async Task WhenTypeHasNullableEvent_ShouldFail()
			{
				Type subject = typeof(ClassWithSingleNullableEvent);

				async Task Act()
				{
					await That(subject).OnlyHasNonNullableMembers();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              only has non-nullable members,
					              but it contained nullable members

					              Nullable members:
					              [
					                event EventHandler ClassWithSingleNullableEvent.NullableEvent
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

				await That(Act).Throws()
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

			[Fact]
			public async Task WhenUsedInThatAll_ShouldListTheNullableMembersOfEachFailure()
			{
				async Task Act()
				{
					await ThatAll(
						That(typeof(ClassWithSingleNullableProperty)).OnlyHasNonNullableMembers(),
						That(typeof(ClassWithSingleNullableEvent)).OnlyHasNonNullableMembers());
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected all of the following to succeed:
					               [01] Expected that typeof(ClassWithSingleNullableProperty) only has non-nullable members
					               [02] Expected that typeof(ClassWithSingleNullableEvent) only has non-nullable members
					              but
					               [01] it contained nullable members
					               [02] it contained nullable members

					              [01] Nullable members:
					              [
					                public string ClassWithSingleNullableProperty.NullableProperty { get; set; }
					              ]

					              [02] Nullable members:
					              [
					                event EventHandler ClassWithSingleNullableEvent.NullableEvent
					              ]
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeHasNoMembers_ShouldFail()
			{
				Type subject = typeof(ClassWithoutMembers);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.OnlyHasNonNullableMembers());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             does not only have non-nullable members,
					             but it only contained non-nullable members
					             """);
			}

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

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.OnlyHasNonNullableMembers());
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              does not only have non-nullable members,
					              but it only contained non-nullable members

					              Non-nullable members:
					              [
					                public string ClassWithSingleNonNullableProperty.NonNullableProperty { get; set; }
					              ]
					              """);
			}
		}
	}
}
