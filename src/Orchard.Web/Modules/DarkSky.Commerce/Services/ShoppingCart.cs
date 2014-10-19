using System;
using System.Collections.Generic;
using System.Linq;
using DarkSky.Commerce.Helpers;
using DarkSky.Commerce.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Services;
using Orchard.Utility.Extensions;

namespace DarkSky.Commerce.Services {
    public interface IShoppingCart : ITransientDependency {
        int Id { get; }
        Guid Guid { get; }
        DateTime CreatedUtc { get; }
        IEnumerable<ShoppingCartItem> Items { get; }
        bool IsClosed { get; }
        void Initialize(ShoppingCartRecord record);
        void Add(ShoppingCartItem item);
        ShoppingCartItem Add(int productId, int quantity = 1);
        ShoppingCartItem GetItem(int id);
        ShoppingCartItem Remove(int id);
        IEnumerable<ProductQuantity> GetProducts();
        Totals Totals();
        IEnumerable<IGrouping<IShopAspect, ProductQuantity>> GroupItemsByShop();
        void Close();
    }

    [OrchardFeature("DarkSky.Commerce.ShoppingCart")]
    public class ShoppingCart : IShoppingCart {
        private ShoppingCartRecord _record;
        private readonly IShoppingCartSettingsAccessor _settings;
        private readonly IRepository<ShoppingCartItem> _shoppingCartItemRepository;
        private readonly IProductManager _productManager;
        private readonly IClock _clock;
        public int Id { get { return _record.Id; } }
        public Guid Guid { get { return _record.Guid; } }
        public DateTime CreatedUtc { get { return _record.CreatedUtc; } }
        public IEnumerable<ShoppingCartItem> Items { get { return _record.Items.ToReadOnlyCollection(); } }
        
        public DateTime? ClosedUtc {
            get { return _record.ClosedUtc; }
            set { _record.ClosedUtc = value; }
        }
        
        public bool IsClosed {
            get { return _record.IsClosed; }
            private set { _record.IsClosed = value; }
        }

        public ShoppingCart(IShoppingCartSettingsAccessor settings, IRepository<ShoppingCartItem> shoppingCartItemRepository, IProductManager productManager, IClock clock) {
            _settings = settings;
            _shoppingCartItemRepository = shoppingCartItemRepository;
            _productManager = productManager;
            _clock = clock;
        }

        public void Initialize(ShoppingCartRecord record) {
            _record = record;
        }

        public ShoppingCartItem Add(int productId, int quantity = 1) {
            var item = new ShoppingCartItem {ProductId = productId, Quantity = quantity};
            Add(item);
            return item;
        }

        public ShoppingCartItem GetItem(int id) {
            return Items.SingleOrDefault(x => x.Id == id);
        }

        public ShoppingCartItem Remove(int id) {
            var item = GetItem(id);

            if(item != null) {
                _shoppingCartItemRepository.Delete(item);
                _shoppingCartItemRepository.Flush();
            }
            
            return item;
        }

        public IEnumerable<ProductQuantity> GetProducts() {
            return from item in Items
                   let products = _productManager.GetProducts(Items.Select(x => x.ProductId), VersionOptions.Published).ToList()
                   let product = products.SingleOrDefault(x => x.Id == item.ProductId)
                   where product != null
                   select new ProductQuantity {Product = product, Quantity = item.Quantity};
        }

        public Totals Totals() {
            return GetProducts().Select(x => _productManager.CalculateItemTotals(x)).Sum();
        }

        public IEnumerable<IGrouping<IShopAspect, ProductQuantity>> GroupItemsByShop() {
            var products = GetProducts();
            var q = from item in Items
                    let product = products.Single(x => x.Product.Id == item.ProductId)
                    let shop = product.Product.Shop
                    group product by shop;

            return q;
        }

        public void Close() {
            IsClosed = true;
            ClosedUtc = _clock.UtcNow;
        }

        public void Add(ShoppingCartItem item) {
            if(item == null)
                throw new ArgumentNullException("item");

            var existingItem = _record.Items.SingleOrDefault(x => x.ProductId == item.ProductId);
            var quantity = existingItem == null ? item.Quantity : existingItem.Quantity + item.Quantity;

            if (quantity <= 0 || quantity > _settings.MaxQuantityPerItem) {
                throw new ArgumentOutOfRangeException("item", "Quantity must be at least 1 and at most 99");
            }

            if(existingItem != null) {
                existingItem.Quantity = quantity;
            }
            else {
                item.ShoppingCartRecord = _record;
                if (item.Id == 0)
                    _shoppingCartItemRepository.Create(item);
                _record.Items.Add(item);
            }
        }
    }
}