using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Caf.Core
{
    public interface IS3Facade
    {
        Task<string> UploadDynamicFileAsync(string s3OptionName, string filePath, string fileName, Stream stream, S3CannedACL s3CannedACL = null);

        Task<string> UploadDynamicFileWithNoGuidAsync(string s3OptionName, string filePath, string fileName, Stream stream, S3CannedACL s3CannedACL = null);
    }
    public class S3Facade : IS3Facade
    {
        S3Options _s3Options;
        public S3Facade(IOptions<S3Options> s3Options)
        {
            _s3Options = s3Options.Value;
        }
        public async Task<string> UploadDynamicFileAsync(string s3OptionName, string filePath, string fileName, Stream stream, S3CannedACL s3CannedACL = null)
        {
            var s3Option = _s3Options[s3OptionName];
            using (var client = CreateClient(s3Option))
            {
                string key = $"{filePath}/{Guid.NewGuid():N}_{fileName}";
                var request = new PutObjectRequest()
                {
                    BucketName = s3Option.BucketName,
                    Key = key,
                    InputStream = stream,
                    ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
                    ServerSideEncryptionCustomerProvidedKey = s3Option.AesKey,
                };
                if (s3CannedACL != null)
                {
                    request.CannedACL = s3CannedACL;
                }

                var res = await client.PutObjectAsync(request);
                if (res.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return key;
                }
            }

            return null;
        }

        public async Task<string> UploadDynamicFileWithNoGuidAsync(string s3OptionName, string filePath, string fileName, Stream stream, S3CannedACL s3CannedACL = null)
        {
            var s3Option = _s3Options[s3OptionName];
            using (var client = CreateClient(s3Option))
            {
                string key = $"{filePath}/{fileName}";
                var request = new PutObjectRequest()
                {
                    BucketName = s3Option.BucketName,
                    Key = key,
                    InputStream = stream,
                    ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
                    ServerSideEncryptionCustomerProvidedKey = s3Option.AesKey,
                };
                if (s3CannedACL != null)
                {
                    request.CannedACL = s3CannedACL;
                }

                var res = await client.PutObjectAsync(request);
                if (res.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return key;
                }
            }

            return null;
        }

        AmazonS3Client CreateClient(S3Option s3Option)
        {
            return new AmazonS3Client(
                s3Option.AccessKey,
                s3Option.AccessSecret,
                new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(s3Option.RegionName),
                    ProxyHost = s3Option.ProxyHost,
                    ProxyPort = s3Option.ProxyPort,
                });
        }
    }
}
