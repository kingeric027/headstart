using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Marketplace.Common.Exceptions;
using Marketplace.Common.Extensions;
using Marketplace.Common.Models;
using ErrorCodes = Marketplace.Common.Exceptions.ErrorCodes;

namespace Marketplace.Common.Helpers
{
    public interface IListArgs
    {
        string Search { get; set; }
        string SearchOn { get; set; }
        IList<string> SortBy { get; set; }
        int Page { get; set; }
        int PageSize { get; set; }
        IList<ListFilter> Filters { get; set; }
        void ValidateAndNormalize();
    }

    public class EmptyListArgs : IListArgs
    {
        public EmptyListArgs()
        {
            Page = 1;
            PageSize = ListPage.DEFAULT_PAGE_SIZE;
        }
        public string SearchOn { get; set; }
        public string Search { get; set; }
        public IList<string> SortBy { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IList<ListFilter> Filters { get; set; }
        public void ValidateAndNormalize() { }
    }

    [ModelBinder(typeof(ListArgsModelBinder))]
    public class ListArgs<T> : IListArgs
    {
        public ListArgs()
        {
            Page = 1;
            PageSize = ListPage.DEFAULT_PAGE_SIZE;
        }
        public string SearchOn { get; set; }
        public string Search { get; set; }
        public IList<string> SortBy { get; set; } = new List<string>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IList<ListFilter> Filters { get; set; }

        public void ValidateAndNormalize()
        {
            var newSortBy = new List<string>();
            foreach (var s in this.SortBy)
            {
                if (newSortBy.Contains(s, StringComparer.InvariantCultureIgnoreCase))
                    continue;
                var desc = s.StartsWith("!");
                var name = s.TrimStart('!');
                var prop = FindSortableProp(typeof(T), name);
                newSortBy.Add(desc ? "!" + prop : prop);
            }
            this.SortBy = newSortBy;
        }

        private string FindSortableProp(Type type, string path)
        {
            if (path.StartsWith("xp."))
                return path;

            var queue = new Queue<string>(path.Split('.'));
            var prop = type.GetProperty(queue.Dequeue(), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            Require.That(prop != null, ErrorCodes.List.InvalidProperty, new InvalidPropertyError(type, path));
            Require.That(prop.HasAttribute<SortableAttribute>(), ErrorCodes.List.InvalidSortProperty, new InvalidPropertyError(type, path));
            var result = prop?.Name;
            if (queue.Any())
                result += "." + FindSortableProp(prop.PropertyType, queue.JoinString("."));
            return result;
        }
    }

    public class ListArgsModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            if (bindingContext.ModelType.WithoutGenericArgs() != typeof(ListArgs<>))
                return Task.CompletedTask;

            var listArgs = (IListArgs)Activator.CreateInstance(bindingContext.ModelType);

            LoadFromQueryString(bindingContext.HttpContext.Request.Query, listArgs);
            listArgs.ValidateAndNormalize();
            bindingContext.Model = listArgs;
            bindingContext.Result = ModelBindingResult.Success(listArgs);
            return Task.CompletedTask;
        }

        public virtual void LoadFromQueryString(IQueryCollection query, IListArgs listArgs)
        {
            listArgs.Filters = new List<ListFilter>();
            foreach (var (key, value) in query)
            {
                int i;
                switch (key.ToLower())
                {
                    case "sortby":
                        listArgs.SortBy = value.ToString().Split(',').Distinct().ToArray();
                        break;
                    case "page":
                        if (int.TryParse(value, out i) && i >= 1)
                            listArgs.Page = i;
                        else
                            throw new UserErrorException("page must be an integer greater than or equal to 1.");
                        break;
                    case "pagesize":
                        if (int.TryParse(value, out i) && i >= 1 && i <= ListPage.MAX_PAGE_SIZE)
                            listArgs.PageSize = i;
                        else
                            throw new UserErrorException($"pageSize must be an integer between 1 and {ListPage.MAX_PAGE_SIZE}.");
                        break;
                    case "search":
                        listArgs.Search = value.ToString();
                        break;
                    case "searchon":
                        listArgs.SearchOn = value.ToString();
                        break;
                    default:
                        listArgs.Filters.Add(ListFilter.Parse(key, value));
                        break;
                }
            }
        }
    }
}
