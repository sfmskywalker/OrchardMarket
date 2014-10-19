using System.IO;
using System.Web;
using Orchard;
using Orchard.FileSystems.Media;
using DarkSky.OrchardMarket.Models;

namespace DarkSky.OrchardMarket.Services {
    public interface IUserProfileManager : IDependency {
        void UpdateLogo(HttpPostedFileBase file, UserProfilePart user);
        string GetAvatarUrl(UserProfilePart user);
    }

    public class UserProfileManager : IUserProfileManager {
        private readonly IStorageProvider _storageProvider;

        public UserProfileManager(IStorageProvider storageProvider) {
            _storageProvider = storageProvider;
        }

        public void UpdateLogo(HttpPostedFileBase file, UserProfilePart user) {

            var folder = string.Format("Users\\{0}", user.Id);
            var fileName = string.Format("logo{0}", Path.GetExtension(file.FileName));
            var path = _storageProvider.Combine(folder, fileName);
            _storageProvider.TryCreateFolder(folder);

            try {
                _storageProvider.DeleteFile(path);
            }
            catch { }

            _storageProvider.SaveStream(path, file.InputStream);
            user.AvatarUrl = path;
        }

        public string GetAvatarUrl(UserProfilePart user) {
            return !string.IsNullOrWhiteSpace(user.AvatarUrl) ? string.Format(_storageProvider.GetPublicUrl(user.AvatarUrl)) : null;
        }
    }

}