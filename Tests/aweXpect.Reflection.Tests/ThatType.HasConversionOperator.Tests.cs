using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

#pragma warning disable CA2263 // Prefer generic overload when type is known

public sealed partial class ThatType
{
	public sealed class HasConversionOperator
	{
		public sealed class ImplicitTests
		{
			[Test]
			public async Task DoesNotHave_WhenAbsent_Generic_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveImplicitConversionOperator<Money, string>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task DoesNotHave_WhenAbsent_WithTypes_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveImplicitConversionOperator(typeof(Money), typeof(string));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task DoesNotHave_WhenOnlyInherited_Directly_ShouldSucceed()
			{
				Type subject = typeof(OperatorDerivedClass);

				async Task Act()
				{
					await That(subject).DoesNotHaveImplicitConversionOperator<OperatorBaseClass, int>().Directly();
				}

				await That(Act).DoesNotThrow()
					.Because("with Directly only the conversion operators declared on the type count");
			}

			[Test]
			public async Task DoesNotHave_WhenOnlyInherited_ShouldFail()
			{
				Type subject = typeof(OperatorDerivedClass);

				async Task Act()
				{
					await That(subject).DoesNotHaveImplicitConversionOperator<OperatorBaseClass, int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              does not have an implicit conversion operator from OperatorBaseClass to int,
					              but it had an implicit conversion operator from OperatorBaseClass to int {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task DoesNotHave_WhenPresent_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveImplicitConversionOperator<Money, decimal>();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              does not have an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(decimal))},
					              but it had an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(decimal))} {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenConversionIsOnlyInherited_Directly_ShouldFail()
			{
				Type subject = typeof(OperatorDerivedClass);

				async Task Act()
				{
					await That(subject).HasImplicitConversionOperator<OperatorBaseClass, int>().Directly();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              declares an implicit conversion operator from OperatorBaseClass to int,
					              but it did not declare an implicit conversion operator from OperatorBaseClass to int {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenConversionIsOnlyInherited_ShouldSucceed()
			{
				Type subject = typeof(OperatorDerivedClass);

				async Task Act()
				{
					await That(subject).HasImplicitConversionOperator<OperatorBaseClass, int>();
				}

				await That(Act).DoesNotThrow()
					.Because("conversion operators inherited from base types are considered by default");
			}

			[Test]
			public async Task WhenTypeDoesNotHaveTheConversion_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).HasImplicitConversionOperator<Money, string>();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              has an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(string))},
					              but it did not have an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(string))} {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenTypeHasTheConversion_Generic_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).HasImplicitConversionOperator<Money, decimal>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeHasTheConversion_WithTypes_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).HasImplicitConversionOperator(typeof(Money), typeof(decimal));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).HasImplicitConversionOperator<Money, decimal>();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              has an implicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(decimal))},
					              but it was <null>
					              """);
			}
		}

		public sealed class ExplicitTests
		{
			[Test]
			public async Task DoesNotHave_WhenAbsent_Generic_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveExplicitConversionOperator<Money, string>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task DoesNotHave_WhenAbsent_WithTypes_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveExplicitConversionOperator(typeof(Money), typeof(string));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task DoesNotHave_WhenPresent_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).DoesNotHaveExplicitConversionOperator<Money, int>();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              does not have an explicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(int))},
					              but it had an explicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(int))} {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenTypeDoesNotHaveTheConversion_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).HasExplicitConversionOperator<Money, string>();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              has an explicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(string))},
					              but it did not have an explicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(string))} {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenTypeHasTheConversion_Generic_ShouldSucceed()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject).HasExplicitConversionOperator<Money, int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
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
			[Test]
			public async Task WhenTypeHasTheExplicitConversion_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(it => it.HasExplicitConversionOperator<Money, int>());
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              does not have an explicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(int))},
					              but it had an explicit conversion operator from {Formatter.Format(typeof(Money))} to {Formatter.Format(typeof(int))} {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenTypeHasTheImplicitConversion_ShouldFail()
			{
				Type subject = typeof(Money);

				async Task Act()
				{
					await That(subject)
						.DoesNotComplyWith(it => it.HasImplicitConversionOperator<Money, decimal>());
				}

				await That(Act).Throws()
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
