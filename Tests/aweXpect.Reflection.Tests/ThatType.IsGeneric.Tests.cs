using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed partial class IsGeneric
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenArgumentFilterThrows_ShouldFail()
			{
				Type subject = typeof(PublicGenericClass<int>);
				Func<Type, string?, bool> predicate = (_, _) => throw new InvalidOperationException("boom");

				async Task Act()
				{
					GenericArgumentCollectionResult<Type, Type?> result = That(subject).IsGeneric();
					((IOptionsProvider<GenericArgumentsFilterOptions>)result).Options.Value.AddFilter(
						new GenericArgumentFilterOptions(predicate, () => "matching predicate"), new CollectionIndexOptions());
					await result;
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is generic with argument matching predicate,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}

			[Test]
			public async Task WhenArgumentsPredicateThrows_ShouldFail()
			{
				Type subject = typeof(PublicGenericClass<int>);

				async Task Act()
				{
					GenericArgumentCollectionResult<Type, Type?> result = That(subject).IsGeneric();
					((IOptionsProvider<GenericArgumentsFilterOptions>)result).Options.Value.AddPredicate(
						_ => throw new InvalidOperationException("boom"), () => "with arguments matching predicate");
					await result;
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is generic with arguments matching predicate,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}

			[Test]
			public async Task WhenTypeIsGeneric_ShouldSucceed()
			{
				Type subject = typeof(PublicGenericClass<int>);

				async Task Act()
				{
					await That(subject).IsGeneric();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNotGeneric_ShouldFail()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
				{
					await That(subject).IsGeneric();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is generic,
					             but it was non-generic PublicClass
					             """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsGeneric();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is generic,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenArgumentsPredicateThrows_ShouldFail()
			{
				Type subject = typeof(PublicGenericClass<int>);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> ((IOptionsProvider<GenericArgumentsFilterOptions>)it.IsGeneric()).Options.Value.AddPredicate(
							_ => throw new InvalidOperationException("boom"), () => "with arguments matching predicate"));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not generic,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("a predicate that throws answers neither the expectation nor its negation");
			}

			[Test]
			public async Task WhenTypeIsGeneric_ShouldFail()
			{
				Type subject = typeof(PublicGenericClass<int>);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsGeneric());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not generic,
					             but it was generic PublicGenericClass<int>
					             """);
			}

			[Test]
			public async Task WhenTypeIsNotGeneric_ShouldSucceed()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsGeneric());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
