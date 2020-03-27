using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Core
{
    public class AWSS3Options
    {
        public string AesKey { get; set; }
        public string RegionName { get; set; }
        public string BucketName { get; set; }
        public string AccessKey { get; set; }
        public string AccessSecret { get; set; }
        public string ProxyHost { get; set; }
        public int ProxyPort { get; set; }
        public string RootPath { get; set; }
    }

    public class EsignS3Options: AWSS3Options
    {

    }

    public class S3Options : Dictionary<string, S3Option>
    {
    }
    public class S3Option
    {
        public string RegionName { get; set; }
        public string BucketName { get; set; }
        public string AccessKey { get; set; }
        public string AccessSecret { get; set; }
        public string ProxyHost { get; set; }
        public int ProxyPort { get; set; }
        public string AesKey { get; set; }
    }
}
