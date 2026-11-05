[assembly: System.Reflection.AssemblyMetadata("IsAotCompatible", "True")]
[assembly: System.Reflection.AssemblyMetadata("IsTrimmable", "True")]
[assembly: System.Reflection.AssemblyMetadata("RepositoryUrl", "https://github.com/Testably/aweXpect.git")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("aweXpect.Snapshots.DiffEngine.Tests, PublicKey=002400000480000094000000060200000024000052534131000400000100010091e3b299f2a31c904fbc5c2150944568b101ba409f5ee4cd95561c0c8cf50c1960d74f682c7f462f792ab37fe315ece00775c5bdda68c06d3abb67c1d531bf8c40d85d3d36ecf19fb05cc890ca7a5f39904e04ff754a55463236c5884f41083fae1ea626eb1f9b8c5c33f7bada7e650bed6ced9f0a75413d7e919f94f804efc9")]
[assembly: System.Runtime.Versioning.TargetFramework(".NETCoreApp,Version=v10.0", FrameworkDisplayName = ".NET 10.0")]
namespace aweXpect.Snapshots.DiffEngine
{
    public sealed class DiffEngineReporter : aweXpect.Snapshots.SnapshotReporter
    {
        public DiffEngineReporter() { }
        public override System.Threading.Tasks.ValueTask OnMatchAsync(aweXpect.Snapshots.SnapshotMatch match, System.Threading.CancellationToken cancellationToken) { }
        public override System.Threading.Tasks.ValueTask OnMismatchAsync(aweXpect.Snapshots.SnapshotMismatch mismatch, System.Threading.CancellationToken cancellationToken) { }
        public static void Register() { }
    }
}

