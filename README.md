# Discord YouTube Music Bot
A simple Discord music bot for Windows x64 platforms that supports songs from YouTube.

## Features
  * Supports youtube/music.youtube/youtu.be links
  * Supports song queue
  * Supports online streams
  * Supports several Discord servers simultaneously

## Commands
Each command has a prefix (`!` by default) that can be configured in the appsettings.json file.

Available commands:
  * Play a song / add a song to a queue `!play <link>`
  * Skip a song `!skip`
  * Stop the bot `!stop`
  * Get a queue `!queue`
  * Remove a song from a queue `!queue-remove <position>`
  * Shuffle a queue `!queue-shuffle`
  * Help `!help`

## Setup 

![Windows](https://img.shields.io/badge/-Windows_x64-blue.svg?style=for-the-badge&logo=windows)

1. Install [FFMPEG](https://www.ffmpeg.org/download.html) ([youtube tutorial](https://www.youtube.com/watch?v=r1AtmY-RMyQ&t=34s))
2. Install [yt-dlp](https://github.com/yt-dlp/yt-dlp) (the installation process is the same)
3. Create a bot on the Discord Developer portal and add it to your server with the following scopes and permissions:
    * Scopes: bot
    * Permissions: Read Messages/View Channels, Send Messages, Connect, Speak
4. Turn on the MESSAGE CONTENT INTENT on the Discord Developer portal
5. Copy your bot token to the appsettings.json file
6. Build the solution and run the console application

### Configuration
You can configure your bot in AppSettings as per below.  

```json
  "BotOptions": {
    "CommandPrefix": "!",
    "Token": "token"
  }
```

## Licensing
The DiscordMusicBot project is licensed under the [MIT license](https://github.com/Malwarax/DiscordMusicBot/blob/master/LICENSE).

#### Voice binaries

|  Library  | License                                                   |
| :-------: | :-------------------------------------------------------- |
|   Opus    | https://opus-codec.org/license/                           |
| libsodium | https://github.com/jedisct1/libsodium/blob/master/LICENSE |
