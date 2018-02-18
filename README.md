# iso-space

Top-down (isometric) space quest/survival game. Something slightly similar to Alien isolation. We would need to come up with a decent storyline.


Useful links
===

* Discord: https://discord.gg/nNNnjcg
* Google Drive: https://drive.google.com/drive/folders/1SObUunXNm5MZZyp2SI-xkODnPrFSId33?usp=sharing

Git style
===

Please use the folowing prefix to your commits:

* [+] - if you added something new in this commit
* [*] - if you fixed something minor in this commit
* [**] - if you fixed something major in this commit
* [***] - if you fixed something critical in this commit
* [=] - use it if your commit doesn't contain imaportant things
* [-] - if you depricate/remove something

For example, if you add a new stage to the game I'll commit it as:

	[+] Added best stage ever

If you fix major bug:

	[**] Fixed major bug in AI behaviour

For something small:

	[=] Fixed several typos in readme

TODO
===

TBD

Cheats
===

Howto use git large file support:

	git lfs track "*.psd"

This command will add to .gitattributes a new line, so don't forget to commit changes to .gitattributes

Change Log
===

Please add your changes to the end of the list

* h0x91b: Added new Unity project `iso-space`
* h0x91b: Added scene "MainMenu" and empty `Stage1`
* h0x91b: Configured a Unity project for correct working with git (using this article https://robots.thoughtbot.com/how-to-git-with-unity)
* h0x91b: Added this README.md

Contributors
===

* Arseniy Pavlenko (aka h0x91b)