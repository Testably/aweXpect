using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethods
{
	public sealed class HaveExactParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllHaveExactType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!, typeof(TestClass).GetMethod(nameof(TestClass.SecondMethodWithStream))!,
				};

				async Task Act()
				{
					await That(methods).HaveExactParameter<Stream>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllHaveExactTypeWithName_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!, typeof(TestClass).GetMethod(nameof(TestClass.SecondMethodWithStream))!,
				};

				async Task Act()
				{
					await That(methods).HaveExactParameter<Stream>("stream");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllHaveExactType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithMemoryStream))!,
				};

				async Task Act()
				{
					await That(methods).HaveExactParameter<Stream>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             all have parameter of exact type Stream,
					             but at least one did not
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllHaveExactType_ShouldFail()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!, typeof(TestClass).GetMethod(nameof(TestClass.SecondMethodWithStream))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveExactParameter<Stream>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that methods
					             not all have parameter of exact type Stream,
					             but all did
					             """);
			}

			[Fact]
			public async Task WhenNotAllHaveExactType_ShouldSucceed()
			{
				IEnumerable<MethodInfo> methods = new[]
				{
					typeof(TestClass).GetMethod(nameof(TestClass.FirstMethodWithStream))!, typeof(TestClass).GetMethod(nameof(TestClass.MethodWithMemoryStream))!,
				};

				async Task Act()
				{
					await That(methods).DoesNotComplyWith(they => they.HaveExactParameter<Stream>());
				}

				await That(Act).DoesNotThrow();
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public void FirstMethodWithStream(Stream stream) { }
			public void SecondMethodWithStream(Stream stream) { }
			public void MethodWithMemoryStream(MemoryStream stream) { }
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
