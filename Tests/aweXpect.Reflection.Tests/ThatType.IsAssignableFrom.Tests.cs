using System.Collections.Generic;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsAssignableFrom
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTargetDerivesFromType_ShouldSucceed()
			{
				Type subject = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).IsAssignableFrom<DerivedClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTargetImplementsTypeInterface_ShouldSucceed()
			{
				Type subject = typeof(ITestInterface);

				async Task Act()
				{
					await That(subject).IsAssignableFrom<ClassWithInterface>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsAnOpenGeneric_ShouldThrowArgumentException()
			{
				Type subject = typeof(BaseClass);
				Type openGeneric = typeof(IEnumerable<>);

				async Task Act()
				{
					await That(subject).IsAssignableFrom(openGeneric);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check assignability against must not be an open generic type definition, but it was IEnumerable<>. Use 'Implements' or 'InheritsFrom' for open generic type definitions.");
			}

			[Fact]
			public async Task WhenTypeIsNotAssignableFromTarget_ShouldFail()
			{
				Type subject = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).IsAssignableFrom<UnrelatedClass>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is assignable from ThatType.UnrelatedClass,
					             but it was not assignable from ThatType.UnrelatedClass

					             Actual:
					             ThatType.BaseClass
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsAssignableFrom<BaseClass>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is assignable from ThatType.BaseClass,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsTheSame_ShouldSucceed()
			{
				Type subject = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).IsAssignableFrom<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeIsAssignableFromTarget_ShouldFail()
			{
				Type subject = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAssignableFrom<DerivedClass>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not assignable from ThatType.DerivedClass,
					             but it was assignable from ThatType.DerivedClass

					             Actual:
					             ThatType.BaseClass
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNotAssignableFromTarget_ShouldSucceed()
			{
				Type subject = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAssignableFrom<UnrelatedClass>());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}

	public sealed class IsNotAssignableFrom
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsAnOpenGeneric_ShouldThrowArgumentException()
			{
				Type subject = typeof(BaseClass);
				Type openGeneric = typeof(IEnumerable<>);

				async Task Act()
				{
					await That(subject).IsNotAssignableFrom(openGeneric);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check assignability against must not be an open generic type definition, but it was IEnumerable<>. Use 'Implements' or 'InheritsFrom' for open generic type definitions.");
			}

			[Fact]
			public async Task WhenTypeIsAssignableFromTarget_ShouldFail()
			{
				Type subject = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).IsNotAssignableFrom<DerivedClass>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not assignable from ThatType.DerivedClass,
					             but it was assignable from ThatType.DerivedClass

					             Actual:
					             ThatType.BaseClass
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNotAssignableFromTarget_ShouldSucceed()
			{
				Type subject = typeof(BaseClass);

				async Task Act()
				{
					await That(subject).IsNotAssignableFrom<UnrelatedClass>();
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
