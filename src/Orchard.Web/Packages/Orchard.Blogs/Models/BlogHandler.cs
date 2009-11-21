using Orchard.Data;
using Orchard.Models.Driver;

namespace Orchard.Blogs.Models {
    public class BlogHandler : ContentHandler {
        public BlogHandler(IRepository<BlogRecord> repository) {
            Filters.Add(new ActivatingFilter<Blog>("blog"));
            Filters.Add(new StorageFilterForRecord<BlogRecord>(repository));
        }
    }
}