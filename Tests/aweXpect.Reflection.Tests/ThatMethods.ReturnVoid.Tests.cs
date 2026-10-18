using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class ReturnVoid
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFailWhenSomeMethodsDoNotReturnVoid()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.VoidMethod))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt))!,
				];

				async Task Act()
				{
					await That(subject).ReturnVoid();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all return void,
					             but it contained not matching methods

					             Not matching items:
					             [
					               int ThatMethods.TestClass.GetInt()
					             ]
					             """);
			}

			[Test]
			public async Task ShouldSucceedWhenAllMethodsReturnVoid()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.VoidMethod))!,
				];

				async Task Act()
				{
					await That(subject).ReturnVoid();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OrReturnTests
		{
			[Test]
			public async Task WhenAllMethodsReturnOneOfTheTypes_ShouldSucceed()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.VoidMethod))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetInt))!,
				];

				async Task Act()
				{
					await That(subject).ReturnVoid().OrReturn<int>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeMethodsReturnNoneOfTheTypes_ShouldFail()
			{
				IEnumerable<MethodInfo> subject =
				[
					typeof(TestClass).GetMethod(nameof(TestClass.VoidMethod))!,
					typeof(TestClass).GetMethod(nameof(TestClass.GetString))!,
				];

				async Task Act()
				{
					await That(subject).ReturnVoid().OrReturn<int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all return void or int,
					             but it contained not matching methods

					             Not matching items:
					             [
					               string ThatMethods.TestClass.GetString()
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllMethodsReturnVoid_ShouldFail()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name == nameof(TestClass.VoidMethod));

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.ReturnVoid());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods matching m => m.Name == nameof(TestClass.VoidMethod) in type ThatMethods.TestClass
					             not all return void,
					             but it only contained matching methods

					             Collection:
					             [
					               void ThatMethods.TestClass.VoidMethod()
					             ]
					             """)
					.AsWildcard();
			}

			[Test]
			public async Task WhenSomeMethodsDoNotReturnVoid_ShouldSucceed()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name.StartsWith("Get"));

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.ReturnVoid());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
