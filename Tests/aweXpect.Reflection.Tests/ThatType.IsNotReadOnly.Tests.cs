using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotReadOnly
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsNotReadOnly_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(PublicStruct);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotReadOnly());
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              is read-only,
					              but it was not read-only {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[MethodDataSource(nameof(NonReadOnlyTypes))]
			public async Task WhenTypeIsNotReadOnly_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsNotReadOnly();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsNotReadOnly();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not read-only,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeIsReadOnly_ShouldFail()
			{
				Type subject = typeof(PublicReadOnlyStruct);

				async Task Act()
				{
					await That(subject).IsNotReadOnly();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              is not read-only,
					              but it was read-only {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenTypeIsReadOnly_ShouldSucceedWithNegatedAssertion()
			{
				Type subject = typeof(PublicReadOnlyStruct);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotReadOnly());
				}

				await That(Act).DoesNotThrow();
			}

			public static IEnumerable<Type> NonReadOnlyTypes() => new List<Type>
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
