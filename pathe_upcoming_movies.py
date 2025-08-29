#!/usr/bin/env python3
import datetime
from argparse import ArgumentParser, ArgumentTypeError
from datetime import datetime
from enum import Enum
from zoneinfo import ZoneInfo, ZoneInfoNotFoundError

import requests
from ics import Calendar, Event

USER_AGENT = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:137.0) Gecko/20100101 Firefox/137.0"
DEFAULT_BEGIN_TIME = "04:00:00"
DEFAULT_END_TIME = "04:00:00"
DEFAULT_TIMEZONE = "UTC"


class Language(Enum):
    EN = "en"
    FR = "fr"
    DE = "de"

    @staticmethod
    def names():
        return [l.name for l in Language]

    @staticmethod
    def values():
        return [l.value for l in Language]

class Country(Enum):
    CH = "ch"
    FR = "fr"

    @staticmethod
    def names():
        return [c.name for c in Country]

    @staticmethod
    def values():
        return [c.value for c in Country]

def get_shows(country: Country,language: Language):
    result = requests.get(f"https://www.pathe.{country.value}/api/shows", params={
        "language": language.value
    }, headers={
        "User-Agent": USER_AGENT,
        "Accept": "application/json"
    })
    return result.json()["shows"]


def get_show(country: Country, slug: str, language: Language):
    result = requests.get(f"https://www.pathe.{country.value}/api/show/{slug}", params={
        "language": language.value
    }, headers={
        "User-Agent": USER_AGENT,
        "Accept": "application/json"
    })
    return result.json()


def create_event(show, country: Country, language: Language, begin_time: datetime.time, end_time: datetime.time, timezone: ZoneInfo):
    slug = show["slug"]

    # Release date key format: CH_FR, FR_FR, etc.
    release_key = f"{country.name}_FR"
    release_at = datetime.strptime(show["releaseAt"][release_key], "%Y-%m-%d").date()

    event = Event()
    event.name = show["title"]
    event.description = show["synopsis"]
    event.begin = datetime.combine(release_at, begin_time, timezone)
    event.end = datetime.combine(release_at, end_time, timezone)

    # URL pattern varies by country + language
    if language == Language.EN:
        event.url = f"https://www.pathe.{country.value}/en/movies-events/{slug}"
    elif language == Language.FR:
        event.url = f"https://www.pathe.{country.value}/fr/films-evenements/{slug}"
    elif language == Language.DE:
        event.url = f"https://www.pathe.{country.value}/de/filme-events/{slug}"
    return event


def validate_time(time):
    try:
        return datetime.strptime(time, "%H:%M:%S").time()
    except ValueError:
        raise ArgumentTypeError(f"invalid time: '{time}' (format %H:%M:%S)")


def validate_timezone(timezone):
    try:
        return ZoneInfo(timezone)
    except ZoneInfoNotFoundError:
        raise ArgumentTypeError(f"invalid timezone: '{timezone}'")


if __name__ == '__main__':
    args_parser = ArgumentParser()
    args_parser.add_argument("-c", "--country", help="Country code (CH or FR)", choices=Country.names(), required=True)
    args_parser.add_argument("-l", "--language", help="Language of movie's information", choices=Language.names(),
                             required=True)
    args_parser.add_argument("-o", "--output", help="Path to write the calendar", required=True)
    args_parser.add_argument("-bt", "--begin-time", type=validate_time,
                             help=f"Begin time for each event. Default: {DEFAULT_BEGIN_TIME}", required=False,
                             default=DEFAULT_BEGIN_TIME)
    args_parser.add_argument("-et", "--end-time", type=validate_time,
                             help=f"End time for each event. Default: {DEFAULT_END_TIME}", required=False,
                             default=DEFAULT_END_TIME)
    args_parser.add_argument("-t", "--timezone", type=validate_timezone,
                             help=f"Timezone for each event. Default: {DEFAULT_TIMEZONE}", required=False,
                             default=DEFAULT_TIMEZONE)
    args = vars(args_parser.parse_args())

    country = Country[args["country"]]
    language = Language[args["language"]]
    output = args["output"]
    begin_time = args["begin_time"]
    end_time = args["end_time"]
    timezone = args["timezone"]

    if country == Country.FR and language == Language.DE:
        raise ArgumentTypeError("German language is not supported for country FR")

    if begin_time >= end_time:
        raise ArgumentTypeError("begin-time must be earlier than end-time")

    calendar = Calendar()
    shows = get_shows(country, language)
    for show in shows:
        slug = show["slug"]
        show = get_show(country, slug, language)
        event = create_event(show, country, language, begin_time, end_time, timezone)
        calendar.events.add(event)

    with open(output, "w") as f:
        f.write(calendar.serialize())
