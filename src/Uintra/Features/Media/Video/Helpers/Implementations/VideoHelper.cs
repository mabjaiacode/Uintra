﻿using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Compent.Extensions;
using Uintra.Core.MediaToolkit;
using Uintra.Core.MediaToolkit.Model;
using Uintra.Core.MediaToolkit.Options;
using Uintra.Features.Media.Extensions;
using Uintra.Features.Media.Video.Helpers.Contracts;
using Uintra.Features.Media.Video.Models;
using Uintra.Infrastructure.ApplicationSettings;
using Uintra.Infrastructure.Constants;
using Umbraco.Core.Models;

namespace Uintra.Features.Media.Video.Helpers.Implementations
{
    public class VideoHelper : IVideoHelper
    {
        private const string ThumbnailFileExtension = ".jpg";

        private readonly IApplicationSettings _applicationSettings;

        public VideoHelper(IApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }

        public bool IsVideo(string extension)
        {
            var cleanExtension = extension.ClearExtension();

            return _applicationSettings.VideoFileTypes.Contains(cleanExtension);
        }

        public string CreateThumbnail(IMedia media)
        {
            var fileUrl = media.GetValue<string>(UmbracoAliases.Media.UmbracoFilePropertyAlias);
            var fileFullPath = HostingEnvironment.MapPath(fileUrl);

            var directoryName = Path.GetDirectoryName(fileFullPath);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileFullPath);
            var outputFileFullPath = Path.Combine(directoryName, $"{fileNameWithoutExtension}{ThumbnailFileExtension}");

            using (var engine = new Engine(HostingEnvironment.MapPath(IntranetConstants.FfmpegRelativePath)))
            {
                var inputMediaFile = new MediaFile { Filename = fileFullPath };
                var outputMediaFile = new MediaFile { Filename = outputFileFullPath };

                engine.GetMetadata(inputMediaFile);

                var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(inputMediaFile.Metadata.Duration.TotalSeconds / 2) };
                engine.GetThumbnail(inputMediaFile, outputMediaFile, options);

                return GetThumbnailUrl(fileUrl);
            }
        }

        public VideoSizeMetadataModel GetSizeMetadata(IMedia media)
        {
            var fileUrl = media.GetValue<string>(UmbracoAliases.Media.UmbracoFilePropertyAlias);
            var fileFullPath = HostingEnvironment.MapPath(fileUrl);

            var inputMediaFile = new MediaFile { Filename = fileFullPath };

            using (var engine = new Engine(HostingEnvironment.MapPath(IntranetConstants.FfmpegRelativePath)))
            {
                engine.GetMetadata(inputMediaFile);

                var sizeMetadata = inputMediaFile.Metadata.VideoData.FrameSize.SplitBy("x").Select(int.Parse).ToArray();

                return new VideoSizeMetadataModel
                {
                    Width = sizeMetadata[0],
                    Height = sizeMetadata[1]
                };
            }
        }

        private static string GetThumbnailUrl(string videoUrl)
        {
            var urlSegments = videoUrl.SplitBy("/").ToList();
            var fileName = urlSegments.Last();
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            urlSegments.Remove(fileName);
            urlSegments.Add($"{fileNameWithoutExtension}{ThumbnailFileExtension}");

            return $"/{urlSegments.JoinWith("/")}";
        }

        public string CreateConvertingThumbnail() => 
            @"/images/videoLoader.jpg";

        public string CreateConvertingFailureThumbnail() => 
            @"/images/videoLoaderFailure.jpg";
    }
}