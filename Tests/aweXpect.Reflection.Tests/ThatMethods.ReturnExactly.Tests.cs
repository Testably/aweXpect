using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

#pragma warning disable CA2263 // tests intentionally exercise the non-generic Type overload

public sealed partial class ThatMethods
{
	public sealed class ReturnExactly
	{
		public sealed class EnumerableTests
		{
			[Test]
			public async Task ShouldFailWhenMethodsReturnInheritedType()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummy))!,
				];

				async Task Act()
				{
					await That(subject).ReturnExactly<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all return exactly ThatMethods.DummyBase,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]

					             Collection:
					             [
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]
					             """);
			}

			[Test]
			public async Task ShouldSucceedWhenAllMethodsReturnExactType()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetString))!,
				];

				async Task Act()
				{
					await That(subject).ReturnExactly<string>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class GenericTests
		{
			[Test]
			public async Task WhenAllMethodsReturnSpecifiedType_ShouldSucceed()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name == nameof(TestClass.GetString));

				async Task Act()
				{
					await That(methods).ReturnExactly<string>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsReturnInheritedTypes_ShouldFail()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name == nameof(TestClass.GetDummy));

				async Task Act()
				{
					await That(methods).ReturnExactly<DummyBase>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods matching m => m.Name == nameof(TestClass.GetDummy) in type ThatMethods.TestClass
					             all return exactly ThatMethods.DummyBase,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]

					             Collection:
					             [
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]
					             """);
			}

			[Test]
			public async Task WhenSomeMethodsDoNotReturnSpecifiedType_ShouldFail()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name.StartsWith("Get"));

				async Task Act()
				{
					await That(methods).ReturnExactly<string>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods matching m => m.Name.StartsWith("Get") in type ThatMethods.TestClass
					             all return exactly string,
					             but only 1 of 5 did

					             Not matching items:
					             [
					               int ThatMethods.TestClass.GetInt(),
					               bool ThatMethods.TestClass.GetBool(),
					               ThatMethods.DummyBase ThatMethods.TestClass.GetDummyBase(),
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]

					             Collection:
					             [
					               string ThatMethods.TestClass.GetString(),
					               int ThatMethods.TestClass.GetInt(),
					               bool ThatMethods.TestClass.GetBool(),
					               ThatMethods.DummyBase ThatMethods.TestClass.GetDummyBase(),
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]
					             """);
			}
		}

		public sealed class TypeTests
		{
			[Test]
			public async Task WhenAllMethodsReturnSpecifiedType_ShouldSucceed()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name == nameof(TestClass.GetString));

				async Task Act()
				{
					await That(methods).ReturnExactly(typeof(string));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsReturnInheritedTypes_ShouldFail()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name == nameof(TestClass.GetDummy));

				async Task Act()
				{
					await That(methods).ReturnExactly(typeof(DummyBase));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods matching m => m.Name == nameof(TestClass.GetDummy) in type ThatMethods.TestClass
					             all return exactly ThatMethods.DummyBase,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]

					             Collection:
					             [
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]
					             """);
			}

			[Test]
			public async Task WhenSomeMethodsDoNotReturnSpecifiedType_ShouldFail()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name.StartsWith("Get"));

				async Task Act()
				{
					await That(methods).ReturnExactly(typeof(string));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods matching m => m.Name.StartsWith("Get") in type ThatMethods.TestClass
					             all return exactly string,
					             but only 1 of 5 did

					             Not matching items:
					             [
					               int ThatMethods.TestClass.GetInt(),
					               bool ThatMethods.TestClass.GetBool(),
					               ThatMethods.DummyBase ThatMethods.TestClass.GetDummyBase(),
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]

					             Collection:
					             [
					               string ThatMethods.TestClass.GetString(),
					               int ThatMethods.TestClass.GetInt(),
					               bool ThatMethods.TestClass.GetBool(),
					               ThatMethods.DummyBase ThatMethods.TestClass.GetDummyBase(),
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]
					             """);
			}
		}

		public sealed class OrReturnExactlyTests
		{
			[Test]
			public async Task WhenAllMethodsReturnOneOfTheSpecifiedTypes_ShouldSucceed()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m
						=> m.Name == nameof(TestClass.GetString) || m.Name == nameof(TestClass.GetInt));

				async Task Act()
				{
					await That(methods).ReturnExactly<string>().OrReturnExactly<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsReturnInheritedTypes_ShouldFail()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name == nameof(TestClass.GetDummy));

				async Task Act()
				{
					await That(methods).ReturnExactly<DummyBase>().OrReturnExactly<string>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods matching m => m.Name == nameof(TestClass.GetDummy) in type ThatMethods.TestClass
					             all return exactly ThatMethods.DummyBase or exactly string,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]

					             Collection:
					             [
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]
					             """)
					.AsWildcard();
			}

			[Test]
			public async Task WhenSomeMethodsDoNotReturnAnyOfTheSpecifiedTypes_ShouldFail()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name.StartsWith("Get"));

				async Task Act()
				{
					await That(methods).ReturnExactly<string>().OrReturnExactly<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods matching m => m.Name.StartsWith("Get") in type ThatMethods.TestClass
					             all return exactly string or exactly int,
					             but only 2 of 5 did

					             Not matching items:
					             [
					               bool ThatMethods.TestClass.GetBool(),
					               ThatMethods.DummyBase ThatMethods.TestClass.GetDummyBase(),
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]

					             Collection:
					             [
					               string ThatMethods.TestClass.GetString(),
					               int ThatMethods.TestClass.GetInt(),
					               bool ThatMethods.TestClass.GetBool(),
					               ThatMethods.DummyBase ThatMethods.TestClass.GetDummyBase(),
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]
					             """);
			}

			[Test]
			public async Task WithMultipleOrReturnExactly_ShouldSupportChaining()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m
						=> m.Name == nameof(TestClass.GetString) || m.Name == nameof(TestClass.GetInt) ||
						   m.Name == nameof(TestClass.GetBool));

				async Task Act()
				{
					await That(methods).ReturnExactly<string>().OrReturnExactly(typeof(int))
						.OrReturnExactly<bool>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OrReturnTests
		{
			[Test]
			public async Task WhenAllMethodsReturnOneOfTheSpecifiedTypes_ShouldSucceed()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m
						=> m.Name == nameof(TestClass.GetString) || m.Name == nameof(TestClass.GetInt));

				async Task Act()
				{
					await That(methods).ReturnExactly<string>().OrReturn<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodsReturnInheritedTypes_ShouldFail()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name == nameof(TestClass.GetDummy));

				async Task Act()
				{
					await That(methods).ReturnExactly<DummyBase>().OrReturn<string>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods matching m => m.Name == nameof(TestClass.GetDummy) in type ThatMethods.TestClass
					             all return exactly ThatMethods.DummyBase or string,
					             but none of 1 did

					             Not matching items:
					             [
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]

					             Collection:
					             [
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]
					             """)
					.AsWildcard();
			}

			[Test]
			public async Task WhenSomeMethodsDoNotReturnAnyOfTheSpecifiedTypes_ShouldFail()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name.StartsWith("Get"));

				async Task Act()
				{
					await That(methods).ReturnExactly<string>().OrReturn<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods matching m => m.Name.StartsWith("Get") in type ThatMethods.TestClass
					             all return exactly string or int,
					             but only 2 of 5 did

					             Not matching items:
					             [
					               bool ThatMethods.TestClass.GetBool(),
					               ThatMethods.DummyBase ThatMethods.TestClass.GetDummyBase(),
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]

					             Collection:
					             [
					               string ThatMethods.TestClass.GetString(),
					               int ThatMethods.TestClass.GetInt(),
					               bool ThatMethods.TestClass.GetBool(),
					               ThatMethods.DummyBase ThatMethods.TestClass.GetDummyBase(),
					               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
					             ]
					             """);
			}

			[Test]
			public async Task WithMultipleOrReturnExactly_ShouldSupportChaining()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m
						=> m.Name == nameof(TestClass.GetString) || m.Name == nameof(TestClass.GetInt) ||
						   m.Name == nameof(TestClass.GetBool));

				async Task Act()
				{
					await That(methods).ReturnExactly<string>().OrReturnExactly(typeof(int))
						.OrReturnExactly<bool>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			public sealed class GenericTests
			{
				[Test]
				public async Task WhenAllMethodsReturnSpecifiedType_ShouldFail()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m => m.Name == nameof(TestClass.GetString));

					async Task Act()
					{
						await That(methods).DoesNotComplyWith(they => they.ReturnExactly<string>());
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that methods matching m => m.Name == nameof(TestClass.GetString) in type ThatMethods.TestClass
						             not all return exactly string,
						             but all 1 did

						             Collection:
						             [
						               string ThatMethods.TestClass.GetString()
						             ]
						             """)
						.AsWildcard();
				}

				[Test]
				public async Task WhenMethodsReturnInheritedTypes_ShouldSucceed()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m => m.Name == nameof(TestClass.GetDummy));

					async Task Act()
					{
						await That(methods).DoesNotComplyWith(they => they.ReturnExactly<DummyBase>());
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSomeMethodsDoNotReturnSpecifiedType_ShouldSucceed()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m => m.Name.StartsWith("Get"));

					async Task Act()
					{
						await That(methods).DoesNotComplyWith(they => they.ReturnExactly<string>());
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class OrReturnExactlyTests
			{
				[Test]
				public async Task WhenAllMethodsReturnOneOfTheSpecifiedTypes_ShouldFail()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m
							=> m.Name == nameof(TestClass.GetString) || m.Name == nameof(TestClass.GetInt));

					async Task Act()
					{
						await That(methods).DoesNotComplyWith(they
							=> they.ReturnExactly<string>().OrReturnExactly<int>());
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that methods matching m => m.Name == nameof(TestClass.GetString) || m.Name == nameof(TestClass.GetInt) in type ThatMethods.TestClass
						             not all return exactly string or exactly int,
						             but all 2 did

						             Collection:
						             [
						               string ThatMethods.TestClass.GetString(),
						               int ThatMethods.TestClass.GetInt()
						             ]
						             """);
				}

				[Test]
				public async Task WhenSomeMethodsDoNotReturnAnyOfTheSpecifiedTypes_ShouldSucceed()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m => m.Name.StartsWith("Get"));

					async Task Act()
					{
						await That(methods).DoesNotComplyWith(they
							=> they.ReturnExactly<string>().OrReturnExactly<int>());
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WithMultipleOrReturnExactly_ShouldSupportChaining()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m
							=> m.Name == nameof(TestClass.GetString) || m.Name == nameof(TestClass.GetInt));

					async Task Act()
					{
						await That(methods)
							.DoesNotComplyWith(they => they.ReturnExactly<bool>().OrReturnExactly<Task>());
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}

#pragma warning restore CA2263
