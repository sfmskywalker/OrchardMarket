using DarkSky.OrchardMarket.Models;
using Orchard.Commands;
using Orchard.ContentManagement;

namespace DarkSky.OrchardMarket.Commands {
    public class ContentCommands : DefaultOrchardCommandHandler {
        private readonly IContentManager _contentManager;

        public ContentCommands(IContentManager contentManager) {
            _contentManager = contentManager;
        }

    }
}