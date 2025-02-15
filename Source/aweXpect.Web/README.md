# aweXpect.Web
[![Nuget](https://img.shields.io/nuget/v/aweXpect.Web)](https://www.nuget.org/packages/aweXpect.Web) 
[![Build](https://github.com/aweXpect/aweXpect.Web/actions/workflows/build.yml/badge.svg)](https://github.com/aweXpect/aweXpect.Web/actions/workflows/build.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=aweXpect_aweXpect.Web&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=aweXpect_aweXpect.Web)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=aweXpect_aweXpect.Web&metric=coverage)](https://sonarcloud.io/summary/new_code?id=aweXpect_aweXpect.Web)

Template for extension projects for [aweXpect](https://github.com/aweXpect/aweXpect).  

## Steps after creating a new project from this Template:

- Replace "Web" with your suffix both in file names and in contents.
- Enable Sonarcloud analysis
  - Create the project at [sonarcloud](https://sonarcloud.io/projects/create)
  - Add the `SONAR_TOKEN` secret as repository secret
- Create a "production" environment and add the `NUGET_API_KEY` secret
- Adapt the copyright and project information in Source/Directory.Build.props
- Adapt the README.md
