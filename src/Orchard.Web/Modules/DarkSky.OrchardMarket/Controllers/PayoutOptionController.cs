using System.Linq;
using System.Web.Mvc;
using DarkSky.OrchardMarket.Models;
using DarkSky.OrchardMarket.Services;
using DarkSky.OrchardMarket.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Themes;
using Orchard.UI.Notify;

namespace DarkSky.OrchardMarket.Controllers {
    [Authorize]
	public class PayoutOptionController : ControllerBase, IUpdateModel {
        private readonly IOrchardServices _services;
        private readonly IPayoutService _payoutService;
        private readonly INotifier _notifier;
        protected dynamic New { get; set; }

        public PayoutOptionController(IOrchardServices services, IPayoutService payoutService) {
            _services = services;
            _payoutService = payoutService;
            _notifier = _services.Notifier;
            New = _services.New;
        }

        [Themed]
        public ActionResult Index(int organizationId) {
            var payoutOptions = _payoutService.GetPayoutOptions(organizationId).ToList();
            var shape = New.ViewModel(
                OrganizationId: organizationId,
                PayoutOptions: payoutOptions.Select(x => New.PaymentOption(
                    Id: x.Id,
                    PayoutMethodName: x.PayoutMethodName,
                    IsActive: x.IsActive)).ToList());
            return View(shape);
        }

        [Themed]
        public ActionResult Add(int organizationId) {
            return View(new AddPayoutOptionViewModel{ OrganizationId = organizationId });
        }

        [Themed, HttpPost]
        public ActionResult Add(AddPayoutOptionViewModel viewModel) {
            
            if (!ModelState.IsValid)
                return View(viewModel);

            var payoutOption = _payoutService.CreatePayoutOption(viewModel.OrganizationId, viewModel.PayoutMethodName);
            return RedirectToAction("Edit", new { id = payoutOption.Id });
        }

        [Themed]
        public ActionResult Edit(int id) {
            var payoutOption = _payoutService.GetPayoutOption(id);
            var payoutMethod = _payoutService.GetPayoutMethod(payoutOption.PayoutMethodName);
            var payoutData = _payoutService.DeserializePayoutData(payoutOption.Data);
            var editorShape = payoutMethod.BuildEditor(payoutData, New) ?? New.PayoutEditor_Default();
            var shape = New.ViewModel(EditorShape: editorShape, ViewModel: new EditPayoutOptionViewModel { Id = id, IsActive = payoutOption.IsActive});
            return View(shape);
        }

        [Themed, HttpPost]
        public ActionResult Edit(EditPayoutOptionViewModel viewModel) {
            var payoutOption = _payoutService.GetPayoutOption(viewModel.Id);
            var payoutMethod = _payoutService.GetPayoutMethod(payoutOption.PayoutMethodName);
            var payoutData = _payoutService.DeserializePayoutData(payoutOption.Data);
            var editorShape = payoutMethod.UpdateEditor(payoutData, New, this) ?? New.PayoutEditor_Default();

            if (!ModelState.IsValid) {
                var shape = New.ViewModel(EditorShape: editorShape, ViewModel: viewModel);
                return View(shape);
            }

            payoutOption.Data = _payoutService.SerializePayoutData(payoutData);

            if (viewModel.IsActive) {
                _payoutService.SetActivePayoutOption(payoutOption);
            }
            else {
                payoutOption.IsActive = false;
            }
            _notifier.Information(T("Your payout option has been updated."));
            return RedirectToAction("Index", new { organizationId = payoutOption.OrganizationId });
        }

        public ActionResult Activate(int id, string returnUrl) {
            var payoutOption = _payoutService.GetPayoutOption(id);
            _payoutService.SetActivePayoutOption(payoutOption);

            _notifier.Information(T("Your payout option has been activated."));
            returnUrl = !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Action("Index");
            return Redirect(returnUrl);
        }

        public ActionResult Delete(int id) {
            var payoutOption = _payoutService.GetPayoutOption(id);
            _payoutService.Delete(payoutOption);

            _notifier.Information(T("Your payout option has been removed."));
            return RedirectToAction("Index");
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.Text);
        }
    }
}