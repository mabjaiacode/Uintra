﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using uIntra.Core.Exceptions;
using uIntra.Core.Extentions;
using uIntra.Core.Media;
using uIntra.Core.TypeProviders;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using File = System.IO.File;

namespace uIntra.Search
{
    public class DocumentIndexer : IIndexer, IDocumentIndexer
    {
        public const string UseInSearchPropertyAlias = "useInSearch";
        private readonly IElasticDocumentIndex _documentIndex;
        private readonly UmbracoHelper _umbracoHelper;
        private readonly ISearchApplicationSettings _settings;
        private readonly IMediaHelper _mediaHelper;
        private readonly IExceptionLogger _exceptionLogger;
        private readonly IContentService _contentService;

        public DocumentIndexer(IElasticDocumentIndex documentIndex,
            UmbracoHelper umbracoHelper,
            ISearchApplicationSettings settings,
            IMediaHelper mediaHelper,
            IExceptionLogger exceptionLogger,
            IContentService contentService)
        {
            _documentIndex = documentIndex;
            _umbracoHelper = umbracoHelper;
            _settings = settings;
            _mediaHelper = mediaHelper;
            _exceptionLogger = exceptionLogger;
            _contentService = contentService;
        }

        public void FillIndex()
        {
            var documentsToIndex = GetDocumentsForIndexing();
            _documentIndex.Index(documentsToIndex);
        }

        private IEnumerable<SearchableDocument> GetDocumentsForIndexing()
        {
            var medias = _umbracoHelper
                .TypedMediaAtRoot()
                .SelectMany(m => m.DescendantsOrSelf());

            var result = medias
                .Where(c => IsAllowedForIndexing(c) && !_mediaHelper.IsMediaDeleted(c))
                .SelectMany(GetSearchableDocument);

            return result.ToList();
        }

        private bool IsAllowedForIndexing(IPublishedContent media)
        {
            return media.GetPropertyValue<bool>(UseInSearchPropertyAlias);
        }

        public void Index(int id)
        {
            Index(id.ToEnumerableOfOne());
        }

        public void Index(IEnumerable<int> ids)
        {
            var medias = _contentService.GetByIds(ids);
            var documents = new List<SearchableDocument>();

            foreach (var media in medias)
            {
                var document = GetSearchableDocument(media.Id);
                if (document == null) continue;
                media.SetValue(UseInSearchPropertyAlias, true);
                _contentService.SaveAndPublishWithStatus(media);
                documents.AddRange(document);
            }
            _documentIndex.Index(documents);
        }

        public void DeleteFromIndex(int id)
        {
            DeleteFromIndex(id.ToEnumerableOfOne());
        }

        public void DeleteFromIndex(IEnumerable<int> ids)
        {
            var medias = _contentService.GetByIds(ids);
            foreach (var media in medias)
            {
                media.SetValue(UseInSearchPropertyAlias, false);
                _contentService.SaveAndPublishWithStatus(media);
                _documentIndex.Delete(media.Id);
            }
        }

        private IEnumerable<SearchableDocument> GetSearchableDocument(int id)
        {
            var content = _umbracoHelper.TypedMedia(id);
            if (content == null)
            {
                return Enumerable.Empty<SearchableDocument>();
            }

            return GetSearchableDocument(content);
        }

        private IEnumerable<SearchableDocument> GetSearchableDocument(IPublishedContent content)
        {
            var fileName = Path.GetFileName(content.Url);
            var extension = Path.GetExtension(fileName)?.Trim('.');

            bool isFileExtensionAllowedForIndex = _settings.IndexingDocumentTypesKey.Contains(extension, StringComparison.OrdinalIgnoreCase);


            if (content.Url.IsNullOrEmpty())
            {
                var physicalPath = HostingEnvironment.MapPath(content.Url);

                if (!File.Exists(physicalPath))
                {
                    _exceptionLogger.Log(new FileNotFoundException($"Could not find file \"{physicalPath}\""));
                   return Enumerable.Empty<SearchableDocument>();
                }
                var base64File = isFileExtensionAllowedForIndex ? Convert.ToBase64String(File.ReadAllBytes(physicalPath)) : string.Empty;
                var result = new SearchableDocument
                {
                    Id = content.Id,
                    Title = fileName,
                    Url = content.Url,
                    Data = base64File,
                    Type = SearchableTypeEnum.Document.ToInt()
                };
                 return result.ToEnumerableOfOne();
            }
            return Enumerable.Empty<SearchableDocument>();
        }
    }
}
