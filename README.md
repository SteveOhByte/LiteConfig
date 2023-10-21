


# LiteConfig (.lc) Library

![LiteConfigLogo](https://i.imgur.com/NblUImL.png)

LiteConfig (.lc) is a lightweight configuration file format designed for ease of use primacy.

## Features

-   **Types**: Read values in standard types int, float, double, bool, and string with dedicated methods
		- Other types will attempt to be dynamically understood
-   **Dictionary**: Optionally cache your config file using the included *Data* property for dictionary access
-   **Simplicity**: LC is designed to be a very barebones format, and as such very few "extras" are included

## Installation

Using NuGet:


`Install-Package LiteConfig` 

## Example File

```
# Single line comments
title: Example
shortForm: single-line strings do not require double quotes
longForm: "strings of text with multiple lines,
can be written by enclosing the whole block of text in double quotes."
firstToggle: True
secondToggle: false
whatDayIsIt: 01/01/1970
lines: 11
collection: item1, item2
collectionSize: 2.0
```
## Usage

```
using LiteConfig;

public class App
{
	public App()
	{
		LC lc = new();
		lc.LoadFromFile("config.lc");
		string name = lc.Data["name"];

		// OR
		name = LC.ReadString("config.lc", "name");

		// OR
		name = LC.ReadValue<string>("config.lc", "name");

		string item1 = LC.ReadList<string>("config.lc", "collection").ToArray()[0];
	}
}
```
## Distribution and Usage Rights
You are granted full freedom and permission to:

-   **Distribute** this library as you see fit, whether in its original form or in a modified version.
-   **Decompile** and use any part of the code directly in your projects.
-   **Copy** code from the repository, and use any part of the code directly in your projects.
-   **Bundle** it within an executable or any other software structure.
-   **Re-purpose** and use this library in any other manner you deem necessary.

In essence, you have carte blanche to utilize this library in any manner you choose. Please remember to respect the licensing agreement for any other third-party libraries or resources that LiteConfig may depend on.

## License

This library is licensed under the [GPL License](https://www.gnu.org/licenses/gpl-3.0.en.html#license-text).
