using System.Security.Cryptography.X509Certificates;
using BouncyCert = Org.BouncyCastle.X509.X509Certificate;
using Certificate = System.Security.Cryptography.X509Certificates.X509Certificate2;

namespace DKS3SC;

public interface IManager
{
    public Dictionary<string, string> Details();

    public void SignFile(string fileinput, string fileoutput, X509Certificate2 certificate);

    public void SignAll(List<string> filesinput, List<string> filesoutput, X509Certificate2 certificate);

    public string CertName(BouncyCert cert);

    public BouncyCert[] BuildChain(Certificate cert);

}