from argparse import ArgumentTypeError
from datetime import time
from unittest.mock import MagicMock, patch
from zoneinfo import ZoneInfo

import pytest

from pathe_upcoming_movies import (
    Country,
    Language,
    create_event,
    get_show,
    get_shows,
    validate_time,
    validate_timezone,
)


def make_show(country: Country, **overrides):
    show = {
        "slug": "some-movie",
        "title": "Some Movie",
        "synopsis": "A great movie synopsis.",
        "releaseAt": {f"{country.name}_FR": "2026-09-10"},
    }
    show.update(overrides)
    return show


class TestValidateTime:
    def test_valid_time(self):
        assert validate_time("14:30:00") == time(14, 30, 0)

    def test_invalid_time(self):
        with pytest.raises(ArgumentTypeError):
            validate_time("not-a-time")


class TestValidateTimezone:
    def test_valid_timezone(self):
        assert validate_timezone("Europe/Zurich") == ZoneInfo("Europe/Zurich")

    def test_invalid_timezone(self):
        with pytest.raises(ArgumentTypeError):
            validate_timezone("Not/ATimezone")


class TestEnums:
    def test_country_names_and_values(self):
        assert Country.names() == ["CH", "FR"]
        assert Country.values() == ["ch", "fr"]

    def test_language_names_and_values(self):
        assert Language.names() == ["EN", "FR", "DE"]
        assert Language.values() == ["en", "fr", "de"]


class TestCreateEvent:
    @pytest.mark.parametrize(
        "country,language,expected_url",
        [
            (Country.CH, Language.EN, "https://www.pathe.ch/en/movies-events/some-movie"),
            (
                Country.CH,
                Language.FR,
                "https://www.pathe.ch/fr/films-evenements/some-movie",
            ),
            (Country.CH, Language.DE, "https://www.pathe.ch/de/filme-events/some-movie"),
            (Country.FR, Language.EN, "https://www.pathe.fr/en/movies-events/some-movie"),
            (
                Country.FR,
                Language.FR,
                "https://www.pathe.fr/fr/films-evenements/some-movie",
            ),
        ],
    )
    def test_url_by_country_and_language(self, country, language, expected_url):
        show = make_show(country)
        event = create_event(
            show, country, language, time(4, 0, 0), time(4, 0, 0), ZoneInfo("UTC")
        )
        assert event["url"] == expected_url

    def test_name_and_description(self):
        show = make_show(Country.CH)
        event = create_event(
            show, Country.CH, Language.EN, time(4, 0, 0), time(4, 0, 0), ZoneInfo("UTC")
        )
        assert event["summary"] == "Some Movie"
        assert event["description"] == "A great movie synopsis."

    def test_uid_is_derived_from_slug_and_country(self):
        show = make_show(Country.CH, slug="some-movie")
        event = create_event(
            show, Country.CH, Language.EN, time(4, 0, 0), time(4, 0, 0), ZoneInfo("UTC")
        )
        assert event["uid"] == "some-movie@pathe.ch"

    def test_begin_and_end_use_release_date_and_times(self):
        show = make_show(Country.CH)
        event = create_event(
            show,
            Country.CH,
            Language.EN,
            time(4, 0, 0),
            time(20, 0, 0),
            ZoneInfo("UTC"),
        )
        dtstart = event["dtstart"].dt
        dtend = event["dtend"].dt
        assert dtstart.year == 2026
        assert dtstart.month == 9
        assert dtstart.day == 10
        assert dtstart.hour == 4
        assert dtend.hour == 20

    def test_release_date_key_uses_fr_suffix(self):
        show = make_show(Country.FR)
        event = create_event(
            show,
            Country.FR,
            Language.EN,
            time(4, 0, 0),
            time(4, 0, 0),
            ZoneInfo("UTC"),
        )
        assert event["dtstart"].dt.day == 10


class TestGetShows:
    @patch("pathe_upcoming_movies.requests.get")
    def test_returns_shows_list(self, mock_get):
        mock_response = MagicMock()
        mock_response.json.return_value = {
            "shows": [{"slug": "movie-a"}, {"slug": "movie-b"}]
        }
        mock_get.return_value = mock_response

        shows = get_shows(Country.CH, Language.EN)

        assert shows == [{"slug": "movie-a"}, {"slug": "movie-b"}]
        mock_get.assert_called_once()
        called_url = mock_get.call_args.args[0]
        called_kwargs = mock_get.call_args.kwargs
        assert called_url == "https://www.pathe.ch/api/shows"
        assert called_kwargs["params"] == {"language": "en"}
        assert called_kwargs["headers"]["Accept"] == "application/json"


class TestGetShow:
    @patch("pathe_upcoming_movies.requests.get")
    def test_returns_show_json(self, mock_get):
        mock_response = MagicMock()
        mock_response.json.return_value = {"slug": "movie-a", "title": "Movie A"}
        mock_get.return_value = mock_response

        show = get_show(Country.FR, "movie-a", Language.FR)

        assert show == {"slug": "movie-a", "title": "Movie A"}
        called_url = mock_get.call_args.args[0]
        called_kwargs = mock_get.call_args.kwargs
        assert called_url == "https://www.pathe.fr/api/show/movie-a"
        assert called_kwargs["params"] == {"language": "fr"}
