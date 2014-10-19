using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Core.Title.Models;

namespace DarkSky.Commerce.Models {

    public class VatPart : ContentPart<VatPartRecord> {
        public float Rate {
            get { return Record.Rate; }
            set { Record.Rate = value; }
        }

        public bool IsActive {
            get { return Record.IsActive; }
            set { Record.IsActive = value; }
        }

        public string Description{
            get { return Record.Description; }
            set { Record.Description = value; }
        }

        public string Name {
            get { return this.As<TitlePart>().Title; }
            set { this.As<TitlePart>().Title = value; }
        }
    }

    public class VatPartRecord : ContentPartRecord {
        public virtual float Rate { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual string Description { get; set; }
    }
}