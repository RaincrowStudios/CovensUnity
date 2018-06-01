Game Localization
----
Helps localizing the game to many languages, like English, German, Portuguese.

----
language source must be generated from Google Doc's Spread sheet;
e.g.: from Runner Project:
https://docs.google.com/spreadsheets/d/10sE0y-XNri_hQBBEH9KI_gj63C4P-2Y5-YIVQQ3LgxA/edit#gid=0
Generate from Menu: Oktagon/Generate Lokaki, and Follow the instructions shown in generated window.

----

Load Language Database:
Loaded first from the asset at 'GameSettings/Lokakit/Lokakit.txt' as data base source;
can be loaded dynamically (from download, or cache) using Lokaki.SetLanguageDatabase(string);

Change the language with
Lokaki.SetCurrentLanguage(LANGUAGES.English);

----
