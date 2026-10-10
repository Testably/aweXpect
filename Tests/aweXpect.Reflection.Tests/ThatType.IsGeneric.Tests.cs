using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed partial class IsGeneric
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenArgumentFilterThrows_ShouldFail()
			{
				Type subject = typeof(PublicGenericClass<int>);
				Func<Type, string?, bool> predicate = (_, _) => throw new InvalidOperationException("boom");

				async Task Act()
				{
					GenericArgumentCollectionResult<Type, Type?> result = That(subject).IsGeneric();
					((IOptionsProvider<GenericArgumentsFilterOptions>)result).Options.AddFilter(
						new GenericArgumentFilterOptions(predicate, () => "matching predicate"), new CollectionIndexOptions());
					await result;
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is generic with argument matching predicate,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}

			[Fact]
			public async Task WhenArgumentsPredicateThrows_ShouldFail()
			{
				Type subject = typeof(PublicGenericClass<int>);

				async Task Act()
				{
					GenericArgumentCollectionResult<Type, Type?> result = That(subject).IsGeneric();
					((IOptionsProvider<GenericArgumentsFilterOptions>)result).Options.AddPredicate(
						_ => throw new InvalidOperationException("boom"), () => "with arguments matching predicate");
					await result;
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is generic with arguments matching predicate,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}

			[Fact]
			public async Task WhenTypeIsGeneric_ShouldSucceed()
			{
				Type subject = typeof(PublicGenericClass<int>);

				async Task Act()
				{
					await That(subject).IsGeneric();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
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

			[Fact]
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
			[Fact]
			public async Task WhenArgumentsPredicateThrows_ShouldFail()
			{
				Type subject = typeof(PublicGenericClass<int>);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it
						=> ((IOptionsProvider<GenericArgumentsFilterOptions>)it.IsGeneric()).Options.AddPredicate(
							_ => throw new InvalidOperationException("boom"), () => "with arguments matching predicate"));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not generic,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("a predicate that throws answers neither the expectation nor its negation");
			}

			[Fact]
			public async Task WhenTypeIsGeneric_ShouldFail()
			{
				Type subject = typeof(PublicGenericClass<int>);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsGeneric());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not generic,
					             but it was generic PublicGenericClass<int>
					             """);
			}

			[Fact]
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
