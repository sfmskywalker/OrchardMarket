using System;
using DarkSky.OrchardMarket.Models;
using Orchard.Commands;
using Orchard.Data;

namespace DarkSky.OrchardMarket.Commands {
    public class CountryCommands : DefaultOrchardCommandHandler {
        private readonly IRepository<Country> _countryRepository;
        public CountryCommands(IRepository<Country> countryRepository) {
            _countryRepository = countryRepository;
        }

        [OrchardSwitch]
        public string Countries { get; set; }

        [CommandName("country create")]
        [CommandHelp("country create /Countries:<country 1, country 2, country 3>")]
        [OrchardSwitches("Countries")]
        public void Create() {

            var names = Countries.Split(new[] {',', ';', ' '}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var name in names) {
                _countryRepository.Create(new Country { Name = name.Trim() });
            }

            Context.Output.WriteLine(T("Countries created successfully.").Text);
        }
    }
}