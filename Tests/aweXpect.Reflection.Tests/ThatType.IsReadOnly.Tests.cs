using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsReadOnly
	{
		public sealed class Tests
		{
			[Theory]
			[MemberData(nameof(NonReadOnlyTypes))]
			public async Task WhenTypeIsNotReadOnly_ShouldFail(Type subject)
			{
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

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

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
			public async Task WhenTypeIsReadOnly_ShouldSucceed()
			{
				Type subject = typeof(PublicReadOnlyStruct);

				async Task Act()
				{
					await That(subject).IsReadOnly();
				}

				await That(Act).DoesNotThrow();
			}

			public static TheoryData<Type> NonReadOnlyTypes() => new()
			{
				typeof(PublicStruct),
				typeof(PublicRefStruct),
				typeof(PublicClass),
				typeof(IPublicInterface),
				typeof(PublicEnum),
			};
		}

		public sealed class NegatedTests
		{
			[Theory]
			[MemberData(nameof(NonReadOnlyTypes))]
			public async Task WhenTypeIsNotReadOnly_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsReadOnly());
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsReadOnly_ShouldFail()
			{
				Type subject = typeof(PublicReadOnlyStruct);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsReadOnly());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not read-only,
					              but it was read-only {Formatter.Format(subject)}
					              """);
			}

			public static TheoryData<Type> NonReadOnlyTypes() => new()
			{
				typeof(PublicStruct),
				typeof(PublicRefStruct),
				typeof(PublicClass),
				typeof(IPublicInterface),
				typeof(PublicEnum),
			};
		}
	}
}
