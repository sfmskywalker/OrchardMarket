using System;
using DarkSky.Commerce.Models;
using Orchard;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Services {
    public interface IShoppingCartService : IDependency {
        IShoppingCart CreateCart();
        Guid GetCartId();
        IShoppingCart GetCart();
        IShoppingCart GetCart(int id);
        IShoppingCart GetCart(Guid guid);
    }

    [OrchardFeature("DarkSky.Commerce.ShoppingCart")]
    public class ShoppingCartService : IShoppingCartService {
        private readonly IRepository<ShoppingCartRecord> _shoppingCartRepository;
        private readonly ICookieManager _cookieManager;
        private readonly IOrchardServices _services;
        private const string ShoppingCartCookieKey = "ShoppingCartId";

        public ShoppingCartService(IRepository<ShoppingCartRecord> shoppingCartRepository, ICookieManager cookieManager, IOrchardServices services) {
            _shoppingCartRepository = shoppingCartRepository;
            _cookieManager = cookieManager;
            _services = services;
        }

        public IShoppingCart CreateCart() {
            var record = ShoppingCartRecord.New();
            var cart = ResolveCart(record);
            _shoppingCartRepository.Create(record);
            _cookieManager.SetValue(ShoppingCartCookieKey, cart.Guid);
            return cart;
        }

        public Guid GetCartId() {
            return _cookieManager.GetValue<Guid?>(ShoppingCartCookieKey) ?? Guid.Empty;
        }

        public IShoppingCart GetCart() {
            var cartId = GetCartId();
            return (cartId != Guid.Empty ? (GetCart(cartId) ?? CreateCart()) : default(IShoppingCart)) ?? CreateCart();
        }

        public IShoppingCart GetCart(int id) {
            return ResolveCart(_shoppingCartRepository.Get(id));
        }

        public IShoppingCart GetCart(Guid guid) {
            return ResolveCart(_shoppingCartRepository.Get(x => x.Guid == guid && !x.IsClosed));
        }

        private IShoppingCart ResolveCart(ShoppingCartRecord record) {
            if (record == null) return null;
            var cart = _services.WorkContext.Resolve<IShoppingCart>();
            cart.Initialize(record);
            return cart;
        }
    }
}