using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rebex.Security.Certificates;
using Rebex.Security.Cryptography;
using Rebex.Security.Cryptography.Pkcs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SimpleAuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertController : ControllerBase
    {
        ILogger<CertController> _logger;
        public CertController(ILogger<CertController> logger)
        {
            _logger = logger;
        }

        [HttpGet("ping")]
        public string GetPing()
        {
            return "Ping";
        }

        // GET: api/<CertController>
        [HttpGet]
        public byte[] Get()
        {
            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Certificates\\DevCertRootCA.pfx");

                Certificate ca = Certificate.LoadPfx(path, "", KeySetOptions.MachineKeySet);

                // prepare certificate info
                var info = new CertificateInfo();

                // specify certificate validity range
                info.EffectiveDate = DateTime.Now.AddDays(-1);
                info.ExpirationDate = info.EffectiveDate.AddYears(1);

                // specify certificate subject for a client certificate
                info.Subject = new DistinguishedName("CN=Sample Certificate");

                // specify certificate usage for a client certificate
                info.Usage = KeyUses.DigitalSignature | KeyUses.KeyEncipherment | KeyUses.DataEncipherment;

                // specify certificate extended usage for a client certificate
                info.SetExtendedUsage(ExtendedUsageOids.ClientAuthentication, ExtendedUsageOids.EmailProtection);

                // sets a unique serial number
                info.SetSerialNumber(Guid.NewGuid().ToByteArray());

                // use SHA-256 signature algorithm
                info.SignatureHashAlgorithm = HashingAlgorithmId.SHA256;

                // generate a 2048-bit RSA key for the certificate
                PrivateKeyInfo privateKey;
                using (var alg = new AsymmetricKeyAlgorithm())
                {
                    alg.GenerateKey(AsymmetricKeyAlgorithmId.RSA, 2048);
                    privateKey = alg.GetPrivateKey();
                }

                // create the certificate signed by the CA certificate
                PublicKeyInfo publicKey = privateKey.GetPublicKey();
                Certificate certificate = CertificateIssuer.Issue(ca, info, publicKey);

                // associate the private key with the certificate
                certificate.Associate(privateKey);

                using (CertificateStore store = new CertificateStore(CertificateStoreName.My, CertificateStoreLocation.LocalMachine))
                {
                    store.Add(certificate);
                }

                using (CertificateStore store = new CertificateStore(CertificateStoreName.TrustedPeople, CertificateStoreLocation.LocalMachine))
                {
                    store.Add(certificate);
                }

                var memoryStream = new MemoryStream();
                certificate.Save(memoryStream, CertificateFormat.Pfx);
                return memoryStream.ToArray();
            } catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }
    }
}
