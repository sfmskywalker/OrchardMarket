using System.Collections.Generic;
using System.Linq;
using DarkSky.Commerce.EventHandlers;
using DarkSky.Commerce.Exceptions;
using DarkSky.Commerce.Models;
using DarkSky.Commerce.PaymentProviders;
using Orchard;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Security;
using Orchard.Services;

namespace DarkSky.Commerce.Services {
    public interface IPaymentService : IDependency {
        IPaymentProvider GetPaymentProviderByName(string name);
        Transaction CreateTransaction(IUser user, IShoppingCart cart, IPaymentProvider provider);
        Transaction GetTransactionByProviderToken(string token);
        Payment CreatePayment(CreatePaymentArgs args);
        bool PaymentExists(int transactionId, string paymentToken);
        void CancelTransaction(int id);
        Transaction GetTransaction(int id);
    }

    [OrchardFeature("DarkSky.Commerce.PaymentProviders")]
    public class PaymentService : IPaymentService {
        private readonly IEnumerable<IPaymentProvider> _providers;
        private readonly IClock _clock;
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly ITransactionHandler _transactionHandler;

        public PaymentService(IEnumerable<IPaymentProvider> providers, IClock clock, IRepository<Transaction> transactionRepository, IRepository<Payment> paymentRepository, ITransactionHandler transactionHandler) {
            _providers = providers;
            _clock = clock;
            _transactionRepository = transactionRepository;
            _paymentRepository = paymentRepository;
            _transactionHandler = transactionHandler;
        }

        public IPaymentProvider GetPaymentProviderByName(string name) {
            return _providers.FirstOrDefault(x => x.Name == name);
        }

        public Transaction CreateTransaction(IUser user, IShoppingCart cart, IPaymentProvider provider) {
            var transaction = new Transaction {
                UserId = user.Id,
                CartId = cart.Id,
                CreatedUtc = _clock.UtcNow,
                OrderTotal = cart.Totals().Total,
                Status = TransactionStatus.New,
                PaymentMethodName = provider.DisplayName
            };

            _transactionRepository.Create(transaction);
            return transaction;
        }

        public Transaction GetTransactionByProviderToken(string token) {
            return _transactionRepository.Get(x => x.ProviderToken == token);
        }

        public Payment CreatePayment(CreatePaymentArgs args) {
            if(PaymentExists(args.Transaction.Id, args.PaymentToken))
                throw new DuplicatePaymentException("There is already a payment registered for the specified transaction and payment token");

            var payment = new Payment {
                CreatedUtc = _clock.UtcNow,
                Token = args.PaymentToken,
                Transaction = args.Transaction
            };
            _paymentRepository.Create(payment);
            _transactionHandler.PaymentCreated(new PaymentContext { Payment = payment});
            return payment;
        }

        public Payment GetPayment(int transactionId, string paymentToken) {
            return _paymentRepository.Get(x => x.Transaction.Id == transactionId && x.Token == paymentToken);
        }

        public bool PaymentExists(int transactionId, string paymentToken) {
            return GetPayment(transactionId, paymentToken) != null;
        }

        public void CancelTransaction(int id) {
            var transaction = GetTransaction(id);

            transaction.Status = TransactionStatus.Cancelled;
            transaction.CompletedUtc = _clock.UtcNow;
        }

        public Transaction GetTransaction(int id) {
            return _transactionRepository.Get(id);
        }
    }

    public class CreatePaymentArgs {
        public Transaction Transaction { get; set; }
        public string PaymentToken { get; set; }
    }
}