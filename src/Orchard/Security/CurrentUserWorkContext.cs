﻿using System;

namespace Orchard.Security {
    public class CurrentUserWorkContext : IWorkContextStateProvider {
        private readonly IAuthenticationService _authenticationService;

        public CurrentUserWorkContext(IAuthenticationService authenticationService) {
            _authenticationService = authenticationService;
        }

        public Func<T> Get<T>(string name) {
            if (name == "CurrentUser") 
                return () => (T)_authenticationService.GetAuthenticatedUser();
            return null;
        }
    }
}