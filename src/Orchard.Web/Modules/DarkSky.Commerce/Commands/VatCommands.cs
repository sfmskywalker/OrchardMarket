using DarkSky.Commerce.Services;
using Orchard.Commands;

namespace DarkSky.Commerce.Commands {
    public class VatCommands : DefaultOrchardCommandHandler {
        private readonly IProductManager _productManager;
        public VatCommands(IProductManager productManager) {
            _productManager = productManager;
        }

        [OrchardSwitch]
        public string Description { get; set; }

        [CommandName("vat create")]
        [CommandHelp("vat create <rate> <title> /Description:<description> /Active:<active>")]
        [OrchardSwitches("Description")]
        public void CreateVat(float rate, string title) {
            _productManager.CreateVat(rate/100f, title, Description);
        }
    }
}