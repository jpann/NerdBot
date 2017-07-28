# NerdBot

This is a GroupMe bot that lets people in chat query Magic: The Gathering card information such as echomtg prices, and card images.

The bot uses a MongoDB database that contains card information that is imported from mtgjson.com (*Set + Extras* json format). You will have to import this information yourself using the [DatabaseUpdater](https://github.com/jpann/NerdBot/tree/master/NerdBot/NerdBot_DatabaseUpdater).

### Build
Builds in Visual Studio 2012 and Xamarin Studio 5.7.

Tested on Windows 7, Windows 10 x64 and Ubuntu 14.04, 15.04 x86_64 using Mono 3.2.8.

You'll probably have to do some tweaking to get this to work in your environment/setup.

### Core Features
* Plugin system that lets you create new commands by creating a plugin.
* Query for card images.
* Query for card price from EchoMTG.com
* Query for list of other sets a card is in.
* Simulate a coin flip.
* Simulate a dice roll.
* Query for a random card that contains a specified static ability.
* Query for a random card that contains specified text in card text.
* Query information for a set.
* Query for a random card for a specified artist.
* Query for a random card.
* Query for flavor text from a random card.
* Query for a random card in a specified set.
* Query for card rulings for a specified card.
* Search for cards using specified text.
* Check which formats a specified card is legal in.

### Additional Features Available From Plugins
* Get random gif from Giphy that matches the specified text.
* Get random top comment from a random thread on r/roastme.
* Query top deck information from TappedOut.net
* Get UrbanDictionary definition for specified text.
* Get list of sets currently in Standard format.
* Query WolframAlpha for specified text.

### Configuration

Example configuration files are provided in the project directories as *&ast;-Example.ini*. Copy the example configuration file and removing *'-Example'* in order to create a new configuration file.
