using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class HaveOutParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllHaveOutParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOutParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.AnotherMethodWithOutParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveOutParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllHaveOutParameter_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOutParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveOutParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have an out parameter,
					             but it contained methods without an out parameter *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WithType_WhenAllHaveOutParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOutParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveOutParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithType_WhenNotAllHaveOutParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOutParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveOutParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeAndName_WhenAllHaveOutParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOutParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveOutParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeAndName_WhenNotAllHaveOutParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOutParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveOutParameter(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeExactly_WhenAllHaveOutParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOutParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveOutParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeExactly_WhenNotAllHaveOutParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOutParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveOutParameterExactly(typeof(int));
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeExactlyAndName_WhenAllHaveOutParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOutParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveOutParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task AsyncEnumerable_WithType_WhenAllHaveOutParameterOfType_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOutParameter))!);

				async Task Act()
				{
					await That(methods).HaveOutParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WithType_WhenNotAllHaveOutParameterOfType_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOutParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!);

				async Task Act()
				{
					await That(methods).HaveOutParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>();
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllHaveOutParameter_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOutParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.AnotherMethodWithOutParameter))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveOutParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             not all have an out parameter,
					             but it only contained methods with an out parameter *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNotAllHaveOutParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOutParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveOutParameter());
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
			public void MethodWithOutParameter(out int value)
			{
				value = 0;
			}

			public void AnotherMethodWithOutParameter(out string text)
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
