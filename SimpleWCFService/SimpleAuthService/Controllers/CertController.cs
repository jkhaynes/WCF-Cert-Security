using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SimpleAuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertController : ControllerBase
    {
        // GET: api/<CertController>
        [HttpGet]
        public byte[] Get()
        {
            //var path = Path.Combine(Directory.GetCurrentDirectory(), "Certificates\\ClientPrivate2.pfx");
            //var cert = new X509Certificate2(path, "password");
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
            var cert = collection.Find(X509FindType.FindBySubjectName, "ClientCert", true);
            var bytes = cert.Export(X509ContentType.Pkcs12);
            return bytes;
        }
    }
}
