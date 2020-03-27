using Caf.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Core
{
    public interface IFileUploader : ISingleton
    {
        /// <summary>
        /// 上传文件，返回文件路径或Key。
        /// 注意路径为：a/b/c
        /// </summary>
        /// <param name="filePath">目录或路径</param>
        /// <param name="fileName">源文件名称，不需要处理</param>
        /// <param name="stream"></param>
        /// <returns></returns>
        Task<string> UploadStaticFileAsync(string filePath, string fileName, Stream stream);
        Task<string> UploadFileAsync(string filePath, string fileName, Stream stream);

        Task<string> UploadOpenFileAsync(string filePath, string fileName, Stream stream);

        Task<Stream> GetFileStreamAsync(string filePath);

        Task<string> GetFullFilePathAsync(string filePath);
        Task<string> GetDownLoadFilePathAsync(string key);
        Task ReadStreamAsync(string key, Action<Stream> read);
        Task<List<string>> GetFileInfoListAsync(string path);
        Task<string> GetFullFilePathAsync(string filePath,DateTime Expires);
    }
}
