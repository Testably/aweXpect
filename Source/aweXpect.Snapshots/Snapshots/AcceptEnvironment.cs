using System;
using System.Linq;
using aweXpect.Core;

namespace aweXpect.Snapshots;

/// <summary>
///     Decides which snapshots are accepted when neither the expectation nor the customization decides it.
/// </summary>
/// <remarks>
///     The level is read from <c>AWEXPECT_ACCEPT_SNAPSHOTS</c> (<c>none</c>, <c>new</c> or <c>all</c>, ignoring case and
///     surrounding whitespace; <c>1</c> and <c>true</c> mean <c>all</c>, <c>0</c> and <c>false</c> mean <c>none</c>),
///     unless a build server is detected, so that a variable that was forgotten in a pipeline cannot change the snapshots
///     there. Most build servers set <c>CI</c>; the others are Azure Pipelines (<c>TF_BUILD</c>), Jenkins
///     (<c>JENKINS_URL</c>) and TeamCity (<c>TEAMCITY_VERSION</c>). A build server variable counts as set unless it is
///     empty, <c>0</c> or <c>false</c>, so that <c>CI=false</c> means what it says.<br />
///     Any other value of <c>AWEXPECT_ACCEPT_SNAPSHOTS</c> throws, so that a typo cannot silently accept nothing.
/// </remarks>
internal static class AcceptEnvironment
{
	private const string AcceptVariable = "AWEXPECT_ACCEPT_SNAPSHOTS";

	private static readonly string[] BuildServerVariables = ["CI", "TF_BUILD", "JENKINS_URL", "TEAMCITY_VERSION",];

	public static SnapshotAcceptance GetAcceptance()
		=> IsBuildServer() ? SnapshotAcceptance.None : Parse(Environment.GetEnvironmentVariable(AcceptVariable));

	public static bool IsBuildServer()
		=> BuildServerVariables.Any(IsSet);

	private static SnapshotAcceptance Parse(string? value)
		=> value?.Trim().ToUpperInvariant() switch
		{
			null or "" or "0" or "FALSE" or "NONE" => SnapshotAcceptance.None,
			"NEW" => SnapshotAcceptance.New,
			"1" or "TRUE" or "ALL" => SnapshotAcceptance.All,
			_ => throw Tracing.WriteException(new InvalidOperationException(
				$"The environment variable {AcceptVariable} has the invalid value '{value}'. Use 'none', 'new' or 'all' (or '0'/'false' for 'none' and '1'/'true' for 'all')."))
		};

	private static bool IsSet(string variable)
		=> Environment.GetEnvironmentVariable(variable) is { Length: > 0, } value &&
		   value != "0" &&
		   !value.Equals("false", StringComparison.OrdinalIgnoreCase);
}
