using iTextSharp.text.pdf;


namespace DKS3SC.Dependencies
{
    public class Apparence : PdfSignatureAppearance
    {
        public Apparence(PdfStamperImp writer) : base(writer)
        {
        }
    }
}
