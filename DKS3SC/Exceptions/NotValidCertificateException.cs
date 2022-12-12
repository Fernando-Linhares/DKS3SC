using iTextSharp.text.pdf.security;
using Certificate = System.Security.Cryptography.X509Certificates.X509Certificate2;
using BouncyCert = Org.BouncyCastle.X509.X509Certificate;
using Parser = Org.BouncyCastle.X509.X509CertificateParser;

namespace DKS3SC.Exceptions
{
    public class NotValidCertificateException : Exception
    {
        public NotValidCertificateException(Certificate certificate) : base(GetMessageByCertificate(certificate))
        {}

        public static  string GetMessageByCertificate(Certificate certificate)
        {
            string certname = GetNameByCertificate(certificate);

            return $"Not valid certificate please, check his preferencies - Certificate Name  {certname}";
        }

        public static string GetNameByCertificate(Certificate certificate)
        {
            Parser parser = new Parser();

            BouncyCert certBouncy = parser.ReadCertificate(certificate.RawData);

            return CertificateInfo.GetSubjectFields(certBouncy).GetField("CN");
        }
    }
}
