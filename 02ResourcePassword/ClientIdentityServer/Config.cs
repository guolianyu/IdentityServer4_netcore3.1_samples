using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
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
                    AllowedGrantTypes=GrantTypes.ResourceOwnerPassword,//账号密码
                    ClientSecrets=
                    {
                        new Secret("louieclient".Sha256())
                    },
                    AllowedScopes={ "api1" } //此应用能获取用户的数据范围
                }
            };
        }
        /// <summary>
        /// 用户数据，正常我们会在认证服务中心去连接用户服务
        /// </summary>
        /// <returns></returns>
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "password"
                },
                 new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password"
                }
            };
        }
    }
}
