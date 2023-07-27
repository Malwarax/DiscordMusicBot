﻿using DiscordMusicBot.Domain.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DiscordMusicBot.Application.Services
{
    public class YoutubeService
    {
        private const string YoutubeWatchUrl = "https://www.youtube.com/watch";
        private const string ApiKeyPattern = @"""INNERTUBE_API_KEY"":""(.+?)""";
        private const string ApiVersionPattern = @"""INNERTUBE_CLIENT_VERSION"":""(.+?)""";
        private const string ApiNamePattern = @"""INNERTUBE_CLIENT_NAME"":""(.+?)""";
        private const string VideoIdPattern = @".+\/(?>watch\?v=|.{0})(.+)";

        private const string ApiSearchUrl = "https://www.youtube.com/youtubei/v1/player?key=";
        private readonly HttpClient _httpClient;

        public YoutubeService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<SongModel> GetSongInfoAsync(string url)
        {
            var videoId = GetVideoId(url);

            if (string.IsNullOrEmpty(videoId))
            {
                return null;
            }

            var site = await _httpClient.GetStringAsync(YoutubeWatchUrl);

            if (string.IsNullOrEmpty(site))
            {
                return null;
            }

            var apiKeyMatch = Regex.Match(site, ApiKeyPattern);
            var apiVersionMatch = Regex.Match(site, ApiVersionPattern);
            var apiNameMatch = Regex.Match(site, ApiNamePattern);

            var isApiParsed = apiKeyMatch.Success
                              && apiVersionMatch.Success
                              && apiNameMatch.Success;

            if (!isApiParsed)
            {
                return null;
            }

            var apiKey = apiKeyMatch.Groups[1].Value;
            var apiVersion = apiVersionMatch.Groups[1].Value;
            var apiName = apiNameMatch.Groups[1].Value;

            return await LoadVideoInfoAsync(url, videoId, apiKey, apiVersion, apiName);
        }

        private async Task<SongModel> LoadVideoInfoAsync(string url, string videoId, string apiKey, string apiVersion, string apiName)
        {
            var requestModel = new YoutubeRequestModel
            {
                VideoId = videoId,
                Context = new YoutubeRequestModel.ContextModel
                {
                    Client = new YoutubeRequestModel.ContextModel.ClientModel
                    {
                        ClientName = apiName,
                        ClientVersion = apiVersion,
                    }
                }
            };

            var requestBodyString = JsonSerializer.Serialize(requestModel);

            var response =
                await _httpClient.PostAsync($"{ApiSearchUrl}{apiKey}", new StringContent(requestBodyString));
            var content = await response.Content.ReadAsAsync<YoutubeResponseModel>();

            var videoDetails = content?.VideoDetails;
            bool isVideoDataExist = videoDetails?.Author != null && videoDetails?.Title != null;

            if (!isVideoDataExist)
            {
                return null;
            }

            return new SongModel
            {
                Author = videoDetails.Author,
                Title = videoDetails.Title,
                Url = url,
            };
        }

        private string GetVideoId(string url)
        {
            Match idMath = Regex.Match(url, VideoIdPattern);

            if (idMath.Success)
            {
                return idMath.Groups[1].Value;
            }

            return string.Empty;
        }
    }
}
