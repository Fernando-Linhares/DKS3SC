using iTextSharp.text.pdf;
using iTextSharp.text;

namespace DKS3SC
{
    public class SignatureDetails : ISignatureDetails
    {
        public string? Reason { get; set; }

        public string? Location { get; set; }

        public string? Content { get; set; }

        public bool? HasLayers { get; set; }

        public bool Visible { get => true; }

        public void Apply(PdfSignatureAppearance apparence)
        {
            apparence.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.DESCRIPTION;
            apparence.SetVisibleSignature(new Rectangle(100, 100, 300, 200), 1, "Signature");
        }
    }
}
