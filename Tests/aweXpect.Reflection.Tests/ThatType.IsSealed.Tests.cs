using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsSealed
	{
		public sealed class Tests
		{
			[Test]
			[MethodDataSource(nameof(NonSealedTypes))]
			public async Task WhenTypeIsNotSealed_ShouldFail(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsSealed();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              is sealed,
					              but it was non-sealed {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsSealed();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is sealed,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTypeIsSealed_ShouldSucceed()
			{
				Type subject = typeof(PublicSealedClass);

				async Task Act()
				{
					await That(subject).IsSealed();
				}

				await That(Act).DoesNotThrow();
			}

			public static IEnumerable<Type> NonSealedTypes()
				=>
				[
					typeof(PublicAbstractClass),
					typeof(PublicStaticClass),
					typeof(Container.PublicNestedClass),
					typeof(IPublicInterface),
				];
		}

		public sealed class NegatedTests
		{
			[Test]
			[MethodDataSource(nameof(NonSealedTypes))]
			public async Task WhenTypeIsNotSealed_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsSealed());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsSealed_ShouldFail()
			{
				Type subject = typeof(PublicSealedClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsSealed());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("*is not sealed*but it was*").AsWildcard();
			}

			public static IEnumerable<Type> NonSealedTypes()
				=>
				[
					typeof(PublicAbstractClass),
					typeof(PublicStaticClass),
					typeof(Container.PublicNestedClass),
					typeof(IPublicInterface),
				];
		}
	}
}
