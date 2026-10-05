using System.Linq;
using System.Text.RegularExpressions;

namespace aweXpect.Generators.Tests;

public sealed partial class TypeMetadataGeneratorTests
{
	public sealed partial class ComparisonTypeTests
	{
		private const string Types = """
		                             namespace Models
		                             {
		                             	public class Booking
		                             	{
		                             		public System.DateOnly Day { get; set; }
		                             		public System.Net.IPAddress Origin { get; set; } = System.Net.IPAddress.None;
		                             	}

		                             	public class CustomAddress() : System.Net.IPAddress(1L)
		                             	{
		                             		public Other Extra { get; set; } = new();
		                             	}

		                             	public class CustomEncoding : System.Text.UTF8Encoding
		                             	{
		                             		public Other Extra { get; set; } = new();
		                             	}

		                             	public class CustomRegex() : System.Text.RegularExpressions.Regex("a")
		                             	{
		                             		public Other Extra { get; set; } = new();
		                             	}

		                             	public class Other
		                             	{
		                             		public int Count { get; set; }
		                             	}
		                             }

		                             namespace Lookalikes
		                             {
		                             	public class DateOnly
		                             	{
		                             		public int Year { get; set; }
		                             	}
		                             }
		                             """;

		[Theory]
		[InlineData("System.DateOnly")]
		[InlineData("System.TimeOnly")]
		[InlineData("System.Net.IPAddress")]
		[InlineData("Models.CustomAddress")]
		[InlineData("System.Text.Encoding")]
		[InlineData("System.Text.UTF8Encoding")]
		[InlineData("Models.CustomEncoding")]
		[InlineData("System.Text.RegularExpressions.Regex")]
		[InlineData("Models.CustomRegex")]
		[InlineData("System.Text.Json.JsonElement")]
		[InlineData("System.Text.Json.Nodes.JsonNode")]
		[InlineData("System.Text.Json.Nodes.JsonObject")]
		[InlineData("System.Text.Json.Nodes.JsonArray")]
		[InlineData("System.Text.Json.Nodes.JsonValue")]
		public async Task WhenGenerateMetadataAttributeNamesATypeComparedByValue_ShouldReportADiagnostic(string type)
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
				[Types, $"[assembly: aweXpect.Core.Metadata.GenerateMetadata(typeof({type}))]",]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated).IsEmpty();
			await That(result.GeneratorDiagnostics).HasSingle().Which
				.Satisfies(x => x.Id == "aweXpect2001" && x.GetMessage().Contains($"'{type}'") &&
				                x.GetMessage().Contains("because it is compared by value"))
				.Because("the comparison never reads the members of the type, so naming it has no effect");
		}

		[Fact]
		public async Task WhenMemberHasTheNameOfATypeComparedByValue_ShouldRegisterItsMembers()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[
				Types,
				Call("Expect.That(new { Day = new Lookalikes.DateOnly() }).IsEquivalentTo(new { Day = new Lookalikes.DateOnly() });"),
			]);

			await That(result.Errors).IsEmpty();
			await That(result.Generated)
				.Contains("RegisterProperty<global::Lookalikes.DateOnly, int>(\"Year\", o => o.Year);")
				.Because("only the type in its own namespace is compared by value");
		}

		[Theory]
		[InlineData("System.DateOnly")]
		[InlineData("System.TimeOnly")]
		[InlineData("System.Net.IPAddress")]
		[InlineData("Models.CustomAddress")]
		[InlineData("System.Text.Encoding")]
		[InlineData("System.Text.UTF8Encoding")]
		[InlineData("Models.CustomEncoding")]
		[InlineData("System.Text.RegularExpressions.Regex")]
		[InlineData("Models.CustomRegex")]
		[InlineData("System.Text.Json.JsonElement")]
		[InlineData("System.Text.Json.Nodes.JsonNode")]
		[InlineData("System.Text.Json.Nodes.JsonObject")]
		[InlineData("System.Text.Json.Nodes.JsonArray")]
		[InlineData("System.Text.Json.Nodes.JsonValue")]
		public async Task WhenMemberIsComparedByValue_ShouldOnlyRegisterTheOwner(string type)
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
			[
				Types,
				$"namespace Models {{ public class Owner {{ public {type} Value {{ get; set; }} = default!; }} }}",
				Call("Expect.That(new Models.Owner()).IsEquivalentTo(new Models.Owner());"),
			]);

			await That(result.Errors).IsEmpty();
			await That(Registered().Matches(result.Generated).Select(x => x.Groups[1].Value)).IsEqualTo(["global::Models.Owner",])
				.Because("the comparison never reads the members of the type, so registering them or the types they lead to only roots them in a trimmed application");
			await That(result.Generated)
				.Contains($"RegisterProperty<global::Models.Owner, global::{type}>(\"Value\", o => o.Value);");
		}

		[Fact]
		public async Task WhenMembersAreADateAndAnAddress_ShouldOnlyRegisterTheOwner()
		{
			GeneratorRunner.GeneratorResult result = GeneratorRunner.Run(
				[Types, Call("Expect.That(new Models.Booking()).IsEquivalentTo(new Models.Booking());"),]);

			await That(result.Errors).IsEmpty();
			await That(Registered().Matches(result.Generated).Select(x => x.Groups[1].Value)).IsEqualTo(["global::Models.Booking",]);
			await That(result.Generated).DoesNotContain("\"DayOfWeek\"").And.DoesNotContain("\"AddressFamily\"")
				.Because("a date and an address are compared by value");
		}

		/// <summary>
		///     The key that precedes every registration in the generated source.
		/// </summary>
		[GeneratedRegex("^\t+// (?!ILC |<auto-generated)(.*?)\r?$", RegexOptions.Multiline)]
		private static partial Regex Registered();
	}
}
