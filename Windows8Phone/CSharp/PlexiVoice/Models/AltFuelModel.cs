using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexiVoice.Models
{
    public class AltFuelFederalAgency
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class AltFuelModel
    {
        public object access_days_time { get; set; }
        public string status_code { get; set; }
        public object intersection_directions { get; set; }
        public string updated_at { get; set; }
        public object e85_blender_pump { get; set; }
        public object hy_status_link { get; set; }
        public int id { get; set; }
        public string geocode_status { get; set; }
        public object ng_fill_type_code { get; set; }
        public object ng_vehicle_class { get; set; }
        public string date_last_confirmed { get; set; }
        public object ev_dc_fast_num { get; set; }
        public double latitude { get; set; }
        public string open_date { get; set; }
        public object ng_psi { get; set; }
        public object ev_other_evse { get; set; }
        public object cards_accepted { get; set; }
        public string groups_with_access_code { get; set; }
        public string fuel_type_code { get; set; }
        public object ev_level2_evse_num { get; set; }
        public object lpg_primary { get; set; }
        public string bd_blends { get; set; }
        public object ev_network_web { get; set; }
        public string station_name { get; set; }
        public object plus4 { get; set; }
        public object expected_date { get; set; }
        public string owner_type_code { get; set; }
        public object ev_network { get; set; }
        public double longitude { get; set; }
        public object ev_level1_evse_num { get; set; }
        public AltFuelFederalAgency federal_agency { get; set; }
        public object station_phone { get; set; }
        public string address { get; set; }
        public double distance { get; set; }
    }
}
