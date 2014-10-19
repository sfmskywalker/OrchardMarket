using DarkSky.OrchardMarket.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Tokens;

namespace DarkSky.OrchardMarket.Providers.Tokens {
    public class PackageTokens : Component, ITokenProvider {

        public void Describe(DescribeContext context) {
            context.For("Package", T("Packages"), T("Packages"))
                   .Token("Content", T("Content"), T("The package as a content item", "Content"))
                   .Token("ExtensionType", T("ExtensionType"), T("ExtensionType"), "User");
        }

        public void Evaluate(EvaluateContext context) {
            context.For("Package", () => ((IContent)context.Data["Content"]).As<PackagePart>())
                .Chain("Content", "Content", part => part.ContentItem)
                .Token("ExtensionType", part => part.ExtensionType);
        }
    }
}