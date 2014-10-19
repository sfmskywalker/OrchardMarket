using DarkSky.OrchardMarket.Helpers;
using DarkSky.OrchardMarket.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;

namespace DarkSky.OrchardMarket.Services {
    public class PayPalPayoutMethod : PayoutMethod {
        public override string Name {
            get { return "PayPal"; }
        }

        public override string DisplayName {
            get { return "PayPal"; }
        }

        public override dynamic BuildEditor(SettingsDictionary settings, dynamic shapeHelper) {
            var viewModel = settings.GetModel<PayPalPayoutMethodViewModel>();
            return shapeHelper.PayoutEditor_PayPal(Model: viewModel);
        }

        public override dynamic UpdateEditor(SettingsDictionary settings, dynamic shapeHelper, IUpdateModel updater) {
            var viewModel = settings.GetModel<PayPalPayoutMethodViewModel>();

            if (updater.TryUpdateModel(viewModel, null, null, null)) {
                settings["PayPalPayoutMethodViewModel.Email"] = viewModel.Email.TrimSafe();
            }

            return BuildEditor(settings, shapeHelper);
        }
    }
}