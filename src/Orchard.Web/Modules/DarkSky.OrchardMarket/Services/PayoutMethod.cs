using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;

namespace DarkSky.OrchardMarket.Services {
    public interface IPayoutMethod : IDependency {
        string Name { get; }
        string DisplayName { get; }
        dynamic BuildEditor(SettingsDictionary settings, dynamic shapeHelper);
        dynamic UpdateEditor(SettingsDictionary settings, dynamic shapeHelper, IUpdateModel updater);
    }

    public abstract class PayoutMethod : IPayoutMethod {
        public abstract string Name { get; }
        public abstract string DisplayName { get; }

        public virtual dynamic BuildEditor(SettingsDictionary settings, dynamic shapeHelper) {
            return null;
        }

        public virtual dynamic UpdateEditor(SettingsDictionary settings, dynamic shapeHelper, IUpdateModel updater) {
            return BuildEditor(settings, shapeHelper);
        }
    }
}