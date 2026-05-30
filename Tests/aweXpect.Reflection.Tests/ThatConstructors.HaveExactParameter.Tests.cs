using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed class HaveExactParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllHaveExactParameter_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(Stream),])!, typeof(OtherClass).GetConstructor([typeof(Stream),])!,
				};

				async Task Act()
				{
					await That(constructors).HaveExactParameter<Stream>();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllHaveExactParameterWithName_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(Stream),])!,
				};

				async Task Act()
				{
					await That(constructors).HaveExactParameter<Stream>("stream");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAnyParameterIsSubtype_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(Stream),])!,
				};

				async Task Act()
				{
					await That(constructors).HaveExactParameter<IDisposable>();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             all have parameter of exact type IDisposable,
					             but at least one did not
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllHaveExactParameter_ShouldFail()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(Stream),])!, typeof(OtherClass).GetConstructor([typeof(Stream),])!,
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveExactParameter<Stream>());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors
					             not all have parameter of exact type Stream,
					             but all did
					             """);
			}

			[Fact]
			public async Task WhenAnyParameterIsSubtype_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> constructors = new[]
				{
					typeof(TestClass).GetConstructor([typeof(Stream),])!,
				};

				async Task Act()
				{
					await That(constructors).DoesNotComplyWith(they => they.HaveExactParameter<IDisposable>());
				}

				await That(Act).DoesNotThrow();
			}
		}

		// ReSharper disable UnusedParameter.Local
		// ReSharper disable UnusedMember.Local
		private class TestClass
		{
			public TestClass(Stream stream) { }
		}

		private class OtherClass
		{
			public OtherClass(Stream stream) { }
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
