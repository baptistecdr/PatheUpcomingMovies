<h3 align="center">PatheUpcomingMovies</h3>
<p align="center">
    Subscribe to a calendar (ics) of new upcoming movies in <a href="https://www.pathe.ch/">Pathé</a> (Suisse or France).
    <br>
    <a href="https://github.com/baptistecdr/PatheUpcomingMovies/issues/new">Report bug</a>
    ·
    <a href="https://github.com/baptistecdr/PatheUpcomingMovies/issues/new">Request feature</a>
</p>

<div align="center">

[![Update calendars](https://github.com/baptistecdr/PatheUpcomingMovies/actions/workflows/python.yml/badge.svg)](https://github.com/baptistecdr/PatheUpcomingMovies/actions/workflows/python.yml)
[![Lint](https://github.com/baptistecdr/PatheUpcomingMovies/actions/workflows/lint.yml/badge.svg)](https://github.com/baptistecdr/PatheUpcomingMovies/actions/workflows/lint.yml)
[![Tests](https://github.com/baptistecdr/PatheUpcomingMovies/actions/workflows/test.yml/badge.svg)](https://github.com/baptistecdr/PatheUpcomingMovies/actions/workflows/test.yml)

</div>

This project creates calendars (ics) of new upcoming movies in Pathé (Suisse or France). The calendars are
regenerated every Monday by a [GitHub Actions workflow](.github/workflows/python.yml).

## Quick start

Subscribe to one of the calendars available for Suisse (CH):

* [PatheUpcomingMovies_EN.ics](https://raw.githubusercontent.com/baptistecdr/PatheUpcomingMovies/main/calendars/PatheUpcomingMovies_EN.ics)
  * Movie's details in English
* [PatheUpcomingMovies_FR.ics](https://raw.githubusercontent.com/baptistecdr/PatheUpcomingMovies/main/calendars/PatheUpcomingMovies_FR.ics)
  * Movie's details in French
* [PatheUpcomingMovies_DE.ics](https://raw.githubusercontent.com/baptistecdr/PatheUpcomingMovies/main/calendars/PatheUpcomingMovies_DE.ics)
  * Movie's details in Deutsch

Or to one of the calendars available for France (FR):

* [PatheUpcomingMovies_EN_FR.ics](https://raw.githubusercontent.com/baptistecdr/PatheUpcomingMovies/main/calendars/PatheUpcomingMovies_EN_FR.ics)
  * Movie's details in English
* [PatheUpcomingMovies_FR_FR.ics](https://raw.githubusercontent.com/baptistecdr/PatheUpcomingMovies/main/calendars/PatheUpcomingMovies_FR_FR.ics)
  * Movie's details in French

## How to build

- Install [Python](https://www.python.org/downloads/) >= 3.10
- Clone the project
- Run `source setup.sh` to create a virtual environment and install the requirements

## Usage

```bash
source setup.sh
python3 pathe_upcoming_movies.py --country CH --language EN --output PatheUpcomingMovies_EN.ics
```

| Option                | Description                                        | Default    |
|-----------------------|----------------------------------------------------|------------|
| `-c`, `--country`     | Country code (`CH` or `FR`)                        | (required) |
| `-l`, `--language`    | Language of movie's information (`EN`, `FR`, `DE`) | (required) |
| `-o`, `--output`      | Path to write the calendar                         | (required) |
| `-bt`, `--begin-time` | Begin time for each event (`%H:%M:%S`)             | `04:00:00` |
| `-et`, `--end-time`   | End time for each event (`%H:%M:%S`)               | `04:00:00` |
| `-t`, `--timezone`    | Timezone for each event                            | `UTC`      |

German (`DE`) is only available for Suisse (`CH`).

## Development

- Run `pip install -r requirements-dev.txt` to install the development requirements
- Run `pytest` to run the tests
- Run `black .` to format the code ([black](https://github.com/psf/black) must be installed)

## Bugs and feature requests

Have a bug or a feature request? Please first search for existing and closed issues. If your problem or idea is not
addressed yet, [please open a new issue](https://github.com/baptistecdr/PatheUpcomingMovies/issues/new).

## Contributing

Contributions are welcome!
