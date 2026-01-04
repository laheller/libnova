using LibNova;

namespace Test {
    internal class Program {
        static void Main(string[] args) {
            Console.WriteLine($"LibNova version: {LibNovaNET.ln_get_version()}\n");

            // Conversion: JD => LibNova date => .NET DateTime
            var date = new LibNovaNET.ln_date();
            LibNovaNET.ln_get_date(LibNovaNET.JD2000, ref date);
            var dt = date.ToDateTime();
            Console.WriteLine($"J2000 #1: {dt:yyyy-MMM-dd HH:mm:ss}");

            // Conversion: JD => .NET DateTime
            var dt2 = LibNovaNET.JD2000.ToDateTime();
            Console.WriteLine($"J2000 #2: {dt2:yyyy-MMM-dd HH:mm:ss}\n");

            // Conversion: current date as DateTime => LibNova date => JD
            var current = DateTime.UtcNow;
            var lnNow = current.ToLibNovaDate();
            var jdNow = LibNovaNET.ln_get_julian_day(ref lnNow);
            Console.WriteLine($"JD now (UTC) #1: {jdNow}");
            Console.WriteLine($"JD now (UTC) #2: {current.ToJD()}");

            var jdNowSys = LibNovaNET.ln_get_julian_from_sys();
            Console.WriteLine($"UTC now: {jdNowSys.ToDateTime():yyyy-MMM-dd HH:mm:ss}");
            Console.WriteLine($"JD now (UTC): {jdNowSys}\n");

            // get Venus heliocentric ecliptic coordinates
            var venus_helio = new LibNovaNET.ln_helio_posn();
            LibNovaNET.ln_get_venus_helio_coords(jdNowSys, ref venus_helio);
            var np = new LibNovaNET.ln_lnlat_posn() {
                lat = venus_helio.B,
                lng = venus_helio.L
            };
            var nph = new LibNovaNET.lnh_lnlat_posn();
            LibNovaNET.ln_lnlat_to_hlnlat(ref np, ref nph);
            Console.WriteLine($"Venus heliocentric longitude: {(nph.lng.neg != 0 ? "-" : "")}{nph.lng.degrees}°{nph.lng.minutes}'{nph.lng.seconds}\"");
            Console.WriteLine($"Venus heliocentric latitude:  {(nph.lat.neg != 0 ? "-" : "")}{nph.lat.degrees}°{nph.lat.minutes}'{nph.lat.seconds}\"\n");

            // get Venus equatorial coordinates
            var venus_equ = new LibNovaNET.ln_equ_posn();
            LibNovaNET.ln_get_venus_equ_coords(jdNowSys, ref venus_equ);
            var eph = new LibNovaNET.lnh_equ_posn();
            LibNovaNET.ln_equ_to_hequ(ref venus_equ, ref eph);
            Console.WriteLine($"Venus right ascension: {eph.ra.hours}:{eph.ra.minutes}:{eph.ra.seconds}");
            Console.WriteLine($"Venus declination:     {(eph.dec.neg != 0 ? "-" : "")}{eph.dec.degrees}°{eph.dec.minutes}'{eph.dec.seconds}\"\n");

            // get Venus-Earth distance
            var venus_dist = LibNovaNET.ln_get_venus_earth_dist(jdNowSys);
            Console.WriteLine($"Venus-Earth distance: {venus_dist} AU\n");

            // get Venus rise-transit-set times from Bratislava/Slovakia
            var hobs = new LibNovaNET.lnh_lnlat_posn() {
                lat = new LibNovaNET.ln_dms() { neg = 0, degrees = 48, minutes = 8, seconds = 38 },
                lng = new LibNovaNET.ln_dms() { neg = 0, degrees = 17, minutes = 6, seconds = 35 }
            };
            var obs = new LibNovaNET.ln_lnlat_posn();
            LibNovaNET.ln_hlnlat_to_lnlat(ref hobs, ref obs);
            var venus_rts = new LibNovaNET.ln_rst_time();
            if (LibNovaNET.ln_get_venus_rst(jdNowSys, ref obs, ref venus_rts) == 0) {
                Console.WriteLine($"Venus rises on:    {venus_rts.rise.ToDateTime():yyyy-MMM-dd HH:mm:ss} UTC");
                Console.WriteLine($"Venus transits on: {venus_rts.transit.ToDateTime():yyyy-MMM-dd HH:mm:ss} UTC");
                Console.WriteLine($"Venus sets on:     {venus_rts.set.ToDateTime():yyyy-MMM-dd HH:mm:ss} UTC\n");
            }

            // Get date from MPC compressed format
            var dpk = new LibNovaNET.ln_date();
            var str = "K01AM";
            if (LibNovaNET.ln_get_date_from_mpc(ref dpk, str) == 0) {
                Console.WriteLine($"Value of the packed MPC string {str}: {dpk.ToDateTime():yyyy-MMM-dd HH:mm:ss}\n");
            }

            var loc = 17.456789; // decimal longitude
            var loc_h = LibNovaNET.ln_get_humanr_location(loc);
            Console.WriteLine($"Human readable location of [{loc}]: {loc_h}\n");

            // Display information about Sirius
            var sirius_hpos = new LibNovaNET.lnh_equ_posn() {
                ra = new LibNovaNET.ln_hms() { hours = 6, minutes = 45, seconds = 8.9173 },
                dec = new LibNovaNET.ln_dms() { neg = 1, degrees = 16, minutes = 42, seconds = 58.017 }
            };
            var sirius_equ = new LibNovaNET.ln_equ_posn();
            LibNovaNET.ln_hequ_to_equ(ref sirius_hpos, ref sirius_equ);
            var sirius_hor = new LibNovaNET.ln_hrz_posn();
            LibNovaNET.ln_get_hrz_from_equ(ref sirius_equ, ref obs, jdNowSys, ref sirius_hor);
            var shr = new LibNovaNET.lnh_hrz_posn();
            sirius_hor.az = LibNovaNET.ln_range_degrees(180.0 + sirius_hor.az);
            LibNovaNET.ln_hrz_to_hhrz(ref sirius_hor, ref shr);
            var nswe = LibNovaNET.ln_hrz_to_nswe(ref sirius_hor);
            Console.WriteLine($"Sirius azimuth:  {shr.az.degrees}°{shr.az.minutes}'{shr.az.seconds}\" {nswe}");
            Console.WriteLine($"Sirius altitude: {(shr.alt.neg != 0 ? "-" : "")}{shr.alt.degrees}°{shr.alt.minutes}'{shr.alt.seconds}\"");
            Console.WriteLine($"Sirius is in the constellation: {LibNovaNET.ln_get_constellation(ref sirius_equ)}\n");
        }
    }
}
