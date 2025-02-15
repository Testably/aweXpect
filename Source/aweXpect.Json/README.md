# aweXpect.T6e
[![Nuget](https://img.shields.io/nuget/v/aweXpect.T6e)](https://www.nuget.org/packages/aweXpect.T6e) 
[![Build](https://github.com/aweXpect/aweXpect.T6e/actions/workflows/build.yml/badge.svg)](https://github.com/aweXpect/aweXpect.T6e/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=aweXpect_aweXpect.T6e&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=aweXpect_aweXpect.T6e)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=aweXpect_aweXpect.T6e&metric=coverage)](https://sonarcloud.io/summary/new_code?id=aweXpect_aweXpect.T6e)

Template for extension projects for [aweXpect](https://github.com/aweXpect/aweXpect).  

## Steps after creating a new project from this Template:

- Replace "T6e" with your suffix both in file names and in contents.
- Enable Sonarcloud analysis
  - Create the project at [sonarcloud](https://sonarcloud.io/projects/create)
  - Add the `SONAR_TOKEN` secret as repository secret
- Create a "production" environment and add the `NUGET_API_KEY` secret
- Adapt the copyright and project information in Source/Directory.Build.props
- Adapt the README.md
