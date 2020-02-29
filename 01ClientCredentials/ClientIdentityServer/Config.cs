using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientIdentityServer
{
    public static class Config
    {
        /// <summary>
        /// 资源下面关联可以访问用户的某些信息标志，如claim声明了name等
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1","mylouie")
            };
        }

        /// <summary>
        /// 应用程序注册在IDS身份认证服务器上的注册信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId="client",
                    AllowedGrantTypes=GrantTypes.ClientCredentials,//客户端模式
                    ClientSecrets=
                    {
                        new Secret("louieclient".Sha256())
                    },
                    AllowedScopes={ "api1" } //此应用能获取用户的数据范围
                }
            };
        }
    }
}
