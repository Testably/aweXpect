using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

#pragma warning disable CA2263 // Prefer generic overload when type is known

public sealed partial class ThatType
{
	public sealed class HasConversionOperator
	{
		public sealed class ImplicitTests
		{
			[Fact]
			public async Task DoesNotHave_WhenAbsent_Generic_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveImplicitConversionOperator<Money, string>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotHave_WhenAbsent_WithTypes_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveImplicitConversionOperator(typeof(Money), typeof(string));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotHave_WhenPresent_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveImplicitConversionOperator<Money, decimal>();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              does not have an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(decimal))},
					              but it had an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(decimal))} {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenTypeDoesNotHaveTheConversion_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).HasImplicitConversionOperator<Money, string>();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              has an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(string))},
					              but it did not have an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(string))} {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenTypeHasTheConversion_Generic_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).HasImplicitConversionOperator<Money, decimal>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasTheConversion_WithTypes_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).HasImplicitConversionOperator(typeof(Money), typeof(decimal));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).HasImplicitConversionOperator<Money, decimal>();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              has an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(decimal))},
					              but it was <null>
					              """);
			}
		}

		public sealed class ExplicitTests
		{
			[Fact]
			public async Task DoesNotHave_WhenAbsent_Generic_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveExplicitConversionOperator<Money, string>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotHave_WhenAbsent_WithTypes_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveExplicitConversionOperator(typeof(Money), typeof(string));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotHave_WhenPresent_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveExplicitConversionOperator<Money, int>();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              does not have an explicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(int))},
					              but it had an explicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(int))} {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenTypeDoesNotHaveTheConversion_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).HasExplicitConversionOperator<Money, string>();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              has an explicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(string))},
					              but it did not have an explicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(string))} {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenTypeHasTheConversion_Generic_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).HasExplicitConversionOperator<Money, int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeHasTheConversion_WithTypes_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).HasExplicitConversionOperator(typeof(Money), typeof(int));
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeHasTheExplicitConversion_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(it => it.HasExplicitConversionOperator<Money, int>());
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              does not have an explicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(int))},
					              but it had an explicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(int))} {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenTypeHasTheImplicitConversion_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(it => it.HasImplicitConversionOperator<Money, decimal>());
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              does not have an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(decimal))},
					              but it had an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(decimal))} {Formatter.Format(subject)}
					              """);
			}
		}
	}
}

#pragma warning restore CA2263 // Prefer generic overload when type is known
