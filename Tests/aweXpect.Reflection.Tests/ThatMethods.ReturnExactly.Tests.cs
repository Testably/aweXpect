using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class ReturnExactly
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task WhenAllMethodsReturnSpecifiedType_ShouldSucceed()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name == nameof(TestClass.GetString));

				async Task Act()
					=> await That(methods).ReturnExactly<string>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsReturnInheritedTypes_ShouldFail()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name == nameof(TestClass.GetDummy));

				async Task Act()
					=> await That(methods).ReturnExactly<DummyBase>();

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WhenSomeMethodsDoNotReturnSpecifiedType_ShouldFail()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name.StartsWith("Get"));

				async Task Act()
					=> await That(methods).ReturnExactly<string>();

				await That(Act).Throws<XunitException>();
			}
		}

		public sealed class TypeTests
		{
			[Fact]
			public async Task WhenAllMethodsReturnSpecifiedType_ShouldSucceed()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name == nameof(TestClass.GetString));

				async Task Act()
					=> await That(methods).ReturnExactly(typeof(string));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsReturnInheritedTypes_ShouldFail()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name == nameof(TestClass.GetDummy));

				async Task Act()
					=> await That(methods).ReturnExactly(typeof(DummyBase));

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WhenSomeMethodsDoNotReturnSpecifiedType_ShouldFail()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name.StartsWith("Get"));

				async Task Act()
					=> await That(methods).ReturnExactly(typeof(string));

				await That(Act).Throws<XunitException>();
			}
		}

		public sealed class OrReturnExactlyTests
		{
			[Fact]
			public async Task WhenAllMethodsReturnOneOfTheSpecifiedTypes_ShouldSucceed()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m
						=> m.Name == nameof(TestClass.GetString) || m.Name == nameof(TestClass.GetInt));

				async Task Act()
					=> await That(methods).ReturnExactly<string>().OrReturnExactly<int>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsReturnInheritedTypes_ShouldFail()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name == nameof(TestClass.GetDummy));

				async Task Act()
					=> await That(methods).ReturnExactly<DummyBase>().OrReturnExactly<string>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods matching m => m.Name == nameof(TestClass.GetDummy) in type ThatMethods.TestClass
					             all return exactly ThatMethods.DummyBase or exactly string,
					             but it contained not matching methods [
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]
					             """)
					.AsWildcard();
			}

			[Fact]
			public async Task WhenSomeMethodsDoNotReturnAnyOfTheSpecifiedTypes_ShouldFail()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name.StartsWith("Get"));

				async Task Act()
					=> await That(methods).ReturnExactly<string>().OrReturnExactly<int>();

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithMultipleOrReturnExactly_ShouldSupportChaining()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m
						=> m.Name == nameof(TestClass.GetString) || m.Name == nameof(TestClass.GetInt) ||
						   m.Name == nameof(TestClass.GetBool));

				async Task Act()
					=> await That(methods).ReturnExactly<string>().OrReturnExactly(typeof(int))
						.OrReturnExactly<bool>();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OrReturnTests
		{
			[Fact]
			public async Task WhenAllMethodsReturnOneOfTheSpecifiedTypes_ShouldSucceed()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m
						=> m.Name == nameof(TestClass.GetString) || m.Name == nameof(TestClass.GetInt));

				async Task Act()
					=> await That(methods).ReturnExactly<string>().OrReturn<int>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMethodsReturnInheritedTypes_ShouldFail()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name == nameof(TestClass.GetDummy));

				async Task Act()
					=> await That(methods).ReturnExactly<DummyBase>().OrReturn<string>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods matching m => m.Name == nameof(TestClass.GetDummy) in type ThatMethods.TestClass
					             all return exactly ThatMethods.DummyBase or string,
					             but it contained not matching methods [
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]
					             """)
					.AsWildcard();
			}

			[Fact]
			public async Task WhenSomeMethodsDoNotReturnAnyOfTheSpecifiedTypes_ShouldFail()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name.StartsWith("Get"));

				async Task Act()
					=> await That(methods).ReturnExactly<string>().OrReturn<int>();

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithMultipleOrReturnExactly_ShouldSupportChaining()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m
						=> m.Name == nameof(TestClass.GetString) || m.Name == nameof(TestClass.GetInt) ||
						   m.Name == nameof(TestClass.GetBool));

				async Task Act()
					=> await That(methods).ReturnExactly<string>().OrReturnExactly(typeof(int))
						.OrReturnExactly<bool>();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			public sealed class GenericTests
			{
				[Fact]
				public async Task WhenAllMethodsReturnSpecifiedType_ShouldFail()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m => m.Name == nameof(TestClass.GetString));

					async Task Act()
						=> await That(methods).DoesNotComplyWith(they => they.ReturnExactly<string>());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that methods matching m => m.Name == nameof(TestClass.GetString) in type ThatMethods.TestClass
						             not all return exactly string,
						             but it only contained matching methods [
						               string ThatMethods.TestClass.GetString()
						             ]
						             """)
						.AsWildcard();
				}

				[Fact]
				public async Task WhenMethodsReturnInheritedTypes_ShouldSucceed()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m => m.Name == nameof(TestClass.GetDummy));

					async Task Act()
						=> await That(methods).DoesNotComplyWith(they => they.ReturnExactly<DummyBase>());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSomeMethodsDoNotReturnSpecifiedType_ShouldSucceed()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m => m.Name.StartsWith("Get"));

					async Task Act()
						=> await That(methods).DoesNotComplyWith(they => they.ReturnExactly<string>());

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class OrReturnExactlyTests
			{
				[Fact]
				public async Task WhenAllMethodsReturnOneOfTheSpecifiedTypes_ShouldFail()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m
							=> m.Name == nameof(TestClass.GetString) || m.Name == nameof(TestClass.GetInt));

					async Task Act()
						=> await That(methods).DoesNotComplyWith(they
							=> they.ReturnExactly<string>().OrReturnExactly<int>());

					await That(Act).Throws<XunitException>();
				}

				[Fact]
				public async Task WhenSomeMethodsDoNotReturnAnyOfTheSpecifiedTypes_ShouldSucceed()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m => m.Name.StartsWith("Get"));

					async Task Act()
						=> await That(methods).DoesNotComplyWith(they
							=> they.ReturnExactly<string>().OrReturnExactly<int>());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithMultipleOrReturnExactly_ShouldSupportChaining()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m
							=> m.Name == nameof(TestClass.GetString) || m.Name == nameof(TestClass.GetInt));

					async Task Act()
						=> await That(methods)
							.DoesNotComplyWith(they => they.ReturnExactly<bool>().OrReturnExactly<Task>());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
