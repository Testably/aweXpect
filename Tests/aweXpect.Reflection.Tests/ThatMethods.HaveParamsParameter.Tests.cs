using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class HaveParamsParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllHaveParamsParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.AnotherMethodWithParamsParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveParamsParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllHaveParamsParameter_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveParamsParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have a params parameter,
					             but it contained methods without a params parameter *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WithType_WhenAllHaveParamsParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveParamsParameter(typeof(int[]));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithType_WhenNotAllHaveParamsParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveParamsParameter(typeof(int[]));
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeAndName_WhenAllHaveParamsParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveParamsParameter(typeof(int[]), "values");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeAndName_WhenNotAllHaveParamsParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveParamsParameter(typeof(int[]), "values");
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeExactly_WhenAllHaveParamsParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveParamsParameterExactly(typeof(int[]));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeExactly_WhenNotAllHaveParamsParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveParamsParameterExactly(typeof(int[]));
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeExactlyAndName_WhenAllHaveParamsParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveParamsParameterExactly(typeof(int[]), "values");
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task AsyncEnumerable_WithType_WhenAllHaveParamsParameterOfType_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!);

				async Task Act()
				{
					await That(methods).HaveParamsParameter(typeof(int[]));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WithType_WhenNotAllHaveParamsParameterOfType_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!);

				async Task Act()
				{
					await That(methods).HaveParamsParameter(typeof(int[]));
				}

				await That(Act).Throws<XunitException>();
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllHaveParamsParameter_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.AnotherMethodWithParamsParameter))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveParamsParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             not all have a params parameter,
					             but it only contained methods with a params parameter *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNotAllHaveParamsParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithParamsParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveParamsParameter());
				}

				await That(Act).DoesNotThrow();
			}
		}

#if NET8_0_OR_GREATER
		private static async IAsyncEnumerable<MethodInfo> ToAsyncEnumerable(params MethodInfo[] items)
		{
			foreach (MethodInfo item in items)
			{
				yield return item;
			}

			await Task.CompletedTask;
		}
#endif

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public void MethodWithParamsParameter(params int[] values)
			{
			}

			public void AnotherMethodWithParamsParameter(params string[] texts)
			{
			}

			public void MethodWithoutModifiers(int[] values)
			{
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
#pragma warning restore CA1822
	}
}
