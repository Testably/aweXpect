using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class Returns
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task WhenMethodDoesNotReturnExpectedType_ShouldFail()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
					=> await That(subject).Returns<string>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             returns string,
					             but it returned int
					             """);
			}

			[Fact]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
					=> await That(subject).Returns<int>();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             returns int,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenMethodReturnsExpectedType_ShouldSucceed()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
					=> await That(subject).Returns<int>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenReturnTypeInheritsFromExpectedType_ShouldSucceed()
			{
				MethodInfo subject = typeof(ClassWithInheritance).GetMethod(nameof(ClassWithInheritance.GetDerived))!;

				async Task Act()
					=> await That(subject).Returns<BaseClass>();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenMethodDoesNotReturnExpectedType_ShouldFail()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
					=> await That(subject).Returns(typeof(string));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             returns string,
					             but it returned int
					             """);
			}

			[Fact]
			public async Task WhenMethodInfoIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
					=> await That(subject).Returns(typeof(int));

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             returns int,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenMethodReturnsExpectedType_ShouldSucceed()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
					=> await That(subject).Returns(typeof(int));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenReturnTypeInheritsFromExpectedType_ShouldSucceed()
			{
				MethodInfo subject = typeof(ClassWithInheritance).GetMethod(nameof(ClassWithInheritance.GetDerived))!;

				async Task Act()
					=> await That(subject).Returns(typeof(BaseClass));

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OrReturnsTests
		{
			[Fact]
			public async Task WithMultipleOrReturns_ShouldSupportChaining()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
					=> await That(subject).Returns<string>().OrReturns(typeof(bool)).OrReturns<int>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithOrReturns_WhenMethodReturnsNoneOfTheTypes_ShouldFail()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
					=> await That(subject).Returns<string>().OrReturns<bool>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             returns string or bool,
					             but it returned int
					             """);
			}

			[Fact]
			public async Task WithOrReturns_WhenMethodReturnsOneOfTheTypes_ShouldSucceed()
			{
				MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

				async Task Act()
					=> await That(subject).Returns(typeof(string)).OrReturns<int>();

				await That(Act).DoesNotThrow();
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
						=> await That(subject).DoesNotComplyWith(it => it.Returns<string>());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenMethodInfoIsNull_ShouldFail()
				{
					MethodInfo? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.Returns<int>());

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that subject
						             does not return int,
						             but it was <null>
						             """)
						.AsWildcard();
				}

				[Fact]
				public async Task WhenMethodReturnsExpectedType_ShouldFail()
				{
					MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.Returns<int>());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not return int,
						             but it did
						             """)
						.AsWildcard();
				}

				[Fact]
				public async Task WhenReturnTypeInheritsFromExpectedType_ShouldFail()
				{
					MethodInfo subject = typeof(ClassWithInheritance).GetMethod(nameof(ClassWithInheritance.GetDerived))!;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.Returns<BaseClass>());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not return ThatMethod.Returns.BaseClass,
						             but it did
						             """)
						.AsWildcard();
				}
			}

			public sealed class TypeTests
			{
				[Fact]
				public async Task WhenMethodDoesNotReturnExpectedType_ShouldSucceed()
				{
					MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.Returns(typeof(string)));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenMethodInfoIsNull_ShouldFail()
				{
					MethodInfo? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.Returns(typeof(int)));

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that subject
						             does not return int,
						             but it was <null>
						             """)
						.AsWildcard();
				}

				[Fact]
				public async Task WhenMethodReturnsExpectedType_ShouldFail()
				{
					MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.Returns(typeof(int)));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not return int,
						             but it did
						             """)
						.AsWildcard();
				}

				[Fact]
				public async Task WhenReturnTypeInheritsFromExpectedType_ShouldFail()
				{
					MethodInfo subject = typeof(ClassWithInheritance).GetMethod(nameof(ClassWithInheritance.GetDerived))!;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.Returns(typeof(BaseClass)));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not return ThatMethod.Returns.BaseClass,
						             but it did
						             """)
						.AsWildcard();
				}
			}

			public sealed class OrReturnsTests
			{
				[Fact]
				public async Task WithOrReturns_WhenMethodReturnsNoneOfTheTypes_ShouldSucceed()
				{
					MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.Returns<string>().OrReturns<bool>());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithOrReturns_WhenMethodReturnsOneOfTheTypes_ShouldFail()
				{
					MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.Returns(typeof(string)).OrReturns<int>());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not return string or int,
						             but it did
						             """)
						.AsWildcard();
				}

				[Fact]
				public async Task WithMultipleOrReturns_ShouldSupportChaining()
				{
					MethodInfo subject = GetMethod(nameof(ClassWithMethods.PublicMethod))!;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.Returns<string>().OrReturns(typeof(bool)).OrReturns<Task>());

					await That(Act).DoesNotThrow();
				}
			}
		}

#pragma warning disable CA1822 // Mark members as static
		public class ClassWithInheritance
		{
			public DerivedClass GetDerived() => new();
		}

		public abstract class BaseClass;

		public class DerivedClass : BaseClass;
#pragma warning restore CA1822 // Mark members as static
	}
}
