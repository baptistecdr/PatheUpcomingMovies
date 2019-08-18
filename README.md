<p align="center">
  <h3 align="center">PatheUpcomingMovies</h3>

  <p align="center">
    Follow upcoming movies in Pathé cinemas in Switzerland directly in your calendar.
    <br>
    <a href="https://github.com/baptistecdr/PatheUpcomingMovies/issues/new">Report bug</a>
    ·
    <a href="https://github.com/baptistecdr/PatheUpcomingMovies/issues/new">Request feature</a>
  </p>
  <img src="https://i.snipboard.io/Ep1yul.jpg"/>
</p>

## Table of contents

- [Quick start](#quick-start)
- [How to build](#how-to-build)
- [Bugs and feature requests](#bugs-and-feature-requests)
- [Contributing](#contributing)
- [Contibutors](#contributors)
- [Thanks to](#thanks-to)

## Quick start
- Install .NET Core 2
- Download latest release
- Run with `dotnet PatheUpcomingMovies.dll`

### Arguments
| Parameters        | Shortcuts | Descriptions  | Mandatory |
|-------------------|-----------|---------------|-----------|
| --city            | -c        | (Default: Geneva) City of the cinema. Valid values: Basel, Bern, Geneva, LausanneFlo, LausanneGal, Lucern, Spreitenbach, Zurich  | No |
| --language        | -l        | Default: Fr) Language of the informations. Valid values: De, En, Fr | No |
| --output          | -o        | (Default: ./PatheUpcomingMovies.ics) Path to export ics file.  | No |
| --help            | -h        | Display this help screen.   | No |


## How to build

- Install [Visual Studio](https://code.visualstudio.com)
- Install [.NET Core 2](https://platformio.org)
- Clone the project
- Open project in Visual Studio

## Bugs and feature requests

Have a bug or a feature request? Please first search for existing and closed issues. If your problem or idea is not addressed yet, [please open a new issue](https://github.com/baptistecdr/PatheUpcomingMovies/issues/new).

## Contributing

Contributions are welcome!

## Contributors

## Thanks to
- [iCal.NET](https://github.com/rianjs/ical.net)
- [Quicktype](https://quicktype.io)