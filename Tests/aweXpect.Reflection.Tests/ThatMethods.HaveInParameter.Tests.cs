using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class HaveInParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllHaveInParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.AnotherMethodWithInParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveInParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllHaveInParameter_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveInParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have an in parameter,
					             but it contained methods without an in parameter *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WithType_WhenAllHaveInParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveInParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithType_WhenNotAllHaveInParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveInParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeAndName_WhenAllHaveInParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveInParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeAndName_WhenNotAllHaveInParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveInParameter(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeExactly_WhenAllHaveInParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveInParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeExactly_WhenNotAllHaveInParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveInParameterExactly(typeof(int));
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeExactlyAndName_WhenAllHaveInParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveInParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task AsyncEnumerable_WithType_WhenAllHaveInParameterOfType_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInParameter))!);

				async Task Act()
				{
					await That(methods).HaveInParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WithType_WhenNotAllHaveInParameterOfType_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!);

				async Task Act()
				{
					await That(methods).HaveInParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>();
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllHaveInParameter_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.AnotherMethodWithInParameter))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveInParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             not all have an in parameter,
					             but it only contained methods with an in parameter *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNotAllHaveInParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithInParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveInParameter());
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
			public void MethodWithInParameter(in int value)
			{
			}

			public void AnotherMethodWithInParameter(in string text)
			{
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
