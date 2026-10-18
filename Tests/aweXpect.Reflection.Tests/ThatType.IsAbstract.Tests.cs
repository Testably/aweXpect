using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsAbstract
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsAbstract_ShouldSucceed()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).IsAbstract();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NonAbstractTypes))]
			public async Task WhenTypeIsNotAbstract_ShouldFail(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsAbstract();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              is abstract,
					              but it was non-abstract {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsAbstract();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is abstract,
					             but it was <null>
					             """);
			}

			public static IEnumerable<Type> NonAbstractTypes()
				=>
				[
					typeof(PublicStaticClass),
					typeof(PublicSealedClass),
					typeof(Container.PublicNestedClass),
					typeof(IPublicInterface),
				];
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeIsAbstract_ShouldFail()
			{
				Type subject = typeof(PublicAbstractClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAbstract());
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not abstract,
					              but it was abstract {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[MethodDataSource(nameof(NonAbstractTypesForNegated))]
			public async Task WhenTypeIsNotAbstract_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAbstract());
				}

				await That(Act).DoesNotThrow();
			}

			public static IEnumerable<Type> NonAbstractTypesForNegated() => new List<Type>
			{
				typeof(PublicStaticClass),
				typeof(PublicSealedClass),
				typeof(Container.PublicNestedClass),
				typeof(IPublicInterface),
			};

			public static IEnumerable<Type> NonAbstractTypes()
				=>
				[
					typeof(PublicStaticClass),
					typeof(PublicSealedClass),
					typeof(Container.PublicNestedClass),
					typeof(IPublicInterface),
				];
		}
	}
}
