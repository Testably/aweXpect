# Deterministic builds

A deterministic build, e.g. with `ContinuousIntegrationBuild=true` on a build server, replaces the path of the
repository with `/_/` in the compiled code, so the path of the test file, e.g. `/_/Tests/AlbumTests.cs`, no longer
leads to its snapshots. The package records the real path of the repository in the test assembly when it is built, so
the snapshots are found in such a build as well, without any configuration.

A snapshot is identified by the path of its test file relative to the repository, e.g. `Tests/AlbumTests.cs`, which
is the same on every machine and in every build.

This needs two things, which a usual setup provides:

- The test project references the `aweXpect.Snapshots` package, directly or through another project, as the build
  targets that record the path come with the package.
- The repository provides source control information, which the .NET SDK does for a git repository.

Without them, a snapshot is identified by the full path of its test file, and in a deterministic build the expectation
fails with:

```text title="Failure message"
Expected that description
matches the snapshot "/_/Tests/AlbumTests.Describes.awexpect-snapshot.txt",
but the snapshot could not be read: The path "/_/Tests/AlbumTests.cs" comes from a deterministic build, but the test assembly records no source root for it, which the build targets of aweXpect.Snapshots add.
```
