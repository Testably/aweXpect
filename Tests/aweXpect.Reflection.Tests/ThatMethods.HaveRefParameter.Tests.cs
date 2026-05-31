using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class HaveRefParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllHaveRefParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.AnotherMethodWithRefParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllHaveRefParameter_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have a ref parameter,
					             but it contained methods without a ref parameter *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WithType_WhenAllHaveRefParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithType_WhenNotAllHaveRefParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeAndName_WhenAllHaveRefParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeAndName_WhenNotAllHaveRefParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameter(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeExactly_WhenAllHaveRefParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeExactly_WhenNotAllHaveRefParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameterExactly(typeof(int));
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeExactlyAndName_WhenAllHaveRefParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveRefParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task AsyncEnumerable_WithType_WhenAllHaveRefParameterOfType_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!);

				async Task Act()
				{
					await That(methods).HaveRefParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WithType_WhenNotAllHaveRefParameterOfType_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!);

				async Task Act()
				{
					await That(methods).HaveRefParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>();
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllHaveRefParameter_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.AnotherMethodWithRefParameter))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveRefParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             not all have a ref parameter,
					             but it only contained methods with a ref parameter *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNotAllHaveRefParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithRefParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveRefParameter());
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
			public void MethodWithRefParameter(ref int value)
			{
				value = 0;
			}

			public void AnotherMethodWithRefParameter(ref string text)
			{
				text = string.Empty;
			}

			public void MethodWithoutModifiers(int value)
			{
			}
		}
		// ReSharper restore UnusedMember.Local
		// ReSharper restore UnusedParameter.Local
#pragma warning restore CA1822
	}
}
