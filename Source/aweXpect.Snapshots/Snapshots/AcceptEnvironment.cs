using System;
using System.Linq;

namespace aweXpect.Snapshots;

/// <summary>
///     Decides whether snapshots are accepted when neither the expectation nor the customization decides it.
/// </summary>
/// <remarks>
///     Snapshots are accepted when <c>AWEXPECT_ACCEPT_SNAPSHOTS</c> is set and no build server is detected, so that a
///     variable that was forgotten in a pipeline cannot change the snapshots there. Most build servers set <c>CI</c>; the
///     others are Azure Pipelines (<c>TF_BUILD</c>), Jenkins (<c>JENKINS_URL</c>) and TeamCity
///     (<c>TEAMCITY_VERSION</c>). A variable counts as set unless it is empty, <c>0</c> or <c>false</c>, so that
///     <c>CI=false</c> or <c>AWEXPECT_ACCEPT_SNAPSHOTS=0</c> mean what they say.
/// </remarks>
internal static class AcceptEnvironment
{
	private const string AcceptVariable = "AWEXPECT_ACCEPT_SNAPSHOTS";

	private static readonly string[] BuildServerVariables = ["CI", "TF_BUILD", "JENKINS_URL", "TEAMCITY_VERSION",];

	public static bool IsAccepting()
		=> IsSet(AcceptVariable) && !BuildServerVariables.Any(IsSet);

	private static bool IsSet(string variable)
		=> Environment.GetEnvironmentVariable(variable) is { Length: > 0, } value &&
		   value != "0" &&
		   !value.Equals("false", StringComparison.OrdinalIgnoreCase);
}
