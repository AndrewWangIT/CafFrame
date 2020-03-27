using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Caf.Core.Utils.DirectoryFinder
{
    public static class DirectoryFinder
    {

        public static string GetFolder(string path)
        {
            var baseBaseDirectory = AppContext.BaseDirectory;

            var app_data = Path.Combine(baseBaseDirectory, path);

            if (!Directory.Exists(app_data))
            {
                Directory.CreateDirectory(app_data);
            }
            return app_data;
        }

        private static bool DirectoryContains(string directory, string fileName)
        {
            return Directory.GetFiles(directory).Any(filePath => string.Equals(Path.GetFileName(filePath), fileName));
        }
    }
}
