using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed class Have
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task WhenAllAssembliesHaveAttribute_ShouldSucceed()
			{
				IEnumerable<Assembly> subjects = new[]
				{
					typeof(AttributeTests).Assembly,
				};

				async Task Act()
					=> await That(subjects).Have<AssemblyTitleAttribute>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllAssembliesHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<Assembly> subjects = new[]
				{
					typeof(AttributeTests).Assembly,
				};

				async Task Act()
					=> await That(subjects)
						.Have<AssemblyTitleAttribute>(attr => attr.Title == "aweXpect.Reflection.Tests");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNotAllAssembliesHaveAttribute_ShouldFail()
			{
				IEnumerable<Assembly> subjects = new[]
				{
					typeof(AttributeTests).Assembly,
				};

				async Task Act()
					=> await That(subjects).Have<TestAttribute>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subjects
					             all have ThatAssemblies.Have.AttributeTests.TestAttribute,
					             but it contained not matching assemblies [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNotAllAssembliesHaveMatchingAttribute_ShouldFail()
			{
				IEnumerable<Assembly> subjects = new[]
				{
					typeof(AttributeTests).Assembly,
				};

				async Task Act()
					=> await That(subjects).Have<AssemblyTitleAttribute>(attr => attr.Title == "NonExistentTitle");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subjects
					             all have AssemblyTitleAttribute matching attr => attr.Title == "NonExistentTitle",
					             but it contained not matching assemblies [
					               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
					             ]
					             """).AsWildcard();
			}

			[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
			private class TestAttribute : Attribute;
		}

		public sealed class OrHave
		{
			public sealed class AttributeTests
			{
				[Fact]
				public async Task WhenAssembliesHaveBothAttributes_ShouldSucceed()
				{
					IEnumerable<Assembly> subjects = new[]
					{
						typeof(AttributeTests).Assembly,
					};

					async Task Act()
						=> await That(subjects).Have<AssemblyTitleAttribute>().OrHave<AssemblyVersionAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAssembliesHaveFirstAttribute_ShouldSucceed()
				{
					IEnumerable<Assembly> subjects = new[]
					{
						typeof(AttributeTests).Assembly,
					};

					async Task Act()
						=> await That(subjects).Have<AssemblyTitleAttribute>().OrHave<AssemblyVersionAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAssembliesHaveMatchingFirstAttribute_ShouldSucceed()
				{
					IEnumerable<Assembly> subjects = new[]
					{
						typeof(AttributeTests).Assembly,
					};

					async Task Act()
						=> await That(subjects).Have<AssemblyTitleAttribute>(attr => attr.Title.Contains("Reflection"))
							.OrHave<TestAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAssembliesHaveMatchingSecondAttribute_ShouldSucceed()
				{
					IEnumerable<Assembly> subjects = new[]
					{
						typeof(AttributeTests).Assembly,
					};

					async Task Act()
						=> await That(subjects).Have<TestAttribute>(attr => attr.Value == "NonExistent")
							.OrHave<AssemblyTitleAttribute>(attr => attr.Title.Contains("Reflection"));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAssembliesHaveNeitherAttribute_ShouldFail()
				{
					IEnumerable<Assembly> subjects = new[]
					{
						typeof(AttributeTests).Assembly,
					};

					async Task Act()
						=> await That(subjects).Have<TestAttribute>().OrHave<BarAttribute>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subjects
						             all have ThatAssemblies.Have.OrHave.AttributeTests.TestAttribute or ThatAssemblies.Have.OrHave.AttributeTests.BarAttribute,
						             but it contained not matching assemblies [
						               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
						             ]
						             """).AsWildcard();
				}

				[Fact]
				public async Task WhenAssembliesHaveSecondAttribute_ShouldSucceed()
				{
					IEnumerable<Assembly> subjects = new[]
					{
						typeof(AttributeTests).Assembly,
					};

					async Task Act()
						=> await That(subjects).Have<TestAttribute>().OrHave<AssemblyTitleAttribute>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithPredicate_WhenAssembliesHaveNotMatchingAttribute_ShouldFail()
				{
					IEnumerable<Assembly> subjects = new[]
					{
						typeof(AttributeTests).Assembly,
					};

					async Task Act()
						=> await That(subjects).Have<AssemblyTitleAttribute>(attr => attr.Title == "NonExistent")
							.OrHave<TestAttribute>(attr => attr.Value == "NonExistent");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subjects
						             all have AssemblyTitleAttribute matching attr => attr.Title == "NonExistent" or ThatAssemblies.Have.OrHave.AttributeTests.TestAttribute matching attr => attr.Value == "NonExistent",
						             but it contained not matching assemblies [
						               aweXpect.Reflection.Tests, Version=*, Culture=neutral, PublicKeyToken=null
						             ]
						             """).AsWildcard();
				}

				[AttributeUsage(AttributeTargets.Assembly)]
				private class TestAttribute : Attribute
				{
					public string Value { get; } = "";
				}

				[AttributeUsage(AttributeTargets.Assembly)]
				private class BarAttribute : Attribute;
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAssembliesDoNotHaveAttribute_ShouldSucceed()
			{
				IEnumerable<Assembly> subjects = new[]
				{
					Assembly.GetExecutingAssembly(),
				};

				async Task Act()
					=> await That(subjects).DoesNotComplyWith(they => they.Have<TestAttribute>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssembliesDoNotHaveMatchingAttribute_ShouldSucceed()
			{
				IEnumerable<Assembly> subjects = new[]
				{
					Assembly.GetExecutingAssembly(),
				};

				async Task Act()
					=> await That(subjects).DoesNotComplyWith(they
						=> they.Have<AssemblyTitleAttribute>(attr => attr.Title == "NonExistent"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAssembliesHaveAttribute_ShouldFail()
			{
				IEnumerable<Assembly> subjects = new[]
				{
					Assembly.GetExecutingAssembly(),
				};

				async Task Act()
					=> await That(subjects).DoesNotComplyWith(they => they.Have<AssemblyTitleAttribute>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subjects
					             not all have AssemblyTitleAttribute,
					             but it only contained matching assemblies *
					             """).AsWildcard();
			}

			[AttributeUsage(AttributeTargets.Assembly)]
			private class TestAttribute : Attribute;
		}
	}
}
