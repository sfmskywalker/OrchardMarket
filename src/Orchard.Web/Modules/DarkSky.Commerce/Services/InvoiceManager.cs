using System.Collections.Generic;
using System.Linq;
using DarkSky.Commerce.EventHandlers;
using DarkSky.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Security;
using Orchard.Services;

namespace DarkSky.Commerce.Services {
    public interface IInvoiceManager : IDependency {
        InvoicePart CreateInvoice(IUser user);
        InvoicePart CreateInvoice(IUser user, OrderPart order);
        IEnumerable<InvoicePart> CreateInvoices(IUser user, IEnumerable<OrderPart> orders);
        void PayInvoice(InvoicePart invoice, Payment payment);
        void PayInvoices(IEnumerable<InvoicePart> invoices, Payment payment);
        void CancelInvoice(InvoicePart invoice);
        IEnumerable<InvoiceDetail> GetDetails(int invoiceId);
        IEnumerable<InvoicePart> GetInvoices(IUser user);
        IEnumerable<InvoicePart> GetInvoicesByTransaction(int transactionId);
    }

    [OrchardFeature("DarkSky.Commerce.Orders")]
    public class InvoiceManager : IInvoiceManager {
        private readonly IContentManager _contentManager;
        private readonly IClock _clock;
        private readonly IProductManager _productManager;
        private readonly IRepository<InvoiceDetail> _invoiceDetailRepository;
        private readonly IInvoiceEventHandler _invoiceEventHandler;

        public InvoiceManager(IContentManager contentManager, IClock clock, IProductManager productManager, IRepository<InvoiceDetail> invoiceDetailRepository, IInvoiceEventHandler invoiceEventHandler) {
            _contentManager = contentManager;
            _clock = clock;
            _productManager = productManager;
            _invoiceDetailRepository = invoiceDetailRepository;
            _invoiceEventHandler = invoiceEventHandler;
        }

        public InvoicePart CreateInvoice(IUser user) {
            var invoice = _contentManager.Create<InvoicePart>("Invoice");
            invoice.As<CommonPart>().Owner = user;
            return invoice;
        }

        public InvoicePart CreateInvoice(IUser user, OrderPart order) {
            var invoice = CreateInvoice(user);

            invoice.Order = order;
            invoice.Transaction = order.Transaction;
            invoice.Shop = order.Shop;

            foreach (var orderDetail in order.Details) {
                CreateInvoiceDetail(orderDetail, invoice);
            }

            return invoice;
        }

        public IEnumerable<InvoicePart> CreateInvoices(IUser user, IEnumerable<OrderPart> orders) {
            return orders.Select(order => CreateInvoice(user, order));
        }

        private void CreateInvoiceDetail(OrderDetail orderDetail, InvoicePart invoice) {
            var product = _productManager.GetProduct(orderDetail.ProductId);
            var invoiceDetail = new InvoiceDetail {
                InvoiceId = invoice.Id,
                ProductId = orderDetail.ProductId,
                Quantity = orderDetail.Quantity,
                UnitPrice = orderDetail.UnitPrice,
                VatRate = orderDetail.VatRate,
                Description = _contentManager.GetItemMetadata(product).DisplayText
            };
            _invoiceDetailRepository.Create(invoiceDetail);
        }

        public void PayInvoice(InvoicePart invoice, Payment payment) {
            var previousStatus = invoice.Status;
            invoice.Payment = payment;
            invoice.CompletedUtc = _clock.UtcNow;
            invoice.Status = InvoiceStatus.Paid;

            _invoiceEventHandler.StatusChanged(new InvoiceStatusChangedContext {
                Invoice = invoice,
                NewStatus = invoice.Status,
                PreviousStatus = previousStatus
            });
        }

        public void PayInvoices(IEnumerable<InvoicePart> invoices, Payment payment) {
            foreach (var invoice in invoices) {
                PayInvoice(invoice, payment);
            }
        }

        public void CancelInvoice(InvoicePart invoice) {
            invoice.Status = InvoiceStatus.Cancelled;
            invoice.CompletedUtc = _clock.UtcNow;
        }

        public IEnumerable<InvoiceDetail> GetDetails(int invoiceId) {
            return _invoiceDetailRepository.Fetch(x => x.InvoiceId == invoiceId);
        }

        public IEnumerable<InvoicePart> GetInvoices(IUser user) {
            return _contentManager.Query<InvoicePart>("Invoice").Join<CommonPartRecord>().Where(x => x.OwnerId == user.Id).List();
        }

        public IEnumerable<InvoicePart> GetInvoicesByTransaction(int transactionId) {
            return _contentManager.Query<InvoicePart, InvoicePartRecord>().Where(x => x.Transaction.Id == transactionId).List();
        }
    }
}