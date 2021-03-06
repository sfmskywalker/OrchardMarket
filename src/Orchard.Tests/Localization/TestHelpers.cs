﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Moq;
using Orchard.Localization.Services;
using Orchard.Services;
using Orchard.Tests.Stubs;

namespace Orchard.Tests.Localization {

    internal class TestHelpers {
        public static IContainer InitializeContainer(string cultureName, string calendarName, TimeZoneInfo timeZone) {
            var builder = new ContainerBuilder();
            builder.RegisterType<StubClock>().As<IClock>();
            builder.RegisterInstance<WorkContext>(new StubWorkContext(cultureName, calendarName, timeZone));
            builder.RegisterType<StubWorkContextAccessor>().As<IWorkContextAccessor>();
            builder.RegisterType<CultureDateTimeFormatProvider>().As<IDateTimeFormatProvider>();
            builder.RegisterType<DefaultDateFormatter>().As<IDateFormatter>();
            builder.RegisterInstance(new Mock<ICalendarSelector>().Object);
            builder.RegisterType<DefaultCalendarManager>().As<ICalendarManager>();
            builder.RegisterType<DefaultDateLocalizationServices>().As<IDateLocalizationServices>();
            return builder.Build();
        }
    }

    internal class StubWorkContext : WorkContext {

        private string _cultureName;
        private string _calendarName;
        private TimeZoneInfo _timeZone;

        public StubWorkContext(string cultureName, string calendarName, TimeZoneInfo timeZone) {
            _cultureName = cultureName;
            _calendarName = calendarName;
            _timeZone = timeZone;
        }

        public override T Resolve<T>() {
            throw new NotImplementedException();
        }

        public override bool TryResolve<T>(out T service) {
            throw new NotImplementedException();
        }

        public override T GetState<T>(string name) {
            if (name == "CurrentCulture") return (T)((object)_cultureName);
            if (name == "CurrentCalendar") return (T)((object)_calendarName);
            if (name == "CurrentTimeZone") return (T)((object)_timeZone);
            throw new NotImplementedException(String.Format("Property '{0}' is not implemented.", name));
        }

        public override void SetState<T>(string name, T value) {
            throw new NotImplementedException();
        }
    }

    internal class StubWorkContextAccessor : IWorkContextAccessor {

        private WorkContext _workContext;

        public StubWorkContextAccessor(WorkContext workContext) {
            _workContext = workContext;
        }

        public WorkContext GetContext(System.Web.HttpContextBase httpContext) {
            throw new NotImplementedException();
        }

        public IWorkContextScope CreateWorkContextScope(System.Web.HttpContextBase httpContext) {
            throw new NotImplementedException();
        }

        public WorkContext GetContext() {
            return _workContext;
        }

        public IWorkContextScope CreateWorkContextScope() {
            throw new NotImplementedException();
        }
    }
}
