using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsObsolete
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsObsolete_ShouldSucceed()
			{
#pragma warning disable CS0612, CS0618
				Type subject = typeof(ObsoleteClass);
#pragma warning restore CS0612, CS0618

				async Task Act()
				{
					await That(subject).IsObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNotObsolete_ShouldFail()
			{
				Type subject = typeof(ClassWithObsoleteMembers);

				async Task Act()
				{
					await That(subject).IsObsolete();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is obsolete,
					              but it was non-obsolete {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsObsolete();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is obsolete,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeIsObsolete_ShouldFail()
			{
#pragma warning disable CS0612, CS0618
				Type subject = typeof(ObsoleteClass);
#pragma warning restore CS0612, CS0618

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsObsolete());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not obsolete,
					              but it was obsolete {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNotObsolete_ShouldSucceed()
			{
				Type subject = typeof(ClassWithObsoleteMembers);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsObsolete());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
