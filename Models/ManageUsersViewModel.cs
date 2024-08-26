using System.Collections.Generic;

namespace Weather_App.Models
{
    public class ManageUsersViewModel
    {
        public IEnumerable<UserRegistration> Users { get; set; }
        public string UsernameFilter { get; set; }
        public string NameFilter { get; set; }
        public string DefaultCityFilter { get; set; }
    }
}