using System.Collections.Generic;
using System.Linq;
using DarkSky.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Security;
using Orchard.Services;

namespace DarkSky.Commerce.Services {
    public interface IOrderManager : IDependency {
        OrderPart CreateOrder(IUser user, IShopAspect shop = null, Transaction transaction = null);
        IEnumerable<OrderPart> CreateOrders(IUser user, IShoppingCart cart, Transaction transaction = null);
        void CompleteOrder(OrderPart order);
        void CancelOrder(OrderPart order);
        IEnumerable<OrderDetail> GetDetails(int orderId);
        IEnumerable<OrderPart> GetOrdersByShop(int shopId);
        IEnumerable<OrderPart> GetOrdersByUser(int userId);
    }

    [OrchardFeature("DarkSky.Commerce.Orders")]
    public class OrderManager : IOrderManager {
        private readonly IContentManager _contentManager;
        private readonly IClock _clock;
        private readonly IRepository<OrderDetail> _orderDetailRepository;

        public OrderManager(IContentManager contentManager, IClock clock, IRepository<OrderDetail> orderDetailRepository) {
            _contentManager = contentManager;
            _clock = clock;
            _orderDetailRepository = orderDetailRepository;
        }

        public OrderPart CreateOrder(IUser user, IShopAspect shop = null, Transaction transaction = null) {
            var order = _contentManager.Create<OrderPart>("Order");
            order.User = user;
            order.Shop = shop;
            order.Transaction = transaction;
            return order;
        }

        public IEnumerable<OrderPart> CreateOrders(IUser user, IShoppingCart cart, Transaction transaction = null) {
            var groupedItems = cart.GroupItemsByShop();
            foreach (var item in groupedItems) {
                var shop = item.Key;
                var shopItems = item.ToList();
                var order = CreateOrder(user, shop, transaction);
                
                foreach (var shopItem in shopItems) {
                    CreateOrderDetail(order, shopItem);
                }

                yield return order;
            }

            cart.Close();
        }

        public void CompleteOrder(OrderPart order) {
            order.Status = OrderStatus.Completed;
            order.CompletedUtc = _clock.UtcNow;
        }

        public void CancelOrder(OrderPart order) {
            order.Status = OrderStatus.Cancelled;
            order.CompletedUtc = _clock.UtcNow;
        }

        public IEnumerable<OrderDetail> GetDetails(int orderId) {
            return _orderDetailRepository.Fetch(x => x.OrderId == orderId);
        }

        public IEnumerable<OrderPart> GetOrdersByShop(int shopId) {
            return _contentManager.Query<OrderPart>().Join<CommonPartRecord>().Where(x => x.Container.Id == shopId).List();
        }

        public IEnumerable<OrderPart> GetOrdersByUser(int userId) {
            return _contentManager.Query<OrderPart>().Join<CommonPartRecord>().Where(x => x.OwnerId == userId).List();
        }

        private OrderDetail CreateOrderDetail(OrderPart order, ProductQuantity cartItem) {
            var product = cartItem.Product;
            var orderDetail = new OrderDetail {
                OrderId = order.Id,
                ProductId = product.Id,
                Quantity = cartItem.Quantity,
                UnitPrice = product.UnitPrice,
                VatRate = product.Vat.Rate
            };
            _orderDetailRepository.Create(orderDetail);
            return orderDetail;
        }
    }
}