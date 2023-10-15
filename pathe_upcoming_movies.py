#!/usr/bin/env python3
from argparse import ArgumentParser
from datetime import datetime
from enum import Enum

import requests
from ics import Calendar, Event


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

USER_AGENT = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:109.0) Gecko/20100101 Firefox/118.0"


def get_shows(language: Language):
    result = requests.get(f"https://www.pathe.ch/api/shows", params={
        "language": language.value
    }, headers={
        "User-Agent": USER_AGENT,
        "Accept": "application/json"
    })
    return result.json()["shows"]


def get_show(slug: str, language: Language):
    result = requests.get(f"https://www.pathe.ch/api/show/{slug}", params={
        "language": language.value
    }, headers={
        "User-Agent": USER_AGENT,
        "Accept": "application/json"
    })
    return result.json()


def create_event(show, language: Language):
    slug = show["slug"]
    release_at = show["releaseAt"]["CH_FR"]
    event = Event()
    event.name = show["title"]
    event.description = show["synopsis"]
    event.begin = datetime.fromisoformat(f"{release_at}T06:00:00+02:00")
    event.end = datetime.fromisoformat(f"{release_at}T06:00:00+02:00")
    if language == Language.EN:
        event.url = f"https://www.pathe.ch/en/movies-events/{slug}"
    elif language == Language.FR:
        event.url = f"https://www.pathe.ch/fr/films-evenements/{slug}"
    elif language == Language.DE:
        event.url = f"https://www.pathe.ch/en/filme-events/{slug}"
    return event


if __name__ == '__main__':
    args_parser = ArgumentParser()
    args_parser.add_argument("-l", "--language", help="Language of movie's information", choices=Language.names(), required=True)
    args_parser.add_argument("-o", "--output", help="Path to write the calendar", required=True)
    args = vars(args_parser.parse_args())
    language = Language[args["language"]]
    output = args["output"]
    shows = get_shows(Language.FR)
    calendar = Calendar()
    for show in shows:
        slug = show["slug"]
        show = get_show(slug, Language.FR)
        event = create_event(show, Language.FR)
        calendar.events.add(event)
    with open(output, "w") as f:
        f.write(calendar.serialize())
