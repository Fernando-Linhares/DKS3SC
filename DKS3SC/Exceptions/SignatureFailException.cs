using iTextSharp.text.pdf.security;
using Certificate = System.Security.Cryptography.X509Certificates.X509Certificate2;
using BouncyCert = Org.BouncyCastle.X509.X509Certificate;
using Parser = Org.BouncyCastle.X509.X509CertificateParser;

namespace DKS3SC.Exceptions
{
    public class SignatureFailException : Exception
    {
        public SignatureFailException(Certificate certificate): base(GetMessageByCertificate(certificate))
        {}

        public static string GetNameByCertificate(Certificate certificate)
        {
            Parser parser = new Parser();

            BouncyCert certBouncy = parser.ReadCertificate(certificate.RawData);

            return CertificateInfo.GetSubjectFields(certBouncy).GetField("CN");
        }

        public static string GetMessageByCertificate(Certificate certificate)
        {
            string name = GetNameByCertificate(certificate);

            return $"Error on Sign data. Not found private key RSA in certificate selected. Certificate - {name}";
        }
    }
}
