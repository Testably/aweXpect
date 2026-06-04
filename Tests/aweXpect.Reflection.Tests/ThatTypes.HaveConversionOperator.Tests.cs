using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

#pragma warning disable CA2263 // Prefer generic overload when type is known

public sealed partial class ThatTypes
{
	public sealed class HaveConversionOperator
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllTypesHaveTheExplicitConversion_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				};

				async Task Act()
				{
					await That(subject).HaveExplicitConversionOperator<Money, int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllTypesHaveTheImplicitConversion_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				};

				async Task Act()
				{
					await That(subject).HaveImplicitConversionOperator<Money, decimal>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomeTypeDoesNotHaveTheExplicitConversion_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money), typeof(ClassWithoutOperators),
				};

				async Task Act()
				{
					await That(subject).HaveExplicitConversionOperator<Money, int>();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              all have an explicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(int))},
					              but it contained types without an explicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(int))} [
					                *
					              ]
					              """).AsWildcard();
			}

			[Fact]
			public async Task WhenSomeTypeDoesNotHaveTheImplicitConversion_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money), typeof(ClassWithoutOperators),
				};

				async Task Act()
				{
					await That(subject).HaveImplicitConversionOperator<Money, decimal>();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              all have an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(decimal))},
					              but it contained types without an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(decimal))} [
					                *
					              ]
					              """).AsWildcard();
			}

			[Fact]
			public async Task WithTypeOverloads_ShouldBehaveLikeGeneric()
			{
				IEnumerable<Type?> money = new[]
				{
					typeof(Money),
				};
				IEnumerable<Type?> without = new[]
				{
					typeof(ClassWithoutOperators),
				};

				await That(money).HaveImplicitConversionOperator(typeof(Money), typeof(decimal));
				await That(money).HaveExplicitConversionOperator(typeof(Money), typeof(int));
				await That(without).DoNotHaveImplicitConversionOperator(typeof(Money), typeof(decimal));
				await That(without).DoNotHaveExplicitConversionOperator(typeof(Money), typeof(int));
			}
		}

		public sealed class DoNotHaveTests
		{
			[Fact]
			public async Task WhenNoTypeHasTheExplicitConversion_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithoutOperators),
				};

				async Task Act()
				{
					await That(subject).DoNotHaveExplicitConversionOperator<Money, int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoTypeHasTheImplicitConversion_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithoutOperators),
				};

				async Task Act()
				{
					await That(subject).DoNotHaveImplicitConversionOperator<Money, decimal>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomeTypeHasTheExplicitConversion_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				};

				async Task Act()
				{
					await That(subject).DoNotHaveExplicitConversionOperator<Money, int>();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              all do not have an explicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(int))},
					              but it contained types with an explicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(int))} [
					                *
					              ]
					              """).AsWildcard();
			}

			[Fact]
			public async Task WhenSomeTypeHasTheImplicitConversion_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				};

				async Task Act()
				{
					await That(subject).DoNotHaveImplicitConversionOperator<Money, decimal>();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              all do not have an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(decimal))},
					              but it contained types with an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(decimal))} [
					                *
					              ]
					              """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllTypesHaveTheConversion_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				};

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(they => they.HaveImplicitConversionOperator<Money, decimal>());
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              not all have an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(decimal))},
					              but it only contained types with an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(decimal))} [
					                *
					              ]
					              """).AsWildcard();
			}

			[Fact]
			public async Task WhenNoTypeHasTheConversion_DoNotHave_ShouldFail()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithoutOperators),
				};

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(they => they.DoNotHaveImplicitConversionOperator<Money, decimal>());
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              also contain a type with an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(decimal))},
					              but it only contained types without an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(decimal))} [
					                *
					              ]
					              """).AsWildcard();
			}
		}

#if NET8_0_OR_GREATER
		public sealed class AsyncEnumerableTests
		{
			[Fact]
			public async Task WhenAllTypesHaveTheExplicitConversion_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).HaveExplicitConversionOperator<Money, int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllTypesHaveTheImplicitConversion_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).HaveImplicitConversionOperator<Money, decimal>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoTypeHasTheImplicitConversion_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithoutOperators),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoNotHaveImplicitConversionOperator<Money, decimal>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomeTypeDoesNotHaveTheImplicitConversion_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(Money), typeof(ClassWithoutOperators),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).HaveImplicitConversionOperator<Money, decimal>();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              all have an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(decimal))},
					              but it contained types without an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(decimal))} [
					                *
					              ]
					              """).AsWildcard();
			}

			[Fact]
			public async Task WhenSomeTypeHasTheExplicitConversion_DoNotHave_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(Money),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).DoNotHaveExplicitConversionOperator<Money, int>();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              all do not have an explicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(int))},
					              but it contained types with an explicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(int))} [
					                *
					              ]
					              """).AsWildcard();
			}

			[Fact]
			public async Task WithTypeOverloads_ShouldBehaveLikeGeneric()
			{
				IAsyncEnumerable<Type?> money = new[]
				{
					typeof(Money),
				}.ToTestAsyncEnumerable<Type?>();
				IAsyncEnumerable<Type?> without = new[]
				{
					typeof(ClassWithoutOperators),
				}.ToTestAsyncEnumerable<Type?>();

				await That(money).HaveImplicitConversionOperator(typeof(Money), typeof(decimal));
				await That(money).HaveExplicitConversionOperator(typeof(Money), typeof(int));
				await That(without).DoNotHaveImplicitConversionOperator(typeof(Money), typeof(decimal));
				await That(without).DoNotHaveExplicitConversionOperator(typeof(Money), typeof(int));
			}
		}
#endif
	}
}

#pragma warning restore CA2263 // Prefer generic overload when type is known
