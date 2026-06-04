using System.Collections.Generic;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsAssignableTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenClosedGenericIsCovariantlyAssignable_ShouldSucceed()
			{
				Type subject = typeof(List<string>);

				async Task Act()
				{
					await That(subject).IsAssignableTo<IEnumerable<object>>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeDerivesFromTarget_ShouldSucceed()
			{
				Type subject = typeof(DerivedClass);

				async Task Act()
				{
					await That(subject).IsAssignableTo<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeImplementsTargetInterface_ShouldSucceed()
			{
				Type subject = typeof(ClassWithInterface);

				async Task Act()
				{
					await That(subject).IsAssignableTo<ITestInterface>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsAnOpenGeneric_ShouldThrowArgumentException()
			{
				Type subject = typeof(ClassWithInterface);
				Type openGeneric = typeof(IEnumerable<>);

				async Task Act()
				{
					await That(subject).IsAssignableTo(openGeneric);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check assignability against must not be an open generic type definition, but it was IEnumerable<>. Use 'Implements' or 'InheritsFrom' for open generic type definitions.");
			}

			[Fact]
			public async Task WhenTypeIsNotAssignable_ShouldFail()
			{
				Type subject = typeof(UnrelatedClass);

				async Task Act()
				{
					await That(subject).IsAssignableTo<BaseClass>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is assignable to ThatType.BaseClass,
					             but it was not assignable to ThatType.BaseClass

					             Actual:
					             ThatType.UnrelatedClass
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsAssignableTo<BaseClass>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is assignable to ThatType.BaseClass,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsTheSame_ShouldSucceed()
			{
				Type subject = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).IsAssignableTo<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeIsAssignable_ShouldFail()
			{
				Type subject = typeof(DerivedClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAssignableTo<BaseClass>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not assignable to ThatType.BaseClass,
					             but it was assignable to ThatType.BaseClass

					             Actual:
					             ThatType.DerivedClass
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNotAssignable_ShouldSucceed()
			{
				Type subject = typeof(UnrelatedClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAssignableTo<BaseClass>());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}

	public sealed class IsNotAssignableTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsAnOpenGeneric_ShouldThrowArgumentException()
			{
				Type subject = typeof(UnrelatedClass);
				Type openGeneric = typeof(IEnumerable<>);

				async Task Act()
				{
					await That(subject).IsNotAssignableTo(openGeneric);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check assignability against must not be an open generic type definition, but it was IEnumerable<>. Use 'Implements' or 'InheritsFrom' for open generic type definitions.");
			}

			[Fact]
			public async Task WhenTypeIsAssignable_ShouldFail()
			{
				Type subject = typeof(DerivedClass);

				async Task Act()
				{
					await That(subject).IsNotAssignableTo<BaseClass>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not assignable to ThatType.BaseClass,
					             but it was assignable to ThatType.BaseClass

					             Actual:
					             ThatType.DerivedClass
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNotAssignable_ShouldSucceed()
			{
				Type subject = typeof(UnrelatedClass);

				async Task Act()
				{
					await That(subject).IsNotAssignableTo<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
