using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotObsolete
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsNotObsolete_ShouldSucceed()
			{
				Type subject = typeof(ClassWithObsoleteMembers);

				async Task Act()
				{
					await That(subject).IsNotObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotObsolete();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not obsolete,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeIsObsolete_ShouldFail()
			{
#pragma warning disable CS0612, CS0618
				Type subject = typeof(ObsoleteClass);
#pragma warning restore CS0612, CS0618

				async Task Act()
				{
					await That(subject).IsNotObsolete();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              is not obsolete,
					              but it was obsolete {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeIsNotObsolete_ShouldFail()
			{
				Type subject = typeof(ClassWithObsoleteMembers);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotObsolete());
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is obsolete,
					              but it was non-obsolete {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenTypeIsObsolete_ShouldSucceed()
			{
#pragma warning disable CS0612, CS0618
				Type subject = typeof(ObsoleteClass);
#pragma warning restore CS0612, CS0618

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotObsolete());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
