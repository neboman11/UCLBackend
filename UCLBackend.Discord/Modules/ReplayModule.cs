using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using UCLBackend.Discord.Interfaces.Services;

namespace UCLBackend.Discord.Modules
{
    public class ReplayModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger _logger;
        private readonly IReplayService _replayService;
        
        public ReplayModule(ILogger logger, IReplayService replayService)
        {
            _logger = logger;
            _replayService = replayService;
        }

        [Command("upload")]
        [Summary("Begins upload process for uploading replay files to Ballchasing.com")]
        public async Task UploadReplay()
        {
            try
            {
                await _replayService.BeginUploadProcess(Context.Message.Author.Id);
                await ReplyAsync("Use !replay with the replay file attached to add replay files.\nUse !endupload when you are finished to submit the replay files");
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, $"An error occurred while uploading replay file. Started by: {Context.User.Username}.");
                await Context.Channel.SendMessageAsync(e.Message);
            }
        }

        [Command("replay")]
        [Summary("Queues a file for uploading to Ballchasing.com. !upload must be called before using this command")]
        public async Task Replay()
        {
            try
            {
                var replayUrl = Context.Message.Attachments.First().Url;
                await _replayService.QueueReplay(Context.Message.Author.Id, replayUrl);
                await ReplyAsync("Replay file queued for upload.");
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, $"An error occurred while queueing replay file. Started by: {Context.User.Username}.");
                await Context.Channel.SendMessageAsync(e.Message);
            }
        }

        [Command("endupload")]
        [Summary("Submits uploads to Ballchasing.com")]
        public async Task EndUpload()
        {
            try
            {
                await _replayService.EndUploadProcess(Context.Message.Author.Id);
                await ReplyAsync("Successfully added replays to Ballchasing.com");
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, $"An error occurred while uploading replay files. Started by: {Context.User.Username}.");
                await Context.Channel.SendMessageAsync(e.Message);
            }
        }
    }
}
