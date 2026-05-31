using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class ReturnVoid
	{
		public sealed class Tests
		{
			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all return void,
					             but it contained not matching methods [
					               int ThatMethods.TestClass.GetInt()
					             ]
					             """);
			}

			[Fact]
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
			[Fact]
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

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             all return void or int,
					             but it contained not matching methods [
					               string ThatMethods.TestClass.GetString()
					             ]
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllMethodsReturnVoid_ShouldFail()
			{
				Filtered.Methods methods = In.Type<TestClass>()
					.Methods().Which(m => m.Name == nameof(TestClass.VoidMethod));

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.ReturnVoid());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods matching m => m.Name == nameof(TestClass.VoidMethod) in type ThatMethods.TestClass
					             not all return void,
					             but it only contained matching methods [
					               void ThatMethods.TestClass.VoidMethod()
					             ]
					             """)
					.AsWildcard();
			}

			[Fact]
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
