# aweXpect.Snapshots

[![Nuget](https://img.shields.io/nuget/v/aweXpect.Snapshots)](https://www.nuget.org/packages/aweXpect.Snapshots)
[![Build](https://github.com/Testably/aweXpect.Snapshots/actions/workflows/build.yml/badge.svg)](https://github.com/Testably/aweXpect.Snapshots/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Snapshots&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Testably_aweXpect.Snapshots)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Testably_aweXpect.Snapshots&metric=coverage)](https://sonarcloud.io/summary/overall?id=Testably_aweXpect.Snapshots)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FTestably%2FaweXpect.Snapshots%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/Testably/aweXpect.Snapshots/main)

Snapshot testing for [aweXpect](https://github.com/Testably/aweXpect).

## At a glance

```csharp
// compare a value with the snapshot that is stored next to the test
await Expect.That(order).MatchesSnapshot();
```

The snapshot is stored as `<Class>.<Method>.snapshot.txt` next to the test file. A missing or different snapshot fails
the test and writes the actual value to `<Class>.<Method>.snapshot.pending.txt`.

## Overview

| Expectation       | Subject  | Summary                                      |
|-------------------|----------|----------------------------------------------|
| `MatchesSnapshot` | `object` | matches the snapshot stored next to the test |

## Documentation

The full documentation is available at
[docs.testably.org](https://docs.testably.org/aweXpect/extensions/aweXpect.Snapshots/).
