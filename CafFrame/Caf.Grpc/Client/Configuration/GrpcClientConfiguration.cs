using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.Grpc.Client.Configuration
{
    internal class GrpcClientConfiguration: IGrpcClientConfiguration
    {
        /// <summary>
        /// 直连模式需要进行的配置
        /// </summary>
        public GrpcDirectConnectionConfiguration GrpcDirectConnectionConfiguration { get; set; }

        public GrpcClientConfiguration()
        {
            GrpcDirectConnectionConfiguration = new GrpcDirectConnectionConfiguration();
        }
    }
}
