﻿using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Settings;
using Orchard.Tags.Models;
using Orchard.UI.Notify;

namespace Orchard.Tags.Services {
    public interface ITagService : IDependency {
        IEnumerable<Tag> GetTags();
        Tag GetTag(int id);
        Tag GetTagByName(string tagName);
        void CreateTag(string tagName);
        void DeleteTag(int id);
        void UpdateTag(int id, string tagName);
        void TagContentItem(int contentItemId, string tagName);
        void UpdateTagsForContentItem(int contentItemId, IEnumerable<int> tagsForContentItem);
    }

    public class TagService : ITagService {
        private readonly IRepository<Tag> _tagRepository;
        private readonly IRepository<TagsContentItems> _tagsContentItemsRepository;
        private readonly INotifier _notifier;

        public TagService(IRepository<Tag> tagRepository, 
                          IRepository<TagsContentItems> tagsContentItemsRepository,
                          INotifier notifier) {
            _tagRepository = tagRepository;
            _tagsContentItemsRepository = tagsContentItemsRepository;
            _notifier = notifier;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public ISite CurrentSite { get; set; }
        public Localizer T { get; set; }

        #region ITagService Members

        public IEnumerable<Tag> GetTags() {
            return from tags in _tagRepository.Table.ToList() select tags;
        }

        public Tag GetTag(int id) {
            return _tagRepository.Get(x => x.Id == id);
        }

        public Tag GetTagByName(string tagName) {
            return _tagRepository.Get(x => x.TagName == tagName);
        }

        public void CreateTag(string tagName) {
            if (_tagRepository.Get(x => x.TagName == tagName) == null) {
                Tag tag = new Tag { TagName = tagName };
                _tagRepository.Create(tag);
            }
            else {
                _notifier.Warning(T("Couldn't create tag: " + tagName + "it already exixts"));
            }
        }

        public void DeleteTag(int id) {
            _tagRepository.Delete(GetTag(id));
            IEnumerable<TagsContentItems> tagsContentItems = _tagsContentItemsRepository.Fetch(x => x.TagId == id);
            foreach (var tagContentItem in tagsContentItems) {
                _tagsContentItemsRepository.Delete(tagContentItem);
            }
        }

        public void UpdateTag(int id, string tagName) {
            Tag tag = _tagRepository.Get(id);
            if (String.IsNullOrEmpty(tagName)) {
                _notifier.Warning(T("Couldn't rename tag: name was empty"));
                return;
            }
            tag.TagName = tagName;
        }

        public void TagContentItem(int contentItemId, string tagName) {
            Tag tag = GetTagByName(tagName);
            TagsContentItems tagsContentItems = new TagsContentItems { ContentItemId = contentItemId, TagId = tag.Id };
            _tagsContentItemsRepository.Create(tagsContentItems);
        }

        public void UpdateTagsForContentItem(int contentItemId, IEnumerable<int> tagsForContentItem) {
            List<int> newTagsForContentItem = new List<int>(tagsForContentItem);
            IEnumerable<TagsContentItems> currentTagsForContentItem = _tagsContentItemsRepository.Fetch(x => x.ContentItemId == contentItemId);
            foreach (var tagContentItem in currentTagsForContentItem) {
                if (!newTagsForContentItem.Contains(tagContentItem.TagId)) {
                    _tagsContentItemsRepository.Delete(tagContentItem);
                }
                else {
                    newTagsForContentItem.Remove(tagContentItem.TagId);
                }
            }
            foreach (var newTagForContentItem in newTagsForContentItem) {
                _tagsContentItemsRepository.Create(new TagsContentItems { ContentItemId = contentItemId, TagId = newTagForContentItem });
            }
        }

        #endregion
    }
}