﻿using Raven.Client;
using Raven.Client.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RavenDbTalk
{
    public static class RavenDbExtensions
    {
        public static string GetFullDocumentKey<T>(this IDocumentStore store, object partialId)
        {
            return store.Conventions.FindFullDocumentKeyFromNonStringIdentifier(partialId, typeof(T), false);
        }

        public static string GetPartialDocumentKey(this IDocumentStore store, string fullId)
        {
            return store.Conventions.FindIdValuePartForValueTypeConversion(null, fullId);
        }

        public static Lazy<T> LazyFirstOfDefault<T>(this IQueryable<T> self, Expression<Func<T, bool>> predicate = null)
        {
            Lazy<IEnumerable<T>> lazy = predicate == null ? self.Take(1).Lazily() : self.Where(predicate).Take(1).Lazily();

            return new Lazy<T>(() => lazy.Value.FirstOrDefault());
        }
    }

    public static class RavenFsExtensions
    {
        public static string CombinePaths(this IFilesStore store, params string[] paths)
        {
            var trimChars = Regex.Escape(store.Conventions.IdentityPartsSeparator);
            return AggregatePaths(paths, store.Conventions.IdentityPartsSeparator, trimChars);
        }

        public static string CombinePaths(this IAsyncFilesCommands asyncCommands, params string[] paths)
        {
            var trimChars = Regex.Escape(asyncCommands.Conventions.IdentityPartsSeparator);
            return AggregatePaths(paths, asyncCommands.Conventions.IdentityPartsSeparator, trimChars);
        }

        private static string AggregatePaths(string[] paths, string identityPartsSeparator, string trimChars)
        {
            return paths.Aggregate(string.Empty, (combinedPath, path) => $"{combinedPath}{(string.IsNullOrEmpty(combinedPath) ? "" : identityPartsSeparator)}{Regex.Replace(path, $"^(\\s*{trimChars}\\s*)+|(\\s*{trimChars}\\s*)+$", "")}");
        }
    }
}
