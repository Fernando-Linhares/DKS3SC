using iTextSharp.text.pdf;

namespace DKS3SC
{
    public class SignatureDetails : ISignatureDetails
    {
        public string? Reason { get; set; }

        public string? Location { get; set; }

        public string? Content { get; set; }

        public bool? HasLayers { get; set; }

        public bool HasRenderMode { get => true; }

        public void AddRender(PdfSignatureAppearance appearance)
        {
            appearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.DESCRIPTION;
        }
    }
}
