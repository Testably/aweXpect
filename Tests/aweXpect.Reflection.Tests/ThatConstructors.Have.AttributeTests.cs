using System.Collections.Generic;
using System.Reflection;
#if NET8_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructors
{
	public sealed partial class Have
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenAllConstructorsHaveAttribute_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(string),])!, typeof(TestClass).GetConstructor([typeof(int),])!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllConstructorsHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(string),])!, typeof(TestClass).GetConstructor([typeof(int),])!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>(attr => attr.Value.StartsWith("Constructor"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenCheckingTheResult_ShouldNotOfferDirectly()
			{
				async Task Act()
				{
					await That(typeof(Reflection.Results.HaveAttributeWithoutInheritResult<,>))
						.ContainsMethods(methods => methods.WithName("Directly")).Never();
				}

				await That(Act).DoesNotThrow()
					.Because("the attributes of assemblies, constructors and fields cannot be inherited");
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<ConstructorInfo?> subject = [typeof(TestClass).GetConstructor([typeof(string),]), null,];

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.Have<MarkerAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatConstructors.Have.AttributeTests.MarkerAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatConstructors.Have.AttributeTests.TestClass(string value),
					               <null>
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all have ThatConstructors.Have.AttributeTests.MarkerAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               ThatConstructors.Have.AttributeTests.TestClass(string value),
					               <null>
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNotAllConstructorsHaveAttribute_ShouldFail()
			{
				IEnumerable<ConstructorInfo> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(string),])!, typeof(TestClass).GetConstructor(Type.EmptyTypes)!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatConstructors.Have.AttributeTests.MarkerAttribute,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ThatConstructors.Have.AttributeTests.TestClass()
					             ]

					             Collection:
					             [
					               ThatConstructors.Have.AttributeTests.TestClass(string value),
					               ThatConstructors.Have.AttributeTests.TestClass()
					             ]
					             """);
			}

			[Test]
			public async Task WhenNotAllConstructorsHaveMatchingAttribute_ShouldFail()
			{
				IEnumerable<ConstructorInfo> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(string),])!, typeof(TestClass).GetConstructor([typeof(int),])!,
				};

				async Task Act()
				{
					await That(subject).Have<MarkerAttribute>(attr => attr.Value == "WrongValue");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have ThatConstructors.Have.AttributeTests.MarkerAttribute matching attr => attr.Value == "WrongValue",
					             but none of 2 did

					             Not matching items:
					             [
					               ThatConstructors.Have.AttributeTests.TestClass(string value),
					               ThatConstructors.Have.AttributeTests.TestClass(int value)
					             ]

					             Collection:
					             [
					               ThatConstructors.Have.AttributeTests.TestClass(string value),
					               ThatConstructors.Have.AttributeTests.TestClass(int value)
					             ]
					             """);
			}

#if NET8_0_OR_GREATER
			public sealed class AsyncEnumerableTests
			{
				[Test]
				public async Task WhenAllConstructorsHaveAttribute_ShouldSucceed()
				{
					IAsyncEnumerable<ConstructorInfo?> subject = new[]
					{
						typeof(TestClass).GetConstructor([typeof(string),])!, typeof(TestClass).GetConstructor([typeof(int),])!,
					}.ToTestAsyncEnumerable<ConstructorInfo?>();

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAllConstructorsHaveMatchingAttribute_ShouldSucceed()
				{
					IAsyncEnumerable<ConstructorInfo?> subject = new[]
					{
						typeof(TestClass).GetConstructor([typeof(string),])!, typeof(TestClass).GetConstructor([typeof(int),])!,
					}.ToTestAsyncEnumerable<ConstructorInfo?>();

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value.StartsWith("Constructor"));
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenNotAllConstructorsHaveAttribute_ShouldFail()
				{
					IAsyncEnumerable<ConstructorInfo?> subject = new[]
					{
						typeof(TestClass).GetConstructor([typeof(string),])!, typeof(TestClass).GetConstructor(Type.EmptyTypes)!,
					}.ToTestAsyncEnumerable<ConstructorInfo?>();

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatConstructors.Have.AttributeTests.MarkerAttribute,
						             but only 1 of 2 did

						             Not matching items:
						             [
						               ThatConstructors.Have.AttributeTests.TestClass()
						             ]

						             Collection:
						             [
						               ThatConstructors.Have.AttributeTests.TestClass(string value),
						               ThatConstructors.Have.AttributeTests.TestClass()
						             ]
						             """);
				}

				[Test]
				public async Task WhenNotAllConstructorsHaveMatchingAttribute_ShouldFail()
				{
					IAsyncEnumerable<ConstructorInfo?> subject = new[]
					{
						typeof(TestClass).GetConstructor([typeof(string),])!, typeof(TestClass).GetConstructor([typeof(int),])!,
					}.ToTestAsyncEnumerable<ConstructorInfo?>();

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value == "WrongValue");
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatConstructors.Have.AttributeTests.MarkerAttribute matching attr => attr.Value == "WrongValue",
						             but none of 2 did

						             Not matching items:
						             [
						               ThatConstructors.Have.AttributeTests.TestClass(string value),
						               ThatConstructors.Have.AttributeTests.TestClass(int value)
						             ]

						             Collection:
						             [
						               ThatConstructors.Have.AttributeTests.TestClass(string value),
						               ThatConstructors.Have.AttributeTests.TestClass(int value)
						             ]
						             """);
				}
			}
