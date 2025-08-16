using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class ReturnsExactly
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task WhenMethodDoesNotReturnExpectedType_ShouldFail()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
					=> await That(subject).ReturnsExactly<string>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             returns exactly string,
					             but it returned int
					             """);
			}

			[Fact]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
					=> await That(subject).ReturnsExactly<int>();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             returns exactly int,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenMethodReturnsExpectedType_ShouldSucceed()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
					=> await That(subject).ReturnsExactly<int>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenReturnTypeInheritsFromExpectedType_ShouldFail()
			{
				MethodInfo subject = typeof(ClassWithInheritance).GetMethod(nameof(ClassWithInheritance.GetDerived))!;

				async Task Act()
					=> await That(subject).ReturnsExactly<BaseClass>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             returns exactly ThatMethod.BaseClass,
					             but it returned ThatMethod.DerivedClass
					             """)
					.AsWildcard();
			}
		}

		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenMethodDoesNotReturnExpectedType_ShouldFail()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
					=> await That(subject).ReturnsExactly(typeof(string));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             returns exactly string,
					             but it returned int
					             """);
			}

			[Fact]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
					=> await That(subject).ReturnsExactly(typeof(int));

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             returns exactly int,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenMethodReturnsExpectedType_ShouldSucceed()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
					=> await That(subject).ReturnsExactly(typeof(int));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenReturnTypeInheritsFromExpectedType_ShouldFail()
			{
				MethodInfo subject = typeof(ClassWithInheritance).GetMethod(nameof(ClassWithInheritance.GetDerived))!;

				async Task Act()
					=> await That(subject).ReturnsExactly(typeof(BaseClass));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             returns exactly ThatMethod.BaseClass,
					             but it returned ThatMethod.DerivedClass
					             """)
					.AsWildcard();
			}
		}

		public sealed class OrReturnsExactlyTests
		{
			[Fact]
			public async Task WithMultipleOrReturnsExactly_ShouldSupportChaining()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
					=> await That(subject).ReturnsExactly<string>().OrReturnsExactly(typeof(bool))
						.OrReturnsExactly<int>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithOrReturnsExactly_WhenMethodReturnsNoneOfTheTypes_ShouldFail()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
					=> await That(subject).ReturnsExactly<string>().OrReturnsExactly<bool>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             returns exactly string or exactly bool,
					             but it returned int
					             """);
			}

			[Fact]
			public async Task WithOrReturnsExactly_WhenMethodReturnsOneOfTheTypes_ShouldSucceed()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
					=> await That(subject).ReturnsExactly<string>().OrReturnsExactly<int>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithOrReturnsExactly_WhenReturnTypeInheritsFromExpectedType_ShouldFail()
			{
				MethodInfo subject = typeof(ClassWithInheritance).GetMethod(nameof(ClassWithInheritance.GetDerived))!;

				async Task Act()
					=> await That(subject).ReturnsExactly<BaseClass>().OrReturnsExactly<string>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             returns exactly ThatMethod.BaseClass or exactly string,
					             but it returned ThatMethod.DerivedClass
					             """)
					.AsWildcard();
			}
		}

		public sealed class OrReturnsTests
		{
			[Fact]
			public async Task WithMultipleOrReturnsExactly_ShouldSupportChaining()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
					=> await That(subject).ReturnsExactly<string>().OrReturns(typeof(bool))
						.OrReturnsExactly<int>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithOrReturnsExactly_WhenMethodReturnsNoneOfTheTypes_ShouldFail()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
					=> await That(subject).ReturnsExactly<string>().OrReturns<bool>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             returns exactly string or bool,
					             but it returned int
					             """);
			}

			[Fact]
			public async Task WithOrReturnsExactly_WhenMethodReturnsOneOfTheTypes_ShouldSucceed()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
					=> await That(subject).ReturnsExactly<string>().OrReturns<int>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithOrReturnsExactly_WhenReturnTypeInheritsFromExpectedType_ShouldFail()
			{
				MethodInfo subject = typeof(ClassWithInheritance).GetMethod(nameof(ClassWithInheritance.GetDerived))!;

				async Task Act()
					=> await That(subject).ReturnsExactly<BaseClass>().OrReturns<string>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             returns exactly ThatMethod.BaseClass or string,
					             but it returned ThatMethod.DerivedClass
					             """)
					.AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			public sealed class GenericTests
			{
				[Fact]
				public async Task WhenMethodDoesNotReturnExpectedType_ShouldSucceed()
				{
					MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.ReturnsExactly<string>());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenMethodReturnsExpectedType_ShouldFail()
				{
					MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.ReturnsExactly<int>());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not return exactly int,
						             but it did
						             """);
				}

				[Fact]
				public async Task WhenReturnTypeInheritsFromExpectedType_ShouldSucceed()
				{
					MethodInfo subject =
						typeof(ClassWithInheritance).GetMethod(nameof(ClassWithInheritance.GetDerived))!;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.ReturnsExactly<BaseClass>());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class OrReturnsTests
			{
				[Fact]
				public async Task WithMultipleOrReturnsExactly_ShouldSupportChaining()
				{
					MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it
							=> it.ReturnsExactly<string>().OrReturnsExactly(typeof(bool)).OrReturns<Task>());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithOrReturnsExactly_WhenMethodReturnsNoneOfTheTypes_ShouldSucceed()
				{
					MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

					async Task Act()
						=> await That(subject)
							.DoesNotComplyWith(it => it.ReturnsExactly<string>().OrReturns<bool>());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithOrReturnsExactly_WhenMethodReturnsOneOfTheTypes_ShouldFail()
				{
					MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

					async Task Act()
						=> await That(subject)
							.DoesNotComplyWith(it => it.ReturnsExactly<string>().OrReturns<int>());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not return exactly string or int,
						             but it did
						             """);
				}
			}
		}
	}
}
