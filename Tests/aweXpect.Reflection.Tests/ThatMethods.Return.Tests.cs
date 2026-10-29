using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

#pragma warning disable CA2263 // tests intentionally exercise the non-generic Type overload

public sealed partial class ThatMethods
{
	public sealed partial class Return
	{
		public sealed class EnumerableTests
		{
			[Test]
			public async Task ShouldFailWhenMethodIsNull()
			{
				IEnumerable<MethodInfo?> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetString)),
					null,
				];

				async Task Act()
				{
					await That(subject).Return<string>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all return string,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               string ThatMethods.TestClass.GetString(),
					               <null>
					             ]
					             """);
			}

			[Test]
			public async Task ShouldFailWhenSomeMethodsDoNotReturnSpecifiedType()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetString))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt))!,
				];

				async Task Act()
				{
					await That(subject).Return<string>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all return string,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               int ThatMethods.TestClass.GetInt()
					             ]

					             Collection:
					             [
					               string ThatMethods.TestClass.GetString(),
					               int ThatMethods.TestClass.GetInt()
					             ]
					             """);
			}

			[Test]
			public async Task ShouldSucceedWhenAllMethodsReturnSpecifiedType()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetString))!,
				];

				async Task Act()
				{
					await That(subject).Return<string>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ShouldSucceedWithInheritance()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.GetDummy))!,
				];

				async Task Act()
				{
					await That(subject).Return<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class GenericTests
		{
			[Test]
			public async Task ShouldFailWhenSomeMethodsDoNotReturnSpecifiedType()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name.StartsWith("Get"));

				async Task Act()
				{
					await That(methods).Return<string>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods matching m => m.Name.StartsWith("Get") in type ThatMethods.TestClass
					             all return string,
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

			[Test]
			public async Task ShouldSucceedWhenAllMethodsReturnSpecifiedType()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name == nameof(TestClass.GetString));

				async Task Act()
				{
					await That(methods).Return<string>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ShouldSucceedWithInheritance()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name == nameof(TestClass.GetDummy));

				async Task Act()
				{
					await That(methods).Return<DummyBase>();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class TypeTests
		{
			[Test]
			public async Task ShouldFailWhenSomeMethodsDoNotReturnSpecifiedType()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name.StartsWith("Get"));

				async Task Act()
				{
					await That(methods).Return(typeof(string));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods matching m => m.Name.StartsWith("Get") in type ThatMethods.TestClass
					             all return string,
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

			[Test]
			public async Task ShouldSucceedWhenAllMethodsReturnSpecifiedType()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name == nameof(TestClass.GetString));

				async Task Act()
				{
					await That(methods).Return(typeof(string));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task ShouldSucceedWithInheritance()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name == nameof(TestClass.GetDummy));

				async Task Act()
				{
					await That(methods).Return(typeof(DummyBase));
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OrReturnTests
		{
			[Test]
			public async Task WithMultipleOrReturn_ShouldSupportChaining()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name.StartsWith("Get"));

				await That(methods).Return<int>().OrReturn<string>().OrReturn(typeof(bool)).OrReturn<DummyBase>();
			}

			[Test]
			public async Task WithOrReturn_WhenAllMethodsReturnOneOfTheTypes_ShouldSucceed()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name is nameof(TestClass.GetString) or nameof(TestClass.GetInt));

				await That(methods).Return<string>().OrReturn(typeof(int));
			}

			[Test]
			public async Task WithOrReturn_WhenSomeMethodsDoNotReturnAnyOfTheTypes_ShouldFail()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name.StartsWith("Get"));

				async Task Act()
				{
					await That(methods).Return<bool>().OrReturn<Task>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods matching m => m.Name.StartsWith("Get") in type ThatMethods.TestClass
					             all return bool or Task,
					             but only 1 of 5 did

					             Not matching items:
					             [
					               string ThatMethods.TestClass.GetString(),
					               int ThatMethods.TestClass.GetInt(),
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
			public async Task WithMultipleOrReturn_ShouldSupportChaining()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name.StartsWith("Get"));

				await That(methods).Return<int>().OrReturn<string>().OrReturnExactly(typeof(bool))
					.OrReturn<DummyBase>();
			}

			[Test]
			public async Task WithOrReturn_WhenAllMethodsReturnOneOfTheTypes_ShouldSucceed()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name is nameof(TestClass.GetString) or nameof(TestClass.GetInt));

				await That(methods).Return<string>().OrReturnExactly(typeof(int));
			}

			[Test]
			public async Task WithOrReturn_WhenSomeMethodsDoNotReturnAnyOfTheTypes_ShouldFail()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name.StartsWith("Get"));

				async Task Act()
				{
					await That(methods).Return<bool>().OrReturnExactly<Task>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods matching m => m.Name.StartsWith("Get") in type ThatMethods.TestClass
					             all return bool or exactly Task,
					             but only 1 of 5 did

					             Not matching items:
					             [
					               string ThatMethods.TestClass.GetString(),
					               int ThatMethods.TestClass.GetInt(),
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

		public sealed class NegatedTests
		{
			public sealed class EnumerableTests
			{
				[Test]
				public async Task WhenMethodIsNull_ShouldFail()
				{
					IEnumerable<MethodInfo?> subject =
					[
						typeof(TestClass).GetMethod(nameof(TestClass.GetString)),
						null,
					];

					async Task Act()
					{
						await That(subject).DoesNotComplyWith(they => they.Return<string>());
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             not all return string,
						             but for the item at index 1, it was <null>

						             Collection:
						             [
						               string ThatMethods.TestClass.GetString(),
						               <null>
						             ]
						             """).Because("a null item cannot be inspected, like a null subject");
				}
			}

			public sealed class GenericTests
			{
				[Test]
				public async Task WhenAllMethodsReturnSpecifiedType_ShouldFail()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m => m.Name == nameof(TestClass.GetString));

					async Task Act()
					{
						await That(methods).DoesNotComplyWith(they => they.Return<string>());
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that methods matching m => m.Name == nameof(TestClass.GetString) in type ThatMethods.TestClass
						             not all return string,
						             but all 1 did

						             Collection:
						             [
						               string ThatMethods.TestClass.GetString()
						             ]
						             """)
						.AsWildcard();
				}

				[Test]
				public async Task WhenMethodsReturnInheritedTypes_ShouldFail()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m => m.Name == nameof(TestClass.GetDummy));

					async Task Act()
					{
						await That(methods).DoesNotComplyWith(they => they.Return<DummyBase>());
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that methods matching m => m.Name == nameof(TestClass.GetDummy) in type ThatMethods.TestClass
						             not all return ThatMethods.DummyBase,
						             but all 1 did

						             Collection:
						             [
						               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
						             ]
						             """)
						.AsWildcard();
				}

				[Test]
				public async Task WhenSomeMethodsDoNotReturnSpecifiedType_ShouldSucceed()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m => m.Name.StartsWith("Get"));

					async Task Act()
					{
						await That(methods).DoesNotComplyWith(they => they.Return<string>());
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class TypeTests
			{
				[Test]
				public async Task WhenAllMethodsReturnSpecifiedType_ShouldFail()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m => m.Name == nameof(TestClass.GetString));

					async Task Act()
					{
						await That(methods).DoesNotComplyWith(they => they.Return(typeof(string)));
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that methods matching m => m.Name == nameof(TestClass.GetString) in type ThatMethods.TestClass
						             not all return string,
						             but all 1 did

						             Collection:
						             [
						               string ThatMethods.TestClass.GetString()
						             ]
						             """)
						.AsWildcard();
				}

				[Test]
				public async Task WhenMethodsReturnInheritedTypes_ShouldFail()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m => m.Name == nameof(TestClass.GetDummy));

					async Task Act()
					{
						await That(methods).DoesNotComplyWith(they => they.Return(typeof(DummyBase)));
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that methods matching m => m.Name == nameof(TestClass.GetDummy) in type ThatMethods.TestClass
						             not all return ThatMethods.DummyBase,
						             but all 1 did

						             Collection:
						             [
						               ThatMethods.Dummy ThatMethods.TestClass.GetDummy()
						             ]
						             """)
						.AsWildcard();
				}

				[Test]
				public async Task WhenSomeMethodsDoNotReturnSpecifiedType_ShouldSucceed()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m => m.Name.StartsWith("Get"));

					async Task Act()
					{
						await That(methods).DoesNotComplyWith(they => they.Return(typeof(string)));
					}

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class OrReturnTests
			{
				[Test]
				public async Task WithMultipleOrReturn_ShouldSupportChaining()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m => m.Name == nameof(TestClass.AsyncMethod));

					async Task Act()
					{
						await That(methods).DoesNotComplyWith(they
							=> they.Return<int>().OrReturn<string>().OrReturn(typeof(bool)).OrReturn<DummyBase>());
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WithOrReturn_WhenAllMethodsReturnOneOfTheTypes_ShouldFail()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m => m.Name is nameof(TestClass.GetString) or nameof(TestClass.GetInt));

					async Task Act()
					{
						await That(methods).DoesNotComplyWith(they => they.Return<string>().OrReturn(typeof(int)));
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that methods matching m => m.Name is nameof(TestClass.GetString) or nameof(TestClass.GetInt) in type ThatMethods.TestClass
						             not all return string or int,
						             but all 2 did

						             Collection:
						             [
						               string ThatMethods.TestClass.GetString(),
						               int ThatMethods.TestClass.GetInt()
						             ]
						             """)
						.AsWildcard();
				}

				[Test]
				public async Task WithOrReturn_WhenSomeMethodsDoNotReturnAnyOfTheTypes_ShouldSucceed()
				{
					Filtered.Methods methods = In.Type<TestClass>()
						.Methods().Which(m => m.Name.StartsWith("Get"));

					async Task Act()
					{
						await That(methods).DoesNotComplyWith(they => they.Return<bool>().OrReturn<Task>());
					}

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}

#pragma warning restore CA2263