#endif

			[AttributeUsage(AttributeTargets.Constructor)]
			private class MarkerAttribute : Attribute
			{
				public string Value { get; set; } = "";
			}

			// ReSharper disable UnusedMember.Local
			// ReSharper disable UnusedParameter.Local
			private class TestClass
			{
				public TestClass() { }

				[Marker(Value = "ConstructorValue1")]
				public TestClass(string value) { }

				[Marker(Value = "ConstructorValue2")]
				public TestClass(int value) { }
			}
			// ReSharper restore UnusedParameter.Local
			// ReSharper restore UnusedMember.Local
		}

		public sealed class OrHave
		{
			public sealed class AttributeTests
			{
				[Test]
				public async Task WhenConstructorsHaveBothAttributes_ShouldSucceed()
				{
					IEnumerable<ConstructorInfo> subject = new[]
					{
						typeof(TestClass).GetConstructor([typeof(long),])!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenConstructorsHaveFirstAttribute_ShouldSucceed()
				{
					IEnumerable<ConstructorInfo> subject = new[]
					{
						typeof(TestClass).GetConstructor([typeof(string),])!, typeof(TestClass).GetConstructor([typeof(int),])!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenConstructorsHaveMatchingFirstAttribute_ShouldSucceed()
				{
					IEnumerable<ConstructorInfo> subject = new[]
					{
						typeof(TestClass).GetConstructor([typeof(string),])!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>(attr => attr.Value == "Constructor1Value")
							.OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenConstructorsHaveMatchingSecondAttribute_ShouldSucceed()
				{
					IEnumerable<ConstructorInfo> subject = new[]
					{
						typeof(TestClass).GetConstructor([typeof(double),])!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>()
							.OrHave<BarAttribute>(attr => attr.Name == "bar");
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenConstructorsHaveNeitherAttribute_ShouldFail()
				{
					IEnumerable<ConstructorInfo> subject = new[]
					{
						typeof(TestClass).GetConstructor(Type.EmptyTypes)!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             all have ThatConstructors.Have.OrHave.AttributeTests.MarkerAttribute or ThatConstructors.Have.OrHave.AttributeTests.BarAttribute,
						             but none of 1 did

						             Not matching items:
						             [
						               ThatConstructors.Have.OrHave.AttributeTests.TestClass()
						             ]

						             Collection:
						             [
						               ThatConstructors.Have.OrHave.AttributeTests.TestClass()
						             ]
						             """);
				}

				[Test]
				public async Task WhenConstructorsHaveSecondAttribute_ShouldSucceed()
				{
					IEnumerable<ConstructorInfo> subject = new[]
					{
						typeof(TestClass).GetConstructor([typeof(double),])!, typeof(TestClass).GetConstructor([typeof(float),])!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WithInheritance_ShouldWorkCorrectly()
				{
					IEnumerable<ConstructorInfo> subject = new[]
					{
						typeof(TestClass).GetConstructor([typeof(string),])!,
					};

					async Task Act()
					{
						await That(subject).Have<MarkerAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[AttributeUsage(AttributeTargets.Constructor)]
				private class MarkerAttribute : Attribute
				{
					public string Value { get; set; } = "";
				}

				[AttributeUsage(AttributeTargets.Constructor)]
				private class BarAttribute : Attribute
				{
					public string Name { get; set; } = "";
				}

				// ReSharper disable UnusedMember.Local
				// ReSharper disable UnusedParameter.Local
				private class TestClass
				{
					public TestClass() { }

					[Marker(Value = "Constructor1Value")]
					public TestClass(string value) { }

					[Marker(Value = "Constructor2Value")]
					public TestClass(int value) { }

					[Bar(Name = "bar")]
					public TestClass(double value) { }

					[Bar(Name = "bar2")]
					public TestClass(float value) { }

					[Marker(Value = "BothValue")]
					[Bar(Name = "both")]
					public TestClass(long value) { }
				}
				// ReSharper restore UnusedParameter.Local
				// ReSharper restore UnusedMember.Local
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAllConstructorsHaveAttribute_ShouldFail()
			{
				IEnumerable<ConstructorInfo> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(string),])!, typeof(TestClass).GetConstructor([typeof(int),])!,
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Have<MarkerAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all have ThatConstructors.Have.NegatedTests.MarkerAttribute,
					             but all 2 did

					             Collection:
					             [
					               ThatConstructors.Have.NegatedTests.TestClass(string value),
					               ThatConstructors.Have.NegatedTests.TestClass(int value)
					             ]
					             """);
			}

			[Test]
			public async Task WhenNotAllConstructorsHaveAttribute_ShouldSucceed()
			{
				IEnumerable<ConstructorInfo> subject = new[]
				{
					typeof(TestClass).GetConstructor([typeof(string),])!, typeof(TestClass).GetConstructor(Type.EmptyTypes)!,
				};

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Have<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[AttributeUsage(AttributeTargets.Constructor)]
			private class MarkerAttribute : Attribute
			{
				// ReSharper disable once UnusedAutoPropertyAccessor.Local
				public string Value { get; set; } = "";
			}

			// ReSharper disable UnusedMember.Local
			// ReSharper disable UnusedParameter.Local
			private class TestClass
			{
				public TestClass() { }

				[Marker(Value = "Constructor1Value")]
				public TestClass(string value) { }

				[Marker(Value = "Constructor2Value")]
				public TestClass(int value) { }
			}
			// ReSharper restore UnusedParameter.Local
			// ReSharper restore UnusedMember.Local
		}
	}
}
