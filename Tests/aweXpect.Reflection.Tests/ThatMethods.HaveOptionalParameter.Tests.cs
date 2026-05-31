using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class HaveOptionalParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllHaveOptionalParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.AnotherMethodWithOptionalParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameter();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameter();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have an optional parameter,
					             but it contained methods without an optional parameter *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WithType_WhenAllHaveOptionalParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithType_WhenNotAllHaveOptionalParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeAndName_WhenAllHaveOptionalParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameter(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeAndName_WhenNotAllHaveOptionalParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameter(typeof(int), "value");
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeExactly_WhenAllHaveOptionalParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameterExactly(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTypeExactly_WhenNotAllHaveOptionalParameterOfType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameterExactly(typeof(int));
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WithTypeExactlyAndName_WhenAllHaveOptionalParameterOfType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!,
				};

				async Task Act()
				{
					await That(methods).HaveOptionalParameterExactly(typeof(int), "value");
				}

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task AsyncEnumerable_WithType_WhenAllHaveOptionalParameterOfType_ShouldSucceed()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!);

				async Task Act()
				{
					await That(methods).HaveOptionalParameter(typeof(int));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AsyncEnumerable_WithType_WhenNotAllHaveOptionalParameterOfType_ShouldFail()
			{
				IAsyncEnumerable<MethodInfo> methods = ToAsyncEnumerable(
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!);

				async Task Act()
				{
					await That(methods).HaveOptionalParameter(typeof(int));
				}

				await That(Act).Throws<XunitException>();
			}
#endif
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllHaveOptionalParameter_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.AnotherMethodWithOptionalParameter))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveOptionalParameter());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             not all have an optional parameter,
					             but it only contained methods with an optional parameter *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNotAllHaveOptionalParameter_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithOptionalParameter))!,
					typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutModifiers))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveOptionalParameter());
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
			public void MethodWithOptionalParameter(int value = 0)
			{
			}

			public void AnotherMethodWithOptionalParameter(string text = "")
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
