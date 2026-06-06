using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class OnlyHasNullableMembers
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenBaseTypeHasNonNullableEvent_ShouldSucceed()
			{
				Type subject = typeof(DerivedClassWithNullableEvent);

				async Task Act()
				{
					await That(subject).OnlyHasNullableMembers();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenBaseTypeHasNonNullableMembers_ShouldSucceed()
			{
				Type subject = typeof(DerivedClassWithNullableMembers);

				async Task Act()
				{
					await That(subject).OnlyHasNullableMembers();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasMixedMembers_ShouldFail()
			{
				Type subject = typeof(ClassWithMixedNullableMembers);

				async Task Act()
				{
					await That(subject).OnlyHasNullableMembers();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             only has nullable members,
					             but it contained non-nullable members [
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
					await That(subject).OnlyHasNullableMembers();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasNonNullableMembers_ShouldFail()
			{
				Type subject = typeof(ClassWithSingleNonNullableProperty);
				PropertyInfo property = subject
					.GetProperty(nameof(ClassWithSingleNonNullableProperty.NonNullableProperty))!;

				async Task Act()
				{
					await That(subject).OnlyHasNullableMembers();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              only has nullable members,
					              but it contained non-nullable members [
					                {Formatter.Format(property)}
					              ]
					              """);
			}

			[Fact]
			public async Task WhenTypeHasNonNullableEvent_ShouldFail()
			{
				Type subject = typeof(ClassWithSingleNonNullableEvent);
				EventInfo @event = subject
					.GetEvent(nameof(ClassWithSingleNonNullableEvent.NonNullableEvent))!;

				async Task Act()
				{
					await That(subject).OnlyHasNullableMembers();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              only has nullable members,
					              but it contained non-nullable members [
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
					await That(subject).OnlyHasNullableMembers();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             only has nullable members,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeOnlyHasNullableEvent_ShouldSucceed()
			{
				Type subject = typeof(ClassWithSingleNullableEvent);

				async Task Act()
				{
					await That(subject).OnlyHasNullableMembers();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeOnlyHasNullableMembers_ShouldSucceed()
			{
				Type subject = typeof(ClassWithNullableMembers);

				async Task Act()
				{
					await That(subject).OnlyHasNullableMembers();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithIncludingInherited_WhenBaseTypeHasNonNullableEvent_ShouldFail()
			{
				Type subject = typeof(DerivedClassWithNullableEvent);
				EventInfo @event = subject
					.GetEvent(nameof(ClassWithSingleNonNullableEvent.NonNullableEvent))!;

				async Task Act()
				{
					await That(subject).OnlyHasNullableMembers(MemberScope.IncludingInherited);
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              only has nullable members,
					              but it contained non-nullable members [
					                {Formatter.Format(@event)}
					              ]
					              """);
			}

			[Fact]
			public async Task WithIncludingInherited_WhenBaseTypeHasNonNullableMembers_ShouldFail()
			{
				Type subject = typeof(DerivedClassWithNullableMembers);

				async Task Act()
				{
					await That(subject).OnlyHasNullableMembers(MemberScope.IncludingInherited);
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             only has nullable members,
					             but it contained non-nullable members [
					               *
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WithIncludingInherited_WhenBaseTypeHasNullableEvent_ShouldSucceed()
			{
				Type subject = typeof(DerivedClassWithInheritedNullableEvent);

				async Task Act()
				{
					await That(subject).OnlyHasNullableMembers(MemberScope.IncludingInherited);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithIncludingInherited_WhenBaseTypeHasPrivateNonNullableEvent_ShouldFail()
			{
				Type subject = typeof(DerivedClassWithPrivateNonNullableBaseEvent);
				EventInfo @event = typeof(BaseClassWithPrivateNonNullableEvent)
					.GetEvent("PrivateNonNullableEvent", BindingFlags.NonPublic | BindingFlags.Instance)!;

				async Task Act()
				{
					await That(subject).OnlyHasNullableMembers(MemberScope.IncludingInherited);
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              only has nullable members,
					              but it contained non-nullable members [
					                {Formatter.Format(@event)}
					              ]
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeHasNonNullableMembers_ShouldSucceed()
			{
				Type subject = typeof(ClassWithMixedNullableMembers);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.OnlyHasNullableMembers());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeOnlyHasNullableMembers_ShouldFail()
			{
				Type subject = typeof(ClassWithSingleNullableProperty);
				PropertyInfo property = subject
					.GetProperty(nameof(ClassWithSingleNullableProperty.NullableProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.OnlyHasNullableMembers());
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              does not only have nullable members,
					              but it only contained nullable members [
					                {Formatter.Format(property)}
					              ]
					              """);
			}
		}
	}
}
