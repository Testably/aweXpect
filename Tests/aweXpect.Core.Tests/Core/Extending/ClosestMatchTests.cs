using System.Collections.Generic;
using System.Linq;
using System.Text;
using aweXpect.Core.Extending;

namespace aweXpect.Core.Tests.Core.Extending;

public class ClosestMatchTests
{
	public sealed class AppendHint
	{
		[Test]
		public async Task WhenACandidateIsClose_ShouldAppendTheSuggestion()
		{
			StringBuilder sb = new("it did not contain key \"Usrname\"");

			ClosestMatch.AppendHint(sb, "Usrname", ["Password", "Username",]);

			await That(sb.ToString()).IsEqualTo("it did not contain key \"Usrname\" (did you mean \"Username\"?)");
		}

		[Test]
		public async Task WhenNoCandidateIsClose_ShouldAppendNothing()
		{
			StringBuilder sb = new("it did not contain key \"Usrname\"");

			ClosestMatch.AppendHint(sb, "Usrname", ["Password",]);

			await That(sb.ToString()).IsEqualTo("it did not contain key \"Usrname\"");
		}

		[Test]
		public async Task WhenStringBuilderIsNull_ShouldThrowArgumentNullException()
		{
			void Act() => ClosestMatch.AppendHint(null!, "Usrname", ["Username",]);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("stringBuilder").And
				.WithMessage("The * cannot be null.*").AsWildcard();
		}
	}

	public sealed class Find
	{
		[Test]
		public async Task WhenCandidatesAreNull_ShouldThrowArgumentNullException()
		{
			void Act() => ClosestMatch.Find("Name", null!);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("candidates").And
				.WithMessage("The * cannot be null.*").AsWildcard();
		}

		[Test]
		public async Task WhenCandidatesContainNull_ShouldIgnoreIt()
		{
			string? result = ClosestMatch.Find("Nmae", [null!, "Name",]);

			await That(result).IsEqualTo("Name");
		}

		[Test]
		public async Task WhenOneCandidateDiffersOnlyInCasing_ShouldPreferItOverCloserEdits()
		{
			string? result = ClosestMatch.Find("username", ["usernam", "Username",]);

			await That(result).IsEqualTo("Username");
		}

		[Test]
		public async Task WhenTheCandidateIsBeyondTheLimit_ShouldReturnNull()
		{
			IEnumerable<string> candidates = Enumerable.Range(0, 1000).Select(i => $"Other{i}").Append("Name");

			string? result = ClosestMatch.Find("Nmae", candidates);

			await That(result).IsNull()
				.Because("only the first 1000 candidates are compared");
		}

		[Test]
		public async Task WhenTheCandidateIsWithinTheLimit_ShouldFindIt()
		{
			IEnumerable<string> candidates = Enumerable.Range(0, 999).Select(i => $"Other{i}").Append("Name");

			string? result = ClosestMatch.Find("Nmae", candidates);

			await That(result).IsEqualTo("Name");
		}

		[Test]
		public async Task WhenTheSameCandidateOccursSeveralTimes_ShouldReturnIt()
		{
			string? result = ClosestMatch.Find("Nmae", ["Name", "Name",]);

			await That(result).IsEqualTo("Name")
				.Because("a repeated candidate is not a second suggestion");
		}

		[Test]
		public async Task WhenTwoCandidatesAreEquallyClose_ShouldReturnNull()
		{
			string? result = ClosestMatch.Find("Nam", ["Name", "Nama",]);

			await That(result).IsNull()
				.Because("a wrong suggestion misleads more than none");
		}

		[Test]
		public async Task WhenTwoCandidatesDifferOnlyInCasing_ShouldReturnNull()
		{
			string? result = ClosestMatch.Find("name", ["Name", "NAME",]);

			await That(result).IsNull();
		}

		[Test]
		public async Task WhenValueIsNull_ShouldThrowArgumentNullException()
		{
			void Act() => ClosestMatch.Find(null!, ["Name",]);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("value").And
				.WithMessage("The * cannot be null.*").AsWildcard();
		}

		[Test]
		public async Task WhenValueIsOneOfTheCandidates_ShouldReturnNull()
		{
			string? result = ClosestMatch.Find("Name", ["Nam", "Name",]);

			await That(result).IsNull()
				.Because("a value that is found has no closer match to suggest");
		}

		[Test]
		[Arguments("Nmae", "Name")]
		[Arguments("Usrname", "Username")]
		[Arguments("Usernmae", "Username")]
		[Arguments("PlayCuont", "PlayCount")]
		[Arguments("Title", "Titel")]
		public async Task WithinOneEditPerThreeCharacters_ShouldFindTheCandidate(string value, string expected)
		{
			string? result = ClosestMatch.Find(value, ["Id", expected, "Duration",]);

			await That(result).IsEqualTo(expected);
		}

		[Test]
		[Arguments("Ab", "Id")]
		[Arguments("Nmea", "Name")]
		[Arguments("Duratn", "Durationxx")]
		public async Task WithMoreThanOneEditPerThreeCharacters_ShouldReturnNull(string value, string candidate)
		{
			string? result = ClosestMatch.Find(value, [candidate,]);

			await That(result).IsNull();
		}
	}
}
