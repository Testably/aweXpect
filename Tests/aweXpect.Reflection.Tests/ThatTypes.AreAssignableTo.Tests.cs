using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed class AreAssignableTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllTypesAreAssignable_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(DerivedClass1), typeof(DerivedClass2));

				async Task Act()
				{
					await That(subject).AreAssignableTo<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableTypesAreAssignable_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClass1), typeof(DerivedClass2),
				};

				async Task Act()
				{
					await That(subject).AreAssignableTo<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomeTypeIsNotAssignable_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(DerivedClass1), typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).AreAssignableTo<BaseClass>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.DerivedClass1, ThatTypes.UnrelatedClass]
					             are all assignable to ThatTypes.BaseClass,
					             but it contained not matching types [
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypeIsAnOpenGeneric_ShouldThrowArgumentException()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(DerivedClass1),
				};
				Type openGeneric = typeof(IEnumerable<>);

				async Task Act()
				{
					await That(subject).AreAssignableTo(openGeneric);
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage(
						"The type to check assignability against must not be an open generic type definition, but it was IEnumerable<>. Use 'Implements' or 'InheritsFrom' for open generic type definitions.");
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllTypesAreAssignable_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(DerivedClass1), typeof(DerivedClass2));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreAssignableTo<BaseClass>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.DerivedClass1, ThatTypes.DerivedClass2]
					             are not all assignable to ThatTypes.BaseClass,
					             but it only contained matching types [
					               ThatTypes.DerivedClass1,
					               ThatTypes.DerivedClass2
					             ]
					             """);
			}
		}
	}

	public sealed class AreNotAssignableTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEnumerableTypesAreNotAssignable_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(UnrelatedClass),
				};

				async Task Act()
				{
					await That(subject).AreNotAssignableTo<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoTypeIsAssignable_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).AreNotAssignableTo<BaseClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomeTypeIsAssignable_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(DerivedClass1));

				async Task Act()
				{
					await That(subject).AreNotAssignableTo<BaseClass>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.DerivedClass1]
					             are all not assignable to ThatTypes.BaseClass,
					             but it contained not matching types [
					               ThatTypes.DerivedClass1
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenNoTypeIsAssignable_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotAssignableTo<BaseClass>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.UnrelatedClass]
					             also contain a type assignable to ThatTypes.BaseClass,
					             but it only contained matching types [
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}
		}
	}

	public sealed class AreAssignableFrom
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllTypesAreAssignableFromTarget_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(BaseClass), typeof(DerivedClass1));

				async Task Act()
				{
					await That(subject).AreAssignableFrom<GrandChildClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableTypesAreAssignableFromTarget_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(BaseClass), typeof(DerivedClass1),
				};

				async Task Act()
				{
					await That(subject).AreAssignableFrom<GrandChildClass>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomeTypeIsNotAssignableFromTarget_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(BaseClass), typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).AreAssignableFrom<DerivedClass1>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.BaseClass, ThatTypes.UnrelatedClass]
					             are all assignable from ThatTypes.DerivedClass1,
					             but it contained not matching types [
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllTypesAreAssignableFromTarget_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(BaseClass), typeof(DerivedClass1));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreAssignableFrom<GrandChildClass>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.BaseClass, ThatTypes.DerivedClass1]
					             are not all assignable from ThatTypes.GrandChildClass,
					             but it only contained matching types [
					               ThatTypes.BaseClass,
					               ThatTypes.DerivedClass1
					             ]
					             """);
			}
		}
	}

	public sealed class AreNotAssignableFrom
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEnumerableTypesAreNotAssignableFromTarget_ShouldSucceed()
			{
				IEnumerable<Type?> subject = new[]
				{
					typeof(UnrelatedClass),
				};

				async Task Act()
				{
					await That(subject).AreNotAssignableFrom<DerivedClass1>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoTypeIsAssignableFromTarget_ShouldSucceed()
			{
				Filtered.Types subject = In.Types(typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).AreNotAssignableFrom<DerivedClass1>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSomeTypeIsAssignableFromTarget_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(BaseClass));

				async Task Act()
				{
					await That(subject).AreNotAssignableFrom<DerivedClass1>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.BaseClass]
					             are all not assignable from ThatTypes.DerivedClass1,
					             but it contained not matching types [
					               ThatTypes.BaseClass
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenNoTypeIsAssignableFromTarget_ShouldFail()
			{
				Filtered.Types subject = In.Types(typeof(UnrelatedClass));

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.AreNotAssignableFrom<DerivedClass1>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in types [ThatTypes.UnrelatedClass]
					             also contain a type assignable from ThatTypes.DerivedClass1,
					             but it only contained matching types [
					               ThatTypes.UnrelatedClass
					             ]
					             """);
			}
		}
	}
}
