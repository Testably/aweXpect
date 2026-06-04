using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssembly
{
	public sealed class HasVersion
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenVersionDoesNotMatch_ShouldFail()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				Version actualVersion = assembly.GetName().Version!;

				async Task Act()
				{
					await That(assembly).HasVersion(version => version.Major < 0);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that assembly
					              has version matching version => version.Major < 0,
					              but it had version {actualVersion}
					              """);
			}

			[Fact]
			public async Task WhenVersionMatches_ShouldSucceed()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(assembly).HasVersion(version => version.Major >= 0);
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenVersionDoesNotMatch_ShouldSucceed()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(assembly).DoesNotComplyWith(it => it.HasVersion(version => version.Major < 0));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenVersionMatches_ShouldFail()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				Version actualVersion = assembly.GetName().Version!;

				async Task Act()
				{
					await That(assembly).DoesNotComplyWith(it => it.HasVersion(version => version.Major >= 0));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that assembly
					              does not have version matching version => version.Major >= 0,
					              but it had version {actualVersion}
					              """);
			}
		}

		public sealed class WithoutVersionTests
		{
			[Fact]
			public async Task WithComponent_WhenVersionIsNull_ShouldFail()
			{
				Assembly assembly = new VersionlessAssembly();

				async Task Act()
				{
					await That(assembly).HasVersion().WithMajor.GreaterThanOrEqualTo(0);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that assembly
					             has major version greater than or equal to 0,
					             but it had no version
					             """);
			}

			[Fact]
			public async Task WithoutComponent_WhenVersionIsNull_ShouldFail()
			{
				Assembly assembly = new VersionlessAssembly();

				async Task Act()
				{
					await That(assembly).HasVersion();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that assembly
					             has a version,
					             but it had no version
					             """);
			}

			[Fact]
			public async Task WithPredicate_WhenVersionIsNull_ShouldFail()
			{
				Assembly assembly = new VersionlessAssembly();

				async Task Act()
				{
					await That(assembly).HasVersion(version => version.Major >= 0);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that assembly
					             has version matching version => version.Major >= 0,
					             but it had version <null>
					             """);
			}

			private sealed class VersionlessAssembly : Assembly
			{
				public override string FullName => nameof(VersionlessAssembly);

				public override AssemblyName GetName() => new("VersionlessAssembly");
			}
		}

		public sealed class ComponentTests
		{
			[Fact]
			public async Task ShouldSupportAllComponents()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;

				Version version = assembly.GetName().Version!;

				async Task Act()
				{
					await That(assembly).HasVersion()
						.WithMajor.EqualTo(version.Major)
						.WithMinor.EqualTo(version.Minor)
						.WithBuild.EqualTo(version.Build)
						.WithRevision.EqualTo(version.Revision);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllComparisonsAreSatisfied_ShouldSucceed()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				int major = assembly.GetName().Version!.Major;

				async Task Act()
				{
					await That(assembly).HasVersion()
						.WithMajor.GreaterThan(major - 1)
						.WithMajor.GreaterThanOrEqualTo(major)
						.WithMajor.LessThan(major + 1)
						.WithMajor.LessThanOrEqualTo(major)
						.WithMajor.EqualTo(major)
						.WithMajor.NotEqualTo(major + 1);
				}

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("GreaterThan", 0, "greater than")]
			[InlineData("GreaterThanOrEqualTo", 1, "greater than or equal to")]
			[InlineData("LessThan", 0, "less than")]
			[InlineData("LessThanOrEqualTo", -1, "less than or equal to")]
			[InlineData("EqualTo", 1, "equal to")]
			[InlineData("NotEqualTo", 0, "not equal to")]
			public async Task WhenComparisonFails_MessageShouldDescribeComparison(
				string comparison, int offset, string wording)
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				int major = assembly.GetName().Version!.Major;
				int expected = major + offset;

				async Task Act()
				{
					await (comparison switch
					{
						"GreaterThan" => That(assembly).HasVersion().WithMajor.GreaterThan(expected),
						"GreaterThanOrEqualTo" => That(assembly).HasVersion().WithMajor.GreaterThanOrEqualTo(expected),
						"LessThan" => That(assembly).HasVersion().WithMajor.LessThan(expected),
						"LessThanOrEqualTo" => That(assembly).HasVersion().WithMajor.LessThanOrEqualTo(expected),
						"EqualTo" => That(assembly).HasVersion().WithMajor.EqualTo(expected),
						_ => That(assembly).HasVersion().WithMajor.NotEqualTo(expected),
					});
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that assembly
					              has major version {wording} {expected},
					              but it had major version {major}
					              """);
			}

			[Theory]
			[InlineData("GreaterThan", 0)]
			[InlineData("GreaterThanOrEqualTo", 1)]
			[InlineData("LessThan", 0)]
			[InlineData("LessThanOrEqualTo", -1)]
			[InlineData("EqualTo", 1)]
			[InlineData("NotEqualTo", 0)]
			public async Task WhenComparisonIsNotSatisfied_ShouldFail(string comparison, int offset)
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					int expected = assembly.GetName().Version!.Major + offset;
					await (comparison switch
					{
						"GreaterThan" => That(assembly).HasVersion().WithMajor.GreaterThan(expected),
						"GreaterThanOrEqualTo" => That(assembly).HasVersion().WithMajor.GreaterThanOrEqualTo(expected),
						"LessThan" => That(assembly).HasVersion().WithMajor.LessThan(expected),
						"LessThanOrEqualTo" => That(assembly).HasVersion().WithMajor.LessThanOrEqualTo(expected),
						"EqualTo" => That(assembly).HasVersion().WithMajor.EqualTo(expected),
						_ => That(assembly).HasVersion().WithMajor.NotEqualTo(expected),
					});
				}

				await That(Act).Throws<XunitException>();
			}

			[Theory]
			[InlineData("GreaterThan", -1)]
			[InlineData("GreaterThanOrEqualTo", 0)]
			[InlineData("LessThan", 1)]
			[InlineData("LessThanOrEqualTo", 0)]
			[InlineData("EqualTo", 0)]
			[InlineData("NotEqualTo", 1)]
			public async Task WhenComparisonIsSatisfied_ShouldSucceed(string comparison, int offset)
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					int expected = assembly.GetName().Version!.Major + offset;
					await (comparison switch
					{
						"GreaterThan" => That(assembly).HasVersion().WithMajor.GreaterThan(expected),
						"GreaterThanOrEqualTo" => That(assembly).HasVersion().WithMajor.GreaterThanOrEqualTo(expected),
						"LessThan" => That(assembly).HasVersion().WithMajor.LessThan(expected),
						"LessThanOrEqualTo" => That(assembly).HasVersion().WithMajor.LessThanOrEqualTo(expected),
						"EqualTo" => That(assembly).HasVersion().WithMajor.EqualTo(expected),
						_ => That(assembly).HasVersion().WithMajor.NotEqualTo(expected),
					});
				}

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("major")]
			[InlineData("minor")]
			[InlineData("build")]
			[InlineData("revision")]
			public async Task WhenComponentDoesNotMatch_MessageShouldNameComponent(string name)
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				Version version = assembly.GetName().Version!;
				int actual = name switch
				{
					"major" => version.Major,
					"minor" => version.Minor,
					"build" => version.Build,
					_ => version.Revision,
				};

				async Task Act()
				{
					await (name switch
					{
						"major" => That(assembly).HasVersion().WithMajor.GreaterThan(actual),
						"minor" => That(assembly).HasVersion().WithMinor.GreaterThan(actual),
						"build" => That(assembly).HasVersion().WithBuild.GreaterThan(actual),
						_ => That(assembly).HasVersion().WithRevision.GreaterThan(actual),
					});
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that assembly
					              has {name} version greater than {actual},
					              but it had {name} version {actual}
					              """);
			}

			[Fact]
			public async Task WhenComponentDoesNotMatch_ShouldFail()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				int major = assembly.GetName().Version!.Major;

				async Task Act()
				{
					await That(assembly).HasVersion().WithMajor.LessThan(0);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that assembly
					              has major version less than 0,
					              but it had major version {major}
					              """);
			}

			[Fact]
			public async Task WhenComponentMatches_ShouldSucceed()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(assembly).HasVersion().WithMajor.GreaterThanOrEqualTo(0);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMultipleComponentsAreCombined_ShouldReportFailingComponent()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				Version version = assembly.GetName().Version!;

				async Task Act()
				{
					await That(assembly).HasVersion()
						.WithMajor.EqualTo(version.Major)
						.WithMinor.GreaterThan(version.Minor);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that assembly
					              has major version equal to {version.Major} and minor version greater than {version.Minor},
					              but it had minor version {version.Minor}
					              """);
			}

			[Fact]
			public async Task WithoutComponent_ShouldVerifyThatVersionExists()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(assembly).HasVersion();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedComponentTests
		{
			[Fact]
			public async Task WhenComponentDoesNotMatch_ShouldSucceed()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;

				async Task Act()
				{
					await That(assembly).DoesNotComplyWith(it => it.HasVersion().WithMajor.LessThan(0));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenComponentMatches_ShouldFail()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				Version version = assembly.GetName().Version!;

				async Task Act()
				{
					await That(assembly).DoesNotComplyWith(it => it.HasVersion().WithMajor.GreaterThanOrEqualTo(0));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that assembly
					              does not have major version greater than or equal to 0,
					              but it had version {version}
					              """);
			}

			[Fact]
			public async Task WhenVersionExists_ShouldFail()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				Version version = assembly.GetName().Version!;

				async Task Act()
				{
					await That(assembly).DoesNotComplyWith(it => it.HasVersion());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that assembly
					              does not have a version,
					              but it had version {version}
					              """);
			}
		}
	}
}
