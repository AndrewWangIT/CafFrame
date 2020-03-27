using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Caf.Core.DependencyInjection;

namespace Caf.Core
{
    public class LocalUploader : IFileUploader
    {
        private string _localPath = Path.Combine(Directory.GetCurrentDirectory(), "files");

        public async Task<string> GetFullFilePathAsync(string filePath)
        {
            var fullFilePath = Path.Combine(_localPath, filePath);
            return await Task.FromResult(fullFilePath);
        }



        public async Task<string> UploadFileAsync(string fileDirectory, string fileName, Stream stream)
        {
            string filePath = Path.Combine(_localPath, fileDirectory);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            filePath = Path.Combine(_localPath, fileDirectory, fileName);

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await stream.CopyToAsync(fs);
            }

            return Path.Combine(fileDirectory, fileName);
        }

        public async Task<Stream> GetFileStreamAsync(string filePath)
        {
            var fullFilePath = Path.Combine(_localPath, filePath);

            if (File.Exists(fullFilePath))
            {
                MemoryStream ms = new MemoryStream();
                {
                    using (var fs = new FileStream(fullFilePath, FileMode.Open))
                    {
                        await fs.CopyToAsync(ms);
                    }
                    ms.Position = 0;
                    return ms;
                }
            }
            else
            {
                return null;
            }
        }

        public Task<string> UploadOpenFileAsync(string filePath, string fileName, Stream stream)
        {
            return null;
        }

        public Task<string> UploadStaticFileAsync(string filePath, string fileName, Stream stream)
        {
            throw new NotImplementedException();
        }

        public Task ReadStreamAsync(string key, Action<Stream> read)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetFileInfoListAsync(string path)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetFullFilePathAsync(string filePath, DateTime Expires)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetDownLoadFilePathAsync(string key)
        {
            throw new NotImplementedException();
        }
    }
}
