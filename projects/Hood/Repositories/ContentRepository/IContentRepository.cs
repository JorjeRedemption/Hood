﻿using Hood.Enums;
using Hood.Infrastructure;
using Hood.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hood.Services
{
    public interface IContentRepository
    {
        // Content CRUD
        Task<ContentModel> GetPagedContent(ContentModel model, bool publishedOnly = true);
        List<Content> GetContentByType(string type, string categorySlug = null, bool publishedOnly = true);
        Content GetContentByID(int id, bool clearCache = false);
        OperationResult Add(Content content);
        OperationResult Update(Content content);
        OperationResult Delete(int id);
        OperationResult<Content> SetStatus(int id, Status status);
        Task<OperationResult> DeleteAll(string type);
        Task<OperationResult<Content>> AddImage(Content content, ContentMedia contentMedia);

        // Content Views
        List<Content> GetRecent(string type, string categorySlug = null);
        List<Content> GetFeatured(string type, string categorySlug = null);
        ContentNeighbours GetNeighbourContent(int id, string type, string categorySlug = null);

        // Other Content functions
        void UpdateTemplateMetas(Content content, List<string> newMetas);
        void RefreshMetas(Content content);
        void RefreshAllMetas();
        List<string> GetMetasForTemplate(string templateName, string folder);
        bool CheckSlug(string slug, int? id = null);

        // Tags
        Task<OperationResult<ContentTag>> AddTag(string value);
        Task<OperationResult> DeleteTag(string value);

        // Categories
        Task<ContentCategory> GetCategoryById(int categoryId);
        IEnumerable<ContentCategory> GetCategories(int contentId);
        Task<OperationResult<ContentCategory>> AddCategory(ContentCategory category);
        Task<OperationResult<ContentCategory>> AddCategory(string value, string type);
        Task<OperationResult> AddCategoryToContent(int contentId, int categoryId);
        OperationResult RemoveCategoryFromContent(int contentId, int categoryId);
        Task<OperationResult> DeleteCategory(int categoryId);
        Task<OperationResult> UpdateCategory(ContentCategory model);

        // Sitemap Functions
        List<SitemapPage> GetPages(string categorySlug = null, bool publishedOnly = true);
        string GetSitemapDocument(IUrlHelper urlHelper);

        // Non Content Related
        Task<List<LinqToTwitter.Status>> GetTweets(string name, int count);
        List<Country> AllCountries();
        Country GetCountry(string name);
    }
}