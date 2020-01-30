<h3 align="center">Pathe - UpcomingMovies</h3>
<p align="center">
  Follow upcoming movies in Pathé cinemas in Switzerland directly in your calendar.
  <br>
  <a href="https://github.com/baptistecdr/PatheUpcomingMovies/issues/new">Report bug</a>
  ·
  <a href="https://github.com/baptistecdr/PatheUpcomingMovies/issues/new">Request feature</a>
</p>
<p align="center">
  <img src="https://i.snipboard.io/Ep1yul.jpg" alt="Calendar example"/>
</p>

## Table of contents

- [Quick start](#quick-start)
- [How to build](#how-to-build)
- [Bugs and feature requests](#bugs-and-feature-requests)
- [Contributing](#contributing)
- [Contibutors](#contributors)
- [Thanks to](#thanks-to)

## Quick start
- Install [.NET Core 3 Runtime](https://dotnet.microsoft.com/download)
- Download latest release
- Run with `dotnet PatheUpcomingMovies.dll`
    - Optionally, you can create a cron job to automatically update the creation of the file.
    - If you want to have trailers, put bypass_vimeo.php on your server and pass -t "http(s)://yourserver.com/bypass_vimeo.php"
- Import .ics file in your calendar app.
    - Optionally, you can put the ics file on your server and subscribe to it.

### Arguments
| Parameters        | Shortcuts | Descriptions  | Mandatory |
|-------------------|-----------|---------------|-----------|
| --city            | -c        | (Default: Geneva) City of the cinema. Valid values: Basel, Bern, Geneva, LausanneFlo, LausanneGal, Lucern, Spreitenbach, Zurich  | No |
| --language        | -l        | (Default: Fr) Language of the informations. Valid values: De, En, Fr | No |
| --trailer-language | -t       | (Default: Ov) Language of trailers. Valid Values: De, Ov, Fr | No |
| --vimeo-bypass-url | -v       | (Default: ) Url to bypass Vimeo. Example: https://example.com/vimeo\_bypass.php | No |
| --output          | -o        | (Default: ./PatheUpcomingMovies.ics) Path to export ics file.  | No |
| --help            | -h        | Display this help screen.   | No |


## How to build

- Install [Visual Studio](https://visualstudio.microsoft.com/downloads)
- Install [.NET Core 3 SDK](https://dotnet.microsoft.com/download)
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
- [Command Line Parser](https://github.com/commandlineparser/commandline)
