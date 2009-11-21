﻿using System;
using Autofac;
using Autofac.Builder;
using Autofac.Modules;
using NHibernate;
using NUnit.Framework;
using Orchard.Data;
using Orchard.Models;
using Orchard.Models.Driver;
using Orchard.Models.Records;
using Orchard.Tests.Models.Stubs;

namespace Orchard.Tests.Models {
    [TestFixture]
    public class DefaultModelManagerTests {
        private IContainer _container;
        private IContentManager _manager;
        private ISessionFactory _sessionFactory;
        private ISession _session;

        [TestFixtureSetUp]
        public void InitFixture() {
            var databaseFileName = System.IO.Path.GetTempFileName();
            _sessionFactory = DataUtility.CreateSessionFactory(
                databaseFileName,
                typeof(GammaRecord),
                typeof(ContentItemRecord),
                typeof(ContentTypeRecord));
        }

        [TestFixtureTearDown]
        public void TermFixture() {

        }

        [SetUp]
        public void Init() {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ImplicitCollectionSupportModule());
            builder.Register<DefaultContentManager>().As<IContentManager>();
            builder.Register<AlphaDriver>().As<IContentHandler>();
            builder.Register<BetaDriver>().As<IContentHandler>();
            builder.Register<GammaDriver>().As<IContentHandler>();
            builder.Register<FlavoredDriver>().As<IContentHandler>();
            builder.Register<StyledDriver>().As<IContentHandler>();

            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));

            _session = _sessionFactory.OpenSession();
            builder.Register(new TestSessionLocator(_session)).As<ISessionLocator>();

            _container = builder.Build();
            _manager = _container.Resolve<IContentManager>();
        }

        public class TestSessionLocator : ISessionLocator {
            private readonly ISession _session;

            public TestSessionLocator(ISession session) {
                _session = session;
            }

            public ISession For(Type entityType) {
                return _session;
            }
        }

        [Test]
        public void AlphaDriverShouldWeldItsPart() {
            var foo = _manager.New("alpha");

            Assert.That(foo.Is<Alpha>(), Is.True);
            Assert.That(foo.As<Alpha>(), Is.Not.Null);
            Assert.That(foo.Is<Beta>(), Is.False);
            Assert.That(foo.As<Beta>(), Is.Null);
        }

        [Test]
        public void StronglyTypedNewShouldTypeCast() {
            var foo = _manager.New<Alpha>("alpha");
            Assert.That(foo, Is.Not.Null);
            Assert.That(foo.GetType(), Is.EqualTo(typeof(Alpha)));
        }

        [Test, ExpectedException(typeof(InvalidCastException))]
        public void StronglyTypedNewShouldThrowCastExceptionIfNull() {
            _manager.New<Beta>("alpha");
        }

        [Test]
        public void AlphaIsFlavoredAndStyledAndBetaIsFlavoredOnly() {
            var alpha = _manager.New<Alpha>("alpha");
            var beta = _manager.New<Beta>("beta");

            Assert.That(alpha.Is<Flavored>(), Is.True);
            Assert.That(alpha.Is<Styled>(), Is.True);
            Assert.That(beta.Is<Flavored>(), Is.True);
            Assert.That(beta.Is<Styled>(), Is.False);
        }

        [Test]
        public void GetByIdShouldDetermineTypeAndLoadParts() {
            var modelRecord = CreateModelRecord("alpha");

            var model = _manager.Get(modelRecord.Id);
            Assert.That(model.ContentType, Is.EqualTo("alpha"));
            Assert.That(model.Id, Is.EqualTo(modelRecord.Id));
        }


        [Test]
        public void ModelPartWithRecordShouldCallRepositoryToPopulate() {

            CreateModelRecord("gamma");
            CreateModelRecord("gamma");
            var modelRecord = CreateModelRecord("gamma");

            var model = _manager.Get(modelRecord.Id);

            // create a gamma record
            var gamma = new GammaRecord {
                ContentItem = _container.Resolve<IRepository<ContentItemRecord>>().Get(model.Id),
                Frap = "foo"
            };

            _container.Resolve<IRepository<GammaRecord>>().Create(gamma);
            _session.Flush();
            _session.Clear();

            // re-fetch from database
            model = _manager.Get(modelRecord.Id);

            Assert.That(model.ContentType, Is.EqualTo("gamma"));
            Assert.That(model.Id, Is.EqualTo(modelRecord.Id));
            Assert.That(model.Is<Gamma>(), Is.True);
            Assert.That(model.As<Gamma>().Record, Is.Not.Null);
            Assert.That(model.As<Gamma>().Record.ContentItem.Id, Is.EqualTo(model.Id));

        }

        [Test]
        public void CreateShouldMakeModelAndContentTypeRecords() {
            var beta = _manager.New("beta");
            _manager.Create(beta);

            var modelRecord = _container.Resolve<IRepository<ContentItemRecord>>().Get(beta.Id);
            Assert.That(modelRecord, Is.Not.Null);
            Assert.That(modelRecord.ContentType.Name, Is.EqualTo("beta"));
        }

        private ContentItemRecord CreateModelRecord(string contentType) {
            var contentItemRepository = _container.Resolve<IRepository<ContentItemRecord>>();
            var contentTypeRepository = _container.Resolve<IRepository<ContentTypeRecord>>();

            var modelRecord = new ContentItemRecord { ContentType = new ContentTypeRecord { Name = contentType } };
            contentTypeRepository.Create(modelRecord.ContentType);
            contentItemRepository.Create(modelRecord);

            _session.Flush();
            _session.Clear();
            return modelRecord;
        }



    }
}