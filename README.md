# Silentstorm

Fanvid Studio management tools.

## Silentstorm Backoffice

<img src="resources/Screenshot-5.png" alt="Main page" width="1000"/>
<img src="resources/Screenshot-6.png" alt="Project page" width="1000"/>

The backoffice lets you view your projects, create projects, create project announcements, add song submissions, add song votes, manage users, etc.  

## Silentstorm Discord Bot

<img src="resources/Screenshot-2.png" alt="Main page" width="1000"/>
<img src="resources/Screenshot-3.png" alt="Main page" width="1000"/>
<img src="resources/Screenshot-7.png" alt="Main page" width="1000"/>
<img src="resources/Screenshot-4.png" alt="Main page" width="1000"/>

The discord bot lets you register and unregister channels for announcements, and submit songs for projects.  

## Running the applications

Download the [`.NET Core 6 Runtime`](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) to be able to run the applications.  
Download the latest version available on the [Releases](https://github.com/ShadowXPA/silentstorm/releases) tab.  

### Database and Lavalink

You will need a [MySQL](https://www.mysql.com/) database and [Lavalink](https://github.com/lavalink-devs/Lavalink) **version 3** to run these applications successfully.
If you have [Docker](https://www.docker.com/) installed, there is a `docker-compose.yml` with both MySQL and Lavalink. Edit it to your liking.  

### Silentstorm Backoffice

Before running the backoffice application, edit the `backoffice.json` configuration file.  
To run the backoffice, simply run the `Backoffice.exe` (for Windows) or `./Backoffice` (for Linux).  
The application should start on [`https://localhost:5001`](https://localhost:5001).

### Silentstorm Discord Bot

Before running the discord bot, edit the `discordbot.json` configuration file. And do not forget to add your bot's token.  
If you do not have a token, head over to the [Discord Developers](https://discord.com/developers) page, create an application, and on the `Bot`'s tab you can grab a `Token`.
To run the bot, simply run the `DiscordBot.exe` (for Windows) or `./DiscordBot` (for Linux).  

## Future work

This project is lacking in a lot of features, namely security features.
It is not production ready!
It is best for personal use, with limited access to the backoffice.  

Below are some features that could be integrated.  

### Silentstorm Backoffice

- Roles (Admin, Member, Editor, etc.)
- Associate members to projects
- Public/Private projects
- Allow members to submit their parts (either a link or a file)
- Profiles for each member, with their public projects
- Pagination on the various pages

### Silentstorm Discord Bot

- Automatic discord role creation
- Automatic discord role assignments
- Join project command
- Submit parts ([unlisted] YouTube links?)
- Automatically create public/private threads for each project
- Alter the submit command to allow for per project song submission
