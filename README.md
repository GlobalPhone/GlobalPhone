# GlobalPhone [![Build Status](https://travis-ci.org/GlobalPhone/GlobalPhone.png?branch=master)](https://travis-ci.org/GlobalPhone/GlobalPhone) [![Build status](https://ci.appveyor.com/api/projects/status/qulay38neeeqpdbp/branch/master?svg=true)](https://ci.appveyor.com/project/wallymathieu/globalphone-d4koy/branch/master)

GlobalPhone parses, validates, and formats local and international phone numbers according to the [E.164 standard](http://en.wikipedia.org/wiki/E.164).

**Store and display phone numbers in your app.** Accept phone number input in national or international format. Convert phone numbers to international strings (`+13125551212`) for storage and retrieval. Present numbers in national format (`(312) 555-1212`) in your UI.

**Designed with the future in mind.** GlobalPhone uses the c# implementation based on Google's open-source [libphonenumber](http://code.google.com/p/libphonenumber/) database.

## Installation

1. Add the `GlobalPhone` nuget package to your app. For example, using Package Manager Console:

        PM> Install-Package GlobalPhone

Or you can add it as a solution level package.


## Examples

Parse an international number string into a `GlobalPhone::Number` object:

```
var number = GlobalPhone.Parse("+1-312-555-1212");
# => #{GlobalPhone::Number +13125551212}
```

Query the country code and likely territory name of the number:

```
number.CountryCode
# => 1

number.RegionCode
# => "US"
```

Present the number in national and international formats:

```
number.NationalFormat
# => "(312) 555-1212"

number.InternationalFormat
# => "+1 312-555-1212"
```

Is the number valid? (Note: this is not definitive. For example, the number here is "IsValid" by format, but there are no US numbers that start with 555. The `IsValid` method may return false positives, but *should not* return false negatives unless the database is out of date.)

```
number.IsValid
# => true
```

Get the number's normalized E.164 international string:

```
number.InternationalString
# => "+13125551212"
```

Parse a number in national format for a given territory:

```
number = GlobalPhone.Parse("(0) 20-7031-3000", "gb");
# => #{GlobalPhone::Number +442070313000}
```

Parse an international number using a territory's international dialing prefix:

```
number = GlobalPhone.Parse("00 1 3125551212", "gb");
# => #{GlobalPhone::Number +13125551212}
```

Set the default territory to Great Britain (territory names are [ISO 3166-1 Alpha-2](http://en.wikipedia.org/wiki/ISO_3166-1_alpha-2) codes):

```
GlobalPhone.DefaultTerritoryName = "gb";
# => "gb"

GlobalPhone.Parse("(0) 20-7031-3000");
# => #{GlobalPhone::Number +442070313000}
```

Shortcuts for validating a phone number:

```
GlobalPhone.Validate("+1 312-555-1212");
# => true

GlobalPhone.Validate("+442070313000");
# => true

GlobalPhone.Validate("(0) 20-7031-3000");
# => false

GlobalPhone.Validate("(0) 20-7031-3000", "gb");
# => true
```

Shortcuts for normalizing a phone number in E.164 format:

```
GlobalPhone.Normalize("(312) 555-1212");
# => "+13125551212"

GlobalPhone.Normalize("+442070313000");
# => "+442070313000"

string normalized;
GlobalPhone.TryNormalize("(0) 20-7031-3000", out normalized);
# => false

GlobalPhone.Normalize("(0) 20-7031-3000");
# => #{PhoneNumbers::NumberParseException}

GlobalPhone.Normalize("(0) 20-7031-3000", "gb");
# => "+442070313000"
```

## Caveats

GlobalPhone currently does not parse emergency numbers or SMS short code numbers.

Validation is not definitive and may return false positives, but *should not* return false negatives unless the database is out of date.

Territory heuristics are imprecise. Parsing a number will usually result in the territory being set to the primary territory of the region. For example, Canadian numbers will be parsed with a territory of `US`. (In most cases this does not matter, but if your application needs to perform geolocation using phone numbers, GlobalPhone may not be a good fit.)

## Development
The GlobalPhone source code is [hosted on GitHub](https://github.com/GlobalPhone/GlobalPhone). You can check out a copy of the latest code using Git:

    CMD> git clone https://github.com/GlobalPhone/GlobalPhone.git

If you've found a bug or have a question, please open an issue on the [issue tracker](https://github.com/GlobalPhone/GlobalPhone/issues). Or, clone the GlobalPhone repository, write a failing test case, fix the bug, and submit a pull request.

GlobalPhone is a port of Sam Stephenson GlobalPhone for ruby [hosted on GitHub](https://github.com/sstephenson/global_phone).

### License

Copyright &copy; 2013 Sam Stephenson, Oskar Gewalli

Released under the MIT license. See [`LICENSE`](LICENSE) for details.
