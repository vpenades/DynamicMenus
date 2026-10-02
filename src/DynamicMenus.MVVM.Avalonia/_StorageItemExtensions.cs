using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Avalonia.Platform.Storage;

namespace DynamicMenus
{
    internal static class StorageItemExtensions
    {
        public static (T?, bool) ConvertTo<T>(this IStorageItem item)
        {
            return ConvertTo<T>(new[] { item });
        }

        public static (T?, bool) ConvertTo<T>(this IReadOnlyCollection<IStorageItem> items)
        {
            if (items == null || items.Count == 0) return (default, true);            

            // avalonia
            if (_TryConvert<T>(items, out var r0)) return (r0, false);
            if (_TryConvert<T>(items.FirstOrDefault(), out var r1)) return (r1, false);

            // system - file
            if (_TryConvert<T>(_ToFileInfo(items.FirstOrDefault()), out var r2)) return (r2, true);

            var systemFiles = items.Select(_ToFileInfo).Where(item => item != null).ToArray();
            if (systemFiles.Length > 0 && _TryConvert<T>(systemFiles, out var r3)) return (r3, true);

            // system - folder
            if (_TryConvert<T>(_ToDirectoryInfo(items.FirstOrDefault()), out var r4)) return (r4, true);

            var systemDirs = items.Select(_ToDirectoryInfo).Where(item => item != null).ToArray();
            if (systemDirs.Length > 0 && _TryConvert<T>(systemDirs, out var r5)) return (r4, true);
            

            throw new InvalidOperationException($"unable to cast collection of {typeof(IStorageFile).Name} into {typeof(T).Name}");
        }

        private static bool _TryConvert<Tout>(object? value, [NotNullWhen(true)] out Tout? result)
        {
            result = default;

            if (value is null) return false;
            if (value is not Tout compatibleValue) return false;
            result = compatibleValue;
            return true;
        }

        private static FINFO? _ToFileInfo(IStorageItem? item)
        {
            switch (item)
            {
                case null: return null;
                case IStorageFile file:
                    var path = file.TryGetLocalPath();
                    return string.IsNullOrWhiteSpace(path)
                        ? null
                        : new FINFO(path);

                default: return null;
            }
        }

        private static DINFO? _ToDirectoryInfo(IStorageItem? item)
        {
            switch (item)
            {
                case null: return null;
                case IStorageFolder folder:
                    var path = folder.TryGetLocalPath();
                    return string.IsNullOrWhiteSpace(path)
                        ? null
                        : new DINFO(path);

                default: return null;
            }
        }

    }
}
