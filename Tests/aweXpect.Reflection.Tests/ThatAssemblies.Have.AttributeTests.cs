using System.Collections.Generic;
using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed partial class Have
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task WhenAllAssembliesHaveAttribute_ShouldSucceed()
			{
				IEnumerable<Assembly> subjects = new[]
				{
					typeof(AttributeTests).Assembly,
				};

				async Task Act()
				{
					await That(subjects).Have<AssemblyTitleAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllAssembliesHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<Assembly> subjects = new[]
				{
					typeof(AttributeTests).Assembly,
				};

				async Task Act()
				{
					await That(subjects)
						.Have<AssemblyTitleAttribute>(attr => attr.Title == "aweXpect.Reflection.Tests");
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Assembly?> subject = [typeof(AttributeTests).Assembly, null,];

				async Task Act()
				{
					await That(subject).Have<AssemblyTitleAttribute>();
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they => they.Have<AssemblyTitleAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             all have AssemblyTitleAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null,
					               <null>
					             ]
					             """).AsWildcard();
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             not all have AssemblyTitleAttribute,
					             but for the item at index 1, it was <null>

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null,
					               <null>
					             ]
					             """).AsWildcard().Because("a null item cannot be inspected, like a null subject");
			}

			[Test]
			public async Task WhenNegated_ShouldListMatchingAssemblies()
			{
				IEnumerable<Assembly> subjects = new[]
				{
					typeof(AttributeTests).Assembly,
				};

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they => they.Have<AssemblyTitleAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subjects
					             not all have AssemblyTitleAttribute,
					             but all 1 did

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenNotAllAssembliesHaveAttribute_ShouldFail()
			{
				IEnumerable<Assembly> subjects = new[]
				{
					typeof(AttributeTests).Assembly,
				};

				async Task Act()
				{
					await That(subjects).Have<MarkerAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subjects
					             all have ThatAssemblies.Have.AttributeTests.MarkerAttribute,
					             but none of 1 did

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenNotAllAssembliesHaveMatchingAttribute_ShouldFail()
			{
				IEnumerable<Assembly> subjects = new[]
				{
					typeof(AttributeTests).Assembly,
				};

				async Task Act()
				{
					await That(subjects).Have<AssemblyTitleAttribute>(attr => attr.Title == "NonExistentTitle");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subjects
					             all have AssemblyTitleAttribute matching attr => attr.Title == "NonExistentTitle",
					             but none of 1 did

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
			private class MarkerAttribute : Attribute;

#if NET8_0_OR_GREATER
			[Test]
			public async Task AsyncEnumerable_WhenAllAssembliesHaveAttribute_ShouldSucceed()
			{
				IAsyncEnumerable<Assembly?> subjects = ToAsyncEnumerable(
					typeof(AttributeTests).Assembly);

				async Task Act()
				{
					await That(subjects).Have<AssemblyTitleAttribute>();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AsyncEnumerable_WhenNotAllAssembliesHaveAttribute_ShouldFail()
			{
				IAsyncEnumerable<Assembly?> subjects = ToAsyncEnumerable(
					typeof(AttributeTests).Assembly);

				async Task Act()
				{
					await That(subjects).Have<MarkerAttribute>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subjects
					             all have ThatAssemblies.Have.AttributeTests.MarkerAttribute,
					             but none of 1 did

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task AsyncEnumerable_WithPredicate_WhenNotAllAssembliesMatch_ShouldFail()
			{
				IAsyncEnumerable<Assembly?> subjects = ToAsyncEnumerable(
					typeof(AttributeTests).Assembly);

				async Task Act()
				{
					await That(subjects).Have<AssemblyTitleAttribute>(attr => attr.Title == "NonExistentTitle");
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subjects
					             all have AssemblyTitleAttribute matching attr => attr.Title == "NonExistentTitle",
					             but none of 1 did

					             Not matching items:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[Test]
			public async Task AsyncEnumerable_WithPredicate_WhenAllAssembliesMatch_ShouldSucceed()
			{
				IAsyncEnumerable<Assembly?> subjects = ToAsyncEnumerable(
					typeof(AttributeTests).Assembly);

				async Task Act()
				{
					await That(subjects)
						.Have<AssemblyTitleAttribute>(attr => attr.Title == "aweXpect.Reflection.Tests");
				}

				await That(Act).DoesNotThrow();
			}

			private static async IAsyncEnumerable<Assembly?> ToAsyncEnumerable(params Assembly?[] items)
			{
				foreach (Assembly? item in items)
				{
					yield return item;
				}

				await Task.CompletedTask;
			}
#endif
		}

		public sealed class OrHave
		{
			public sealed class AttributeTests
			{
				[Test]
				public async Task WhenAssembliesHaveBothAttributes_ShouldSucceed()
				{
					IEnumerable<Assembly> subjects = new[]
					{
						typeof(AttributeTests).Assembly,
					};

					async Task Act()
					{
						await That(subjects).Have<AssemblyTitleAttribute>().OrHave<AssemblyVersionAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAssembliesHaveFirstAttribute_ShouldSucceed()
				{
					IEnumerable<Assembly> subjects = new[]
					{
						typeof(AttributeTests).Assembly,
					};

					async Task Act()
					{
						await That(subjects).Have<AssemblyTitleAttribute>().OrHave<AssemblyVersionAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAssembliesHaveMatchingFirstAttribute_ShouldSucceed()
				{
					IEnumerable<Assembly> subjects = new[]
					{
						typeof(AttributeTests).Assembly,
					};

					async Task Act()
					{
						await That(subjects).Have<AssemblyTitleAttribute>(attr => attr.Title.Contains("Reflection"))
							.OrHave<MarkerAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAssembliesHaveMatchingSecondAttribute_ShouldSucceed()
				{
					IEnumerable<Assembly> subjects = new[]
					{
						typeof(AttributeTests).Assembly,
					};

					async Task Act()
					{
						await That(subjects).Have<MarkerAttribute>(attr => attr.Value == "NonExistent")
							.OrHave<AssemblyTitleAttribute>(attr => attr.Title.Contains("Reflection"));
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAssembliesHaveNeitherAttribute_ShouldFail()
				{
					IEnumerable<Assembly> subjects = new[]
					{
						typeof(AttributeTests).Assembly,
					};

					async Task Act()
					{
						await That(subjects).Have<MarkerAttribute>().OrHave<BarAttribute>();
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subjects
						             all have ThatAssemblies.Have.OrHave.AttributeTests.MarkerAttribute or ThatAssemblies.Have.OrHave.AttributeTests.BarAttribute,
						             but none of 1 did

						             Not matching items:
						             [
						               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
						             ]

						             Collection:
						             [
						               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
						             ]
						             """).AsWildcard();
				}

				[Test]
				public async Task WhenAssembliesHaveSecondAttribute_ShouldSucceed()
				{
					IEnumerable<Assembly> subjects = new[]
					{
						typeof(AttributeTests).Assembly,
					};

					async Task Act()
					{
						await That(subjects).Have<MarkerAttribute>().OrHave<AssemblyTitleAttribute>();
					}

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WithPredicate_WhenAssembliesHaveNotMatchingAttribute_ShouldFail()
				{
					IEnumerable<Assembly> subjects = new[]
					{
						typeof(AttributeTests).Assembly,
					};

					async Task Act()
					{
						await That(subjects).Have<AssemblyTitleAttribute>(attr => attr.Title == "NonExistent")
							.OrHave<MarkerAttribute>(attr => attr.Value == "NonExistent");
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subjects
						             all have AssemblyTitleAttribute matching attr => attr.Title == "NonExistent" or ThatAssemblies.Have.OrHave.AttributeTests.MarkerAttribute matching attr => attr.Value == "NonExistent",
						             but none of 1 did

						             Not matching items:
						             [
						               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
						             ]

						             Collection:
						             [
						               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
						             ]
						             """).AsWildcard();
				}

				[AttributeUsage(AttributeTargets.Assembly)]
				private class MarkerAttribute : Attribute
				{
					public string Value { get; } = "";
				}

				[AttributeUsage(AttributeTargets.Assembly)]
				private class BarAttribute : Attribute;
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenAssembliesDoNotHaveAttribute_ShouldSucceed()
			{
				IEnumerable<Assembly> subjects = new[]
				{
					Assembly.GetExecutingAssembly(),
				};

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they => they.Have<MarkerAttribute>());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssembliesDoNotHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<Assembly> subjects = new[]
				{
					Assembly.GetExecutingAssembly(),
				};

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they
						=> they.Have<AssemblyTitleAttribute>(attr => attr.Title == "NonExistent"));
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAssembliesHaveAttribute_ShouldFail()
			{
				IEnumerable<Assembly> subjects = new[]
				{
					Assembly.GetExecutingAssembly(),
				};

				async Task Act()
				{
					await That(subjects).DoesNotComplyWith(they => they.Have<AssemblyTitleAttribute>());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subjects
					             not all have AssemblyTitleAttribute,
					             but all 1 did

					             Collection:
					             [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[AttributeUsage(AttributeTargets.Assembly)]
			private class MarkerAttribute : Attribute;
		}
	}
}
