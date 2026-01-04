using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LibNova {
    class StringMarshaller : ICustomMarshaler {
        public void CleanUpManagedData(object ManagedObj) { }

        public void CleanUpNativeData(IntPtr pNativeData) { }

        public int GetNativeDataSize() => -1;

        public IntPtr MarshalManagedToNative(object ManagedObj) => Marshal.StringToHGlobalAnsi(ManagedObj.ToString());

        public object MarshalNativeToManaged(IntPtr pNativeData) => Marshal.PtrToStringAnsi(pNativeData);

        public static ICustomMarshaler GetInstance(string cookie) => new StringMarshaller();
    }

    /// <summary>
	/// LibNova is a general purpose, double precision, Celestial Mechanics, Astrometry and Astrodynamics library.
	/// <para>Authors:</para>Liam Girdwood, <see href="mailto:lgirdwood@gmail.com" /> and Petr Kubanek, <see href="mailto:petr@kubanek.net" />
	/// <para>Thanks:</para>Thanks to Jean Meeus for most of the algorithms used in this library. From his book "Astronomical Algorithms".
	/// </summary>
	public static class LibNovaNET {
        private const string LibName = "clibnova";

        #region Constants
        public const double M_PI_2 = 1.5707963267948966192313216916398;
        public const double M_PI_4 = 0.78539816339744830961566084581988;
        public const double M_PI = 3.1415926535897932384626433832795;
        public const double LN_SIDEREAL_DAY_SEC = 86164.09;
        public const double LN_SIDEREAL_DAY_DAY = (LN_SIDEREAL_DAY_SEC / 86400.0);
        public const double JD2000 = 2451545.0;
        public const double JD2050 = 2469807.50;
        public const double B1900 = 2415020.3135;
        public const double B1950 = 2433282.4235;
        public const double LN_LUNAR_STANDART_HORIZON = 0.125;
        public const double LN_STAR_STANDART_HORIZON = -0.5667;
        public const double LN_SOLAR_STANDART_HORIZON = -0.8333;
        public const double LN_SOLAR_CIVIL_HORIZON = -6.0;
        public const double LN_SOLAR_NAUTIC_HORIZON = -12.0;
        public const double LN_SOLAR_ASTRONOMICAL_HORIZON = -18.0;
        #endregion

        #region Delegates
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void get_equ_body_coords_fn(double d, ref ln_equ_posn pos);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate void get_motion_body_coords_t(double d, IntPtr orbit, ref ln_equ_posn pos);
        #endregion

        #region Structures
        /// <summary>
        /// Human readable Date and time used by libnova. It's always in UTC.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ln_date {
            public int years;
            public int months;
            public int days;
            public int hours;
            public int minutes;
            public double seconds;
        }

        /// <summary>
        /// Human readable Date and time with timezone information used by libnova
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ln_zonedate {
            public int years;
            public int months;
            public int days;
            public int hours;
            public int minutes;
            public double seconds;
            public long gmtoff;
        }

        /// <summary>
        /// Degrees, minutes and seconds
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ln_dms {
            public ushort neg;
            public ushort degrees;
            public ushort minutes;
            public double seconds;
        }

        /// <summary>
        /// Hours, minutes and seconds
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ln_hms {
            public ushort hours;
            public ushort minutes;
            public double seconds;
        }

        /// <summary>
        /// Human readable Right Ascension and Declination
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct lnh_equ_posn {
            public ln_hms ra;
            public ln_dms dec;
        }

        /// <summary>
        /// Human readable Azimuth and Altitude.
		/// Azimuth South is 0 degrees, West is 90 degrees.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct lnh_hrz_posn {
            public ln_dms az;
            public ln_dms alt;
        }

        /// <summary>
        /// Human readable Ecliptical (or celestial) Latitude and Longitude.
		/// Angles are expressed in degrees. East is positive, West negative.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct lnh_lnlat_posn {
            public ln_dms lng;
            public ln_dms lat;
        }

        /// <summary>
        /// Equatorial Coordinates
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ln_equ_posn {
            public double ra;
            public double dec;
        }

        /// <summary>
        /// Horizontal Coordinates
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ln_hrz_posn {
            public double az;
            public double alt;
        }

        /// <summary>
        /// Ecliptical (or celestial) Longitude and Latitude
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ln_lnlat_posn {
            public double lng;
            public double lat;
        }

        /// <summary>
        /// Heliocentric position
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ln_helio_posn {
            public double L;
            public double B;
            public double R;
        }

        /// <summary>
        /// Rectangular coordinates
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ln_rect_posn {
            public double X;
            public double Y;
            public double Z;
        }

        /// <summary>
        /// Galactic coordinates
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ln_gal_posn {
            public double l;
            public double b;
        }

        /// <summary>
        /// Elliptic Orbital elements
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ln_ell_orbit {
            public double a;
            public double e;
            public double i;
            public double w;
            public double omega;
            public double n;
            public double JD;
        }

        /// <summary>
        /// Parabolic Orbital elements
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ln_par_orbit {
            public double q;
            public double i;
            public double w;
            public double omega;
            public double JD;
        }

        /// <summary>
        /// Hyperbolic Orbital elements
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ln_hyp_orbit {
            public double q;
            public double e;
            public double i;
            public double w;
            public double omega;
            public double JD;
        }

        /// <summary>
        /// Rise, Set and Transit times
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ln_rst_time {
            public double rise;
            public double set;
            public double transit;
        }

        /// <summary>
        /// Nutation in longitude, ecliptic and obliquity
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ln_nutation {
            public double longitude;
            public double obliquity;
            public double ecliptic;
        }
        #endregion

        #region General Calendar Functions
        /// <summary>
        /// Calculate the julian day from a calendar day. 
        /// Valid for positive and negative years but not for negative JD.
        /// </summary>
        /// <param name="date">Date required.</param>
        /// <returns>Julian day</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_julian_day(ref ln_date date);

        /// <summary>
        /// Calculate the date from the Julian day
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="date">Pointer to new calendar date.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_date(double JD, ref ln_date date);

        /// <summary>
        /// Set date from system time
        /// </summary>
        /// <param name="t">system time</param>
        /// <param name="date">Pointer to new calendar date.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_date_from_timet(IntPtr t, ref ln_date date);

        /// <summary>
        /// Set date from system tm structure
        /// </summary>
        /// <param name="tm">system tm structure</param>
        /// <param name="date">Pointer to new calendar date.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_date_from_tm(IntPtr tm, ref ln_date date);

        /// <summary>
        /// Calculate the zone date from the Julian day (UT). Gets zone info from 
        /// system using either _timezone or tm_gmtoff fields.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="zonedate">Pointer to new calendar date.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_local_date(double JD, ref ln_zonedate zonedate);

        /// <summary>
        /// Calculate the day of the week. 
        /// Returns 0 = Sunday .. 6 = Saturday
        /// </summary>
        /// <param name="date">Date required</param>
        /// <returns>Day of the week</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern uint ln_get_day_of_week(ref ln_date date);

        /// <summary>
        /// Calculate local date from system date.
        /// </summary>
        /// <param name="date">Pointer to store date.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_date_from_sys(ref ln_date date);

        /// <summary>
        /// Calculate Julian day from time_t.
        /// </summary>
        /// <param name="in_time">The time_t.</param>
        /// <returns>Julian day.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_julian_from_timet(IntPtr in_time);

        /// <summary>
        /// Calculate the julian day (UT) from the local system time.
        /// </summary>
        /// <returns>Julian day (UT)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_julian_from_sys();

        /// <summary>
        /// Calculate time_t from julian day
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="in_time">Pointer to store time_t</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_timet_from_julian(double JD, IntPtr in_time);

        /// <summary>
        /// Calculate Julian day (UT) from zone date
        /// </summary>
        /// <param name="zonedate">Local date</param>
        /// <returns>Julian day (UT)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_julian_local_date(ref ln_zonedate zonedate);

        /// <summary>
        /// Calculate the local date from the a MPC packed date.
        /// See <see href="https://www.minorplanetcenter.net/iau/info/PackedDates.html" /> for info.
        /// </summary>
        /// <param name="date">Pointer to new calendar date.</param>
        /// <param name="mpc_date">Pointer to string MPC date</param>
        /// <returns>0 for valid date</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_date_from_mpc(ref ln_date date, string mpc_date);

        /// <summary>
        /// Calculate the julian day from the a MPC packed date.
        /// See <see href="https://www.minorplanetcenter.net/iau/info/PackedDates.html" /> for info.
        /// </summary>
        /// <param name="mpc_date">Pointer to string MPC date</param>
        /// <returns>Julian day.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_julian_from_mpc(string mpc_date);

        /// <summary>
        /// Converts a ln_date (UT) to a ln_zonedate (local time).
        /// </summary>
        /// <param name="zonedate">Ptr to zonedate</param>
        /// <param name="gmtoff">Offset in seconds from UT</param>
        /// <param name="date">Ptr to date</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_date_to_zonedate(ref ln_date date, ref ln_zonedate zonedate, long gmtoff);

        /// <summary>
        /// Converts a ln_zonedate (local time) to a ln_date (UT).
        /// </summary>
        /// <param name="zonedate">Ptr to zonedate</param>
        /// <param name="date">Ptr to date</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_zonedate_to_date(ref ln_zonedate zonedate, ref ln_date date);
        #endregion

        #region Dynamical Time
        /// <summary>
        /// Calculates the dynamical time (TD) difference in seconds (delta T) from 
        /// universal time.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <returns>TD</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_dynamical_time_diff(double JD);

        /// <summary>
        /// Calculates the Julian Ephemeris Day (JDE) from the given julian day
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <returns>Julian Ephemeris day</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_jde(double JD);
        #endregion

        #region Sidereal Time
        /// <summary>
        /// Calculate the mean sidereal time at the meridian of Greenwich of a given date.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <returns>Mean sidereal time.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_mean_sidereal_time(double JD);

        /// <summary>
        /// Calculate the apparent sidereal time at the meridian of Greenwich of a given date.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <returns>Apparent sidereal time (hours).</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_apparent_sidereal_time(double JD);
        #endregion

        #region Transformation of Coordinates
        /// <summary>
        /// Transform an objects equatorial coordinates into horizontal coordinates
        /// for the given julian day and observers position.
        /// 
        /// 0 deg azimuth = south, 90 deg = west.
        /// </summary>
        /// <param name="_object">Object coordinates.</param>
        /// <param name="observer">Observer cordinates.</param>
        /// <param name="JD">Julian day</param>
        /// <param name="position">Pointer to store new position.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_hrz_from_equ(ref ln_equ_posn _object, ref ln_lnlat_posn observer, double JD, ref ln_hrz_posn position);

        /// <summary>
        /// Calculate horizontal coordinates from equatorial coordinates, using mean sidereal time.
        /// </summary>
        /// <param name="_object">Object coordinates.</param>
        /// <param name="observer">Observer cordinates.</param>
        /// <param name="sidereal">Mean sidereal time.</param>
        /// <param name="position">Pointer to store new position.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_hrz_from_equ_sidereal_time(ref ln_equ_posn _object, ref ln_lnlat_posn observer, double sidereal, ref ln_hrz_posn position);

        /// <summary>
        /// Transform an objects ecliptical coordinates into equatorial coordinates
        /// for the given julian day.
        /// </summary>
        /// <param name="_object">Object coordinates.</param>
        /// <param name="JD">Julian day</param>
        /// <param name="position">Pointer to store new position.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_equ_from_ecl(ref ln_lnlat_posn _object, double JD, ref ln_equ_posn position);

        /// <summary>
        /// Transform an objects equatorial cordinates into ecliptical coordinates
        /// for the given julian day.
        /// </summary>
        /// <param name="_object">Object coordinates in B1950. Use ln_get_equ_prec2 to transform from J2000.</param>
        /// <param name="JD">Julian day</param>
        /// <param name="position">Pointer to store new position.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_ecl_from_equ(ref ln_equ_posn _object, double JD, ref ln_lnlat_posn position);

        /// <summary>
        /// Transform an objects horizontal coordinates into equatorial coordinates
        /// for the given julian day and observers position.
        /// </summary>
        /// <param name="_object">Object coordinates.</param>
        /// <param name="observer">Observer cordinates.</param>
        /// <param name="JD">Julian day</param>
        /// <param name="position">Pointer to store new position.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_equ_from_hrz(ref ln_hrz_posn _object, ref ln_lnlat_posn observer, double JD, ref ln_equ_posn position);

        /// <summary>
        /// Transform an objects heliocentric ecliptical coordinates
        /// into heliocentric rectangular coordinates.
        /// </summary>
        /// <param name="_object">Object heliocentric coordinates</param>
        /// <param name="position">Pointer to store new position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_rect_from_helio(ref ln_helio_posn _object, ref ln_rect_posn position);

        /// <summary>
        /// Transform an objects rectangular coordinates into ecliptical coordinates.
        /// </summary>
        /// <param name="rect">Rectangular coordinates.</param>
        /// <param name="posn">Pointer to store new position.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_ecl_from_rect(ref ln_rect_posn rect, ref ln_lnlat_posn posn);

        /// <summary>
        /// Transform an object galactic coordinates into B1950 equatorial coordinate.
        /// </summary>
        /// <param name="gal">Galactic coordinates.</param>
        /// <param name="equ">B1950 equatorial coordinates. Use ln_get_equ_prec2 to transform to J2000.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_equ_from_gal(ref ln_gal_posn gal, ref ln_equ_posn equ);

        /// <summary>
        /// Transform an object galactic coordinates into equatorial coordinate.
        /// </summary>
        /// <param name="gal">Galactic coordinates.</param>
        /// <param name="equ">J2000 equatorial coordinates.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_equ2000_from_gal(ref ln_gal_posn gal, ref ln_equ_posn equ);

        /// <summary>
        /// Transform an object B1950 equatorial coordinate into galactic coordinates.
        /// </summary>
        /// <param name="equ">B1950 equatorial coordinates.</param>
        /// <param name="gal">Galactic coordinates.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_gal_from_equ(ref ln_equ_posn equ, ref ln_gal_posn gal);

        /// <summary>
        /// Transform an object J2000 equatorial coordinate into galactic coordinates.
        /// </summary>
        /// <param name="equ">J2000 equatorial coordinates.</param>
        /// <param name="gal">Galactic coordinates.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_gal_from_equ2000(ref ln_equ_posn equ, ref ln_gal_posn gal);
        #endregion

        #region Nutation
        /// <summary>
        /// Calculate nutation of longitude and obliquity in degrees from Julian Ephemeris Day
        /// </summary>
        /// <param name="JD">Julian Day.</param>
        /// <param name="nutation">Pointer to store nutation</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_nutation(double JD, ref ln_nutation nutation);
        #endregion

        #region Aberration
        /// <summary>
        /// Calculate a stars equatorial coordinates from it's mean equatorial coordinates
        /// with the effects of aberration and nutation for a given Julian Day.
        /// </summary>
        /// <param name="mean_position">Mean position of object</param>
        /// <param name="JD">Julian Day</param>
        /// <param name="position">Pointer to store new object position.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_equ_aber(ref ln_equ_posn mean_position, double JD, ref ln_equ_posn position);

        /// <summary>
        /// Calculate a stars ecliptical coordinates from it's mean ecliptical coordinates
        /// with the effects of aberration and nutation for a given Julian Day.
        /// </summary>
        /// <param name="mean_position">Mean position of object</param>
        /// <param name="JD">Julian Day</param>
        /// <param name="position">Pointer to store new object position.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_ecl_aber(ref ln_lnlat_posn mean_position, double JD, ref ln_lnlat_posn position);
        #endregion

        #region Apparent position of a Star
        /// <summary>
        /// Calculate the apparent equatorial position of a star from its mean equatorial position. 
        /// This function takes into account the effects of proper motion, precession, nutation, 
        /// annual aberration when calculating the stars apparent position. The effects of annual 
        /// parallax and the gravitational deflection of light (Einstein effect) are NOT used
        /// in this calculation.
        /// </summary>
        /// <param name="mean_position">Mean position of object</param>
        /// <param name="proper_motion">Proper motion of object</param>
        /// <param name="JD">Julian Day</param>
        /// <param name="position">Pointer to store new object position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_apparent_posn(ref ln_equ_posn mean_position, ref ln_equ_posn proper_motion, double JD, ref ln_equ_posn position);
        #endregion

        #region Solar
        /// <summary>
        /// Return solar rise/set time over local horizon (specified in degrees).
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="horizon"></param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_solar_rst_horizon(double JD, ref ln_lnlat_posn observer, double horizon, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time of rise, set and transit for the Sun.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_solar_rst(double JD, ref ln_lnlat_posn observer, ref ln_rst_time rst);

        /// <summary>
        /// Calculate geometric coordinates and radius vector
        /// accuracy 0.01 arc second error - uses VSOP87 solution.
        /// *
        /// Latitude and Longitude returned are in degrees, whilst radius
        /// vector returned is in AU.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="position">Pointer to store calculated solar position.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_solar_geom_coords(double JD, ref ln_helio_posn position);

        /// <summary>
        /// Calculate apparent equatorial solar coordinates for given julian day.
        /// This function includes the effects of aberration and nutation.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="position">Pointer to store calculated solar position.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_solar_equ_coords(double JD, ref ln_equ_posn position);

        /// <summary>
        /// Calculate apparent ecliptical solar coordinates for given julian day.
        /// This function includes the effects of aberration and nutation.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="position">Pointer to store calculated solar position.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_solar_ecl_coords(double JD, ref ln_lnlat_posn position);

        /// <summary>
        /// Calculate geocentric coordinates (rectangular) for given julian day.
        /// Accuracy 0.01 arc second error - uses VSOP87 solution.
        /// Position returned is in units of AU.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="position">Pointer to store calculated solar position.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_solar_geo_coords(double JD, ref ln_rect_posn position);

        /// <summary>
        /// Calculate the semidiameter of the Sun in arc seconds for the 
        /// given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Semidiameter in arc seconds</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_solar_sdiam(double JD);
        #endregion

        #region Precession
        /// <summary>
        /// Calculate equatorial coordinates with the effects of precession for a given Julian Day. 
        /// Uses mean equatorial coordinates and is
        /// only for initial epoch J2000.0
        /// </summary>
        /// <param name="mean_position">Mean object position</param>
        /// <param name="JD">Julian day</param>
        /// <param name="position">Pointer to store new object position.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_equ_prec(ref ln_equ_posn mean_position, double JD, ref ln_equ_posn position);

        /// <summary>
        /// Calculate the effects of precession on equatorial coordinates, between arbitary Jxxxx epochs.
        /// Use fromJD and toJD parameters to specify required Jxxxx epochs.
        /// </summary>
        /// <param name="mean_position">Mean object position</param>
        /// <param name="fromJD">Julian day (start)</param>
        /// <param name="toJD">Julian day (end)</param>
        /// <param name="position">Pointer to store new object position.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_equ_prec2(ref ln_equ_posn mean_position, double fromJD, double toJD, ref ln_equ_posn position);

        /// <summary>
        /// Calculate ecliptical coordinates with the effects of precession for a given Julian Day. 
        /// Uses mean ecliptical coordinates and is
        /// only for initial epoch J2000.0  
        /// </summary>
        /// <param name="mean_position">Mean object position</param>
        /// <param name="JD">Julian day</param>
        /// <param name="position">Pointer to store new object position.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_ecl_prec(ref ln_lnlat_posn mean_position, double JD, ref ln_lnlat_posn position);
        #endregion

        #region Proper Motion
        /// <summary>
        /// Calculate a stars equatorial coordinates from it's mean coordinates (J2000.0)
        /// with the effects of proper motion for a given Julian Day.
        /// </summary>
        /// <param name="mean_position">Mean position of object.</param>
        /// <param name="proper_motion">Annual Proper motion of object.</param>
        /// <param name="JD">Julian Day.</param>
        /// <param name="position">Pointer to store new object position.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_equ_pm(ref ln_equ_posn mean_position, ref ln_equ_posn proper_motion, double JD, ref ln_equ_posn position);

        /// <summary>
        /// Calculate a stars equatorial coordinates from it's mean coordinates and epoch
        /// with the effects of proper motion for a given Julian Day.
        /// </summary>
        /// <param name="mean_position">Mean position of object.</param>
        /// <param name="proper_motion">Annual Proper motion of object.</param>
        /// <param name="JD">Julian Day.</param>
        /// <param name="epoch_JD">Mean position epoch in JD</param>
        /// <param name="position">Pointer to store new object position.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_equ_pm_epoch(ref ln_equ_posn mean_position, ref ln_equ_posn proper_motion, double JD, double epoch_JD, ref ln_equ_posn position);
        #endregion

        #region Mercury
        /// <summary>
        /// Calculate the semidiameter of Mercury in arc seconds for the 
        /// given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Semidiameter in arc seconds</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_mercury_sdiam(double JD);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at upper culmination)
        /// time of Mercury for the given Julian day.
        /// *
        /// Note: this functions returns 1 if Mercury is circumpolar, that is it remains the whole
        /// day either above or below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_mercury_rst(double JD, ref ln_lnlat_posn observer, ref ln_rst_time rst);

        /// <summary>
        /// Calculate Mercury heliocentric (refered to the centre of the Sun) coordinates
        /// in the FK5 reference frame for the given julian day.
        /// Longitude and Latitude are in degrees, whilst radius vector is in AU.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <param name="position">Pointer to store heliocentric position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_mercury_helio_coords(double JD, ref ln_helio_posn position);

        /// <summary>
        /// Calculates Mercury's equatorial position for given julian day.
        /// This function includes calculations for planetary aberration and refers
        /// to the FK5 reference frame.
        /// *
        /// To get the complete equatorial coordinates, corrections for nutation
        /// have to be applied.
        /// *
        /// The position returned is accurate to within 0.1 arcsecs.
        /// </summary>
        /// <param name="JD">julian Day</param>
        /// <param name="position">Pointer to store position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_mercury_equ_coords(double JD, ref ln_equ_posn position);

        /// <summary>
        /// Calculates the distance in AU between the Earth and Mercury for
        /// the given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Distance in AU</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_mercury_earth_dist(double JD);

        /// <summary>
        /// Calculates the distance in AU between the Sun and Mercury for
        /// the given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Distance in AU</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_mercury_solar_dist(double JD);

        /// <summary>
        /// Calculate the visisble magnitude of Mercury for the given
        /// julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Visisble magnitude of mercury</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_mercury_magnitude(double JD);

        /// <summary>
        /// Calculate the illuminated fraction of Mercury's disk for the given Julian
        /// day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Illuminated fraction of Mercury's disk</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_mercury_disk(double JD);

        /// <summary>
        /// Calculates the phase angle of Mercury, that is, the angle Sun -
        /// Mercury - Earth for the given Julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Phase angle of Mercury (degrees)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_mercury_phase(double JD);

        /// <summary>
        /// Calculate Mercurys rectangular heliocentric coordinates for the
        /// given Julian day. Coordinates are in AU.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <param name="position">pointer to return position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_mercury_rect_helio(double JD, ref ln_rect_posn position);
        #endregion

        #region Venus
        /// <summary>
        /// Calculate the semidiameter of Venus in arc seconds for the 
        /// given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Semidiameter in arc seconds</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_venus_sdiam(double JD);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at upper culmination)
        /// time of Venus for the given Julian day.
        /// *
        /// Note: this functions returns 1 if Venus is circumpolar, that is it remains the whole
        /// day either above or below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_venus_rst(double JD, ref ln_lnlat_posn observer, ref ln_rst_time rst);

        /// <summary>
        /// Calculate Venus heliocentric (referred to the centre of the Sun) coordinates
        /// in the FK5 reference frame for the given julian day.
        /// Longitude and Latitude are in degrees, whilst radius vector is in AU.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <param name="position">Pointer to store new heliocentric position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_venus_helio_coords(double JD, ref ln_helio_posn position);

        /// <summary>
        /// Calculates Venus's equatorial position for given julian day.
        /// This function includes calculations for planetary aberration and refers
        /// to the FK5 reference frame.
        /// *
        /// To get the complete equatorial coordinates, corrections for nutation
        /// have to be applied.
        /// *
        /// The position returned is accurate to within 0.1 arcsecs..
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <param name="position">Pointer to store position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_venus_equ_coords(double JD, ref ln_equ_posn position);

        /// <summary>
        /// Calculates the distance in AU between the Earth and Venus for the
        /// given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Distance in AU</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_venus_earth_dist(double JD);

        /// <summary>
        /// Calculates the distance in AU between the Sun and Venus for
        /// the given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Distance in AU</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_venus_solar_dist(double JD);

        /// <summary>
        /// Calculate the visible magnitude of Venus for the 
        /// given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Visible magnitude of venus</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_venus_magnitude(double JD);

        /// <summary>
        /// Calculate the illuminated fraction of Venus's disk for the given Julian
        /// day
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Illuminated fraction of venus disk</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_venus_disk(double JD);

        /// <summary>
        /// Calculates the phase angle of Venus, that is, the angle Sun -
        /// Venus - Earth for the given Julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Phase angle of Venus (degrees)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_venus_phase(double JD);

        /// <summary>
        /// Calculate Venus rectangular heliocentric coordinates for the
        /// given Julian day. Coordinates are in AU.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <param name="position">pointer to return position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_venus_rect_helio(double JD, ref ln_rect_posn position);
        #endregion

        #region Earth
        /// <summary>
        /// Calculate Earths heliocentric (referred to the centre of the Sun) coordinates 
        /// for given julian day.
        /// Longitude and Latitude are in degrees, whilst radius vector is in AU.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="position">Pointer to store heliocentric position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_earth_helio_coords(double JD, ref ln_helio_posn position);

        /// <summary>
        /// Calculates the distance in AU between the Sun and Earth for 
        /// the given julian day.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <returns>Distance in AU</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_earth_solar_dist(double JD);

        /// <summary>
        /// Calculate the Earths rectangular heliocentric coordinates for the
        /// given Julian day. Coordinates are in AU.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <param name="position">pointer to return position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_earth_rect_helio(double JD, ref ln_rect_posn position);

        /// <summary>
        /// Calculate the quantities "p sin o" and "p cos o" needed in calculations for
        /// diurnal parallaxes, eclipses and occultations given the observers height
        /// in metres above sea level and there latitude in degrees.
        /// </summary>
        /// <param name="height">Height above sea level in metres.</param>
        /// <param name="latitude">latitude in degrees.</param>
        /// <param name="p_sin_o">Pointer to hold p_sin_o</param>
        /// <param name="p_cos_o">Pointer to hold p_cos_o</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_earth_centre_dist(float height, double latitude, ref double p_sin_o, ref double p_cos_o);
        #endregion

        #region Mars
        /// <summary>
        /// Calculate the semidiameter of Mars in arc seconds for the 
        /// given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Semidiameter in arc seconds</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_mars_sdiam(double JD);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at upper culmination)
        /// time of Mars for the given Julian day.
        /// *
        /// Note: this functions returns 1 if Mars is circumpolar, that is it remains the whole
        /// day either above or below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_mars_rst(double JD, ref ln_lnlat_posn observer, ref ln_rst_time rst);

        /// <summary>
        /// Calculate Mars heliocentric (refered to the centre of the Sun) coordinates
        /// in the FK5 reference frame for the given julian day.
        /// Longitude and Latitude are in degrees, whilst radius vector is in AU.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <param name="position">Pointer to store heliocentric position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_mars_helio_coords(double JD, ref ln_helio_posn position);

        /// <summary>
        /// Calculates Mars equatorial position for given julian day.
        /// This function includes calculations for planetary aberration and refers
        /// to the FK5 reference frame.
        /// *
        /// To get the complete equatorial coordinates, corrections for nutation
        /// have to be applied.
        /// *
        /// The position returned is accurate to within 0.1 arcsecs.
        /// </summary>
        /// <param name="JD">julian Day</param>
        /// <param name="position">Pointer to store position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_mars_equ_coords(double JD, ref ln_equ_posn position);

        /// <summary>
        /// Calculates the distance in AU between the Earth and Mars for the given 
        /// julian day.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <returns>Distance in AU</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_mars_earth_dist(double JD);

        /// <summary>
        /// Calculates the distance in AU between the Sun and Mars for the given 
        /// julian day.
        /// </summary>
        /// <param name="JD">Julian Day.</param>
        /// <returns>Distance in AU.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_mars_solar_dist(double JD);

        /// <summary>
        /// Calculate the visisble magnitude of Mars for given julian day.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <returns>Magnitude of Mars</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_mars_magnitude(double JD);

        /// <summary>
        /// Calculates the illuminated fraction of Mars disk for given julian day.
        /// </summary>
        /// <param name="JD">Julian Day.</param>
        /// <returns>Illuminated fraction of Mars disk (Value between 0 - 1)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_mars_disk(double JD);

        /// <summary>
        /// Calculates the phase angle of Mars for the given julian day.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <returns>Phase angle of Mars (degrees)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_mars_phase(double JD);

        /// <summary>
        /// Calculate Mars rectangular heliocentric coordinates for the
        /// given Julian day. Coordinates are in AU.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <param name="position">pointer to return position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_mars_rect_helio(double JD, ref ln_rect_posn position);
        #endregion

        #region Jupiter
        /// <summary>
        /// Calculate the equatorial semidiameter of Jupiter in arc seconds for the 
        /// given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Semidiameter in arc seconds</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_jupiter_equ_sdiam(double JD);

        /// <summary>
        /// Calculate the polar semidiameter of Jupiter in arc seconds for the 
        /// given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Semidiameter in arc seconds</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_jupiter_pol_sdiam(double JD);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at upper culmination)
        /// time of Jupiter for the given Julian day.
        /// *
        /// Note: this functions returns 1 if Jupiter is circumpolar, that is it remains the whole
        /// day either above or below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_jupiter_rst(double JD, ref ln_lnlat_posn observer, ref ln_rst_time rst);

        /// <summary>
        /// Calculate Jupiters heliocentric (refered to the centre of the Sun) coordinates
        /// in the FK5 reference frame for the given julian day.
        /// Longitude and Latitude are in degrees, whilst radius vector is in AU.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <param name="position">Pointer to store heliocentric position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_jupiter_helio_coords(double JD, ref ln_helio_posn position);

        /// <summary>
        /// Calculates Jupiter's equatorial position for given julian day.
        /// This function includes calculations for planetary aberration and refers
        /// to the FK5 reference frame.
        /// *
        /// To get the complete equatorial coordinates, corrections for nutation
        /// have to be applied.
        /// *
        /// The position returned is accurate to within 0.1 arcsecs.
        /// </summary>
        /// <param name="JD">julian Day</param>
        /// <param name="position">Pointer to store position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_jupiter_equ_coords(double JD, ref ln_equ_posn position);

        /// <summary>
        /// Calculates the distance in AU between the Earth and Jupiter for the given julian day.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <returns>Distance in AU.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_jupiter_earth_dist(double JD);

        /// <summary>
        /// Calculates the distance in AU between the Sun and Jupiter for the given julian day.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <returns>Distance in AU.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_jupiter_solar_dist(double JD);

        /// <summary>
        /// Calculate the visible magnitude of jupiter for the given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Visible magnitude of Jupiter</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_jupiter_magnitude(double JD);

        /// <summary>
        /// Calculate the illuminated fraction of Jupiter's disk for the given Julian
        /// day.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <returns>Illuminated fraction of Jupiters disk (Value between 0 and 1)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_jupiter_disk(double JD);

        /// <summary>
        /// Calculates the phase angle of Jupiter, that is, the angle Sun -
        /// Jupiter - Earth for the given Julian day.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <returns>Phase angle of Jupiter (degrees)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_jupiter_phase(double JD);

        /// <summary>
        /// Calculate Jupiters rectangular heliocentric coordinates for the
        /// given Julian day. Coordinates are in AU.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <param name="position">pointer to return position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_jupiter_rect_helio(double JD, ref ln_rect_posn position);
        #endregion

        #region Saturn
        /// <summary>
        /// Calculate the equatorial semidiameter of Saturn in arc seconds for the 
        /// given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Semidiameter in arc seconds</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_saturn_equ_sdiam(double JD);

        /// <summary>
        /// Calculate the polar semidiameter of Saturn in arc seconds for the 
        /// given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Semidiameter in arc seconds</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_saturn_pol_sdiam(double JD);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at upper culmination)
        /// time of Saturn for the given Julian day.
        /// *
        /// Note: this functions returns 1 if Saturn is circumpolar, that is it remains the whole
        /// day either above or below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_saturn_rst(double JD, ref ln_lnlat_posn observer, ref ln_rst_time rst);

        /// <summary>
        /// Calculate Saturns heliocentric (refered to the centre of the Sun) coordinates
        /// in the FK5 reference frame for the given julian day.
        /// Longitude and Latitude are in degrees, whilst radius vector is in AU.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <param name="position">Pointer to store heliocentric position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_saturn_helio_coords(double JD, ref ln_helio_posn position);

        /// <summary>
        /// Calculates Saturn's equatorial position for given julian day.
        /// This function includes calculations for planetary aberration and refers
        /// to the FK5 reference frame.
        /// *
        /// To get the complete equatorial coordinates, corrections for nutation
        /// have to be applied.
        /// *
        /// The position returned is accurate to within 0.1 arcsecs..
        /// </summary>
        /// <param name="JD">julian Day</param>
        /// <param name="position">Pointer to store position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_saturn_equ_coords(double JD, ref ln_equ_posn position);

        /// <summary>
        /// Calculates the distance in AU between the Earth and Saturn for 
        /// the given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Distance in AU</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_saturn_earth_dist(double JD);

        /// <summary>
        /// Calculates the distance in AU between the Sun and Saturn for
        /// the given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Distance in AU</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_saturn_solar_dist(double JD);

        /// <summary>
        /// Calculate the visible magnitude of Saturn for the given
        /// julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Visisble magnitude of saturn</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_saturn_magnitude(double JD);

        /// <summary>
        /// Calculate the illuminated fraction of Saturn's disk for the given Julian
        /// day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Illuminated fraction of Saturns disk. (Value between 0..1)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_saturn_disk(double JD);

        /// <summary>
        /// Calculates the phase angle of Saturn, that is, the angle Sun -
        /// Saturn - Earth for the given Julian day.
        /// </summary>
        /// <returns>Phase angle of Saturn (degrees)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_saturn_phase(double JD);

        /// <summary>
        /// Calculate Saturns rectangular heliocentric coordinates for the
        /// given Julian day. Coordinates are in AU.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <param name="position">pointer to return position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_saturn_rect_helio(double JD, ref ln_rect_posn position);
        #endregion

        #region Uranus
        /// <summary>
        /// Calculate the semidiameter of Uranus in arc seconds for the 
        /// given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Semidiameter in arc seconds</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_uranus_sdiam(double JD);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at upper culmination)
        /// time of Uranus for the given Julian day.
        /// *
        /// Note: this functions returns 1 if Uranus is circumpolar, that is it remains the whole
        /// day either above or below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_uranus_rst(double JD, ref ln_lnlat_posn observer, ref ln_rst_time rst);

        /// <summary>
        /// Calculate Uranus heliocentric (refered to the centre of the Sun) coordinates
        /// in the FK5 reference frame for the given julian day.
        /// Longitude and Latitude are in degrees, whilst radius vector is in AU.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <param name="position">Pointer to store heliocentric position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_uranus_helio_coords(double JD, ref ln_helio_posn position);

        /// <summary>
        /// Calculates uranus's equatorial position for given julian day.
        /// This function includes calculations for planetary aberration and refers
        /// to the FK5 reference frame.
        /// *
        /// To get the complete equatorial coordinates, corrections for nutation
        /// have to be applied.
        /// *
        /// The position returned is accurate to within 0.1 arcsecs.
        /// </summary>
        /// <param name="JD">julian Day</param>
        /// <param name="position">pointer to store position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_uranus_equ_coords(double JD, ref ln_equ_posn position);

        /// <summary>
        /// Calculates the distance in AU between the Earth and Uranus for 
        /// the given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Distance in AU</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_uranus_earth_dist(double JD);

        /// <summary>
        /// Calculates the distance in AU between the Sun and Uranus for
        /// the given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Distance in AU</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_uranus_solar_dist(double JD);

        /// <summary>
        /// Calculate the visible magnitude of Uranus for the given 
        /// julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Visible magnitude of Uranus</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_uranus_magnitude(double JD);

        /// <summary>
        /// Calculate the illuminated fraction of Uranus's disk for the given Julian
        /// day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Illuminated fraction of Uranus disk</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_uranus_disk(double JD);

        /// <summary>
        /// Calculates the phase angle of Uranus, that is, the angle Sun -
        /// Uranus - Earth for the given Julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Phase angle of Uranus (degrees)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_uranus_phase(double JD);

        /// <summary>
        /// Calculate Uranus rectangular heliocentric coordinates for the
        /// given Julian day. Coordinates are in AU.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <param name="position">pointer to return position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_uranus_rect_helio(double JD, ref ln_rect_posn position);
        #endregion

        #region Neptune
        /// <summary>
        /// Calculate the semidiameter of Neptune in arc seconds for the 
        /// given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Semidiameter in arc seconds</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_neptune_sdiam(double JD);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at upper culmination)
        /// time of Neptune for the given Julian day.
        /// *
        /// Note: this functions returns 1 if Neptune is circumpolar, that is it remains the whole
        /// day either above or below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_neptune_rst(double JD, ref ln_lnlat_posn observer, ref ln_rst_time rst);

        /// <summary>
        /// Calculate Neptunes heliocentric (refered to the centre of the Sun) coordinates
        /// in the FK5 reference frame for the given julian day.
        /// Longitude and Latitude are in degrees, whilst radius vector is in AU.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <param name="position">Pointer to store heliocentric position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_neptune_helio_coords(double JD, ref ln_helio_posn position);

        /// <summary>
        /// Calculates Neptune's equatorial position for given julian day.
        /// This function includes calculations for planetary aberration and refers
        /// to the FK5 reference frame.
        /// *
        /// To get the complete equatorial coordinates, corrections for nutation
        /// have to be applied.
        /// *
        /// The position returned is accurate to within 0.1 arcsecs.
        /// </summary>
        /// <param name="JD">julian Day</param>
        /// <param name="position">Pointer to store position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_neptune_equ_coords(double JD, ref ln_equ_posn position);

        /// <summary>
        /// Calculates the distance in AU between the Earth and Neptune for 
        /// the given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Distance in AU</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_neptune_earth_dist(double JD);

        /// <summary>
        /// Calculates the distance in AU between the Sun and Neptune
        /// for the given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>distance in AU</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_neptune_solar_dist(double JD);

        /// <summary>
        /// Calculate the visible magnitude of Neptune for the given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Visible magnitude of neptune</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_neptune_magnitude(double JD);

        /// <summary>
        /// Calculate the illuminated fraction of Neptune's disk for the given Julian
        /// day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Illuminated fraction of Neptune's disk</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_neptune_disk(double JD);

        /// <summary>
        /// Calculates the phase angle of Neptune, that is, the angle Sun -
        /// Neptune - Earth for the given Julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Phase angle of Neptune (degrees)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_neptune_phase(double JD);

        /// <summary>
        /// Calculate Neptunes rectangular heliocentric coordinates for the
        /// given Julian day. Coordinates are in AU.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <param name="position">pointer to return position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_neptune_rect_helio(double JD, ref ln_rect_posn position);
        #endregion

        #region Pluto
        /// <summary>
        /// Calculate the semidiameter of Pluto in arc seconds for the 
        /// given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Semidiameter in arc seconds</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_pluto_sdiam(double JD);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at upper culmination)
        /// time of Pluto for the given Julian day.
        /// *
        /// Note: this functions returns 1 if Pluto is circumpolar, that is it remains the whole
        /// day either above or below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_pluto_rst(double JD, ref ln_lnlat_posn observer, ref ln_rst_time rst);

        /// <summary>
        /// Calculate Pluto's heliocentric coordinates for the given julian day. 
        /// This function is accurate to within 0.07" in longitude, 0.02" in latitude 
        /// and 0.000006 AU in radius vector.
        /// *
        /// Note: This function is not valid outside the period of 1885-2099.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <param name="position">Pointer to store new heliocentric position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_pluto_helio_coords(double JD, ref ln_helio_posn position);

        /// <summary>
        /// Calculates Pluto's equatorial position for the given julian day.
        /// </summary>
        /// <param name="JD">julian Day</param>
        /// <param name="position">Pointer to store position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_pluto_equ_coords(double JD, ref ln_equ_posn position);

        /// <summary>
        /// Calculates the distance in AU between the Earth and Pluto for the
        /// given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Distance in AU</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_pluto_earth_dist(double JD);

        /// <summary>
        /// Calculates the distance in AU between the Sun and Pluto for the
        /// given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Distance in AU</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_pluto_solar_dist(double JD);

        /// <summary>
        /// Calculate the visible magnitude of Pluto for the given
        /// julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Visible magnitude of Pluto</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_pluto_magnitude(double JD);

        /// <summary>
        /// Calculate the illuminated fraction of Pluto's disk for
        /// the given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Illuminated fraction of Plutos disk</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_pluto_disk(double JD);

        /// <summary>
        /// Calculate the phase angle of Pluto (Sun - Pluto - Earth)
        /// for the given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Phase angle of Pluto (degrees)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_pluto_phase(double JD);

        /// <summary>
        /// Calculate Plutos rectangular heliocentric coordinates for the
        /// given Julian day. Coordinates are in AU.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <param name="position">pointer to return position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_pluto_rect_helio(double JD, ref ln_rect_posn position);
        #endregion

        #region VSOP87
        /// <summary>
        /// Transform from VSOP87 to FK5 reference frame.
        /// </summary>
        /// <param name="position">Position to transform.</param>
        /// <param name="JD">Julian day</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_vsop87_to_fk5(ref ln_helio_posn position, double JD);

        [StructLayout(LayoutKind.Sequential)]
        public struct ln_vsop {
            public double A;
            public double B;
            public double C;
        }

        /// <summary>
        /// Helper function for the VSOP87 theory.
        /// </summary>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_calc_series(ref ln_vsop data, int terms, double t);
        #endregion

        #region Lunar
        /// <summary>
        /// Calculate the semidiameter of the Moon in arc seconds for the 
        /// given julian day.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <returns>Semidiameter in arc seconds</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_lunar_sdiam(double JD);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at upper culmination)
        /// time of the Moon for the given Julian day.
        /// *
        /// Note: this functions returns 1 if the Moon is circumpolar, that is it remains the whole
        /// day either above or below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_lunar_rst(double JD, ref ln_lnlat_posn observer, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the rectangular geocentric lunar coordinates to the inertial mean
        /// ecliptic and equinox of J2000. 
        /// The geocentric coordinates returned are in units of km.
        /// 
        /// This function is based upon the Lunar Solution ELP2000-82B by 
        /// Michelle Chapront-Touze and Jean Chapront of the Bureau des Longitudes, 
        /// Paris.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <param name="pos">Pointer to a geocentric position structure to held result.</param>
        /// <param name="precision">The truncation level of the series in radians for longitude and latitude and in km for distance. (Valid range 0 - 0.01, 0 being highest accuracy)</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_lunar_geo_posn(double JD, ref ln_rect_posn pos, double precision);

        /// <summary>
        /// Calculate the lunar RA and DEC for Julian day JD. 
        /// Accuracy is better than 10 arcsecs in right ascension and 4 arcsecs in declination.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <param name="position">Pointer to a struct ln_lnlat_posn to store result.</param>
        /// <param name="precision">The truncation level of the series in radians for longitude and latitude and in km for distance. (Valid range 0 - 0.01, 0 being highest accuracy)</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_lunar_equ_coords_prec(double JD, ref ln_equ_posn position, double precision);

        /// <summary>
        /// Calculate the lunar RA and DEC for Julian day JD. 
        /// Accuracy is better than 10 arcsecs in right ascension and 4 arcsecs in declination.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <param name="position">Pointer to a struct ln_lnlat_posn to store result.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_lunar_equ_coords(double JD, ref ln_equ_posn position);

        /// <summary>
        /// Calculate the lunar longitude and latitude for Julian day JD.
        /// Accuracy is better than 10 arcsecs in longitude and 4 arcsecs in latitude.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <param name="position">Pointer to a struct ln_lnlat_posn to store result.</param>
        /// <param name="precision">The truncation level of the series in radians for longitude and latitude and in km for distance. (Valid range 0 - 0.01, 0 being highest accuracy)</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_lunar_ecl_coords(double JD, ref ln_lnlat_posn position, double precision);

        /// <summary>
        /// Calculates the angle Sun - Moon - Earth.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <returns>Phase angle. (Value between 0 and 180)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_lunar_phase(double JD);

        /// <summary>
        /// Calculates the illuminated fraction of the Moon's disk.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <returns>Illuminated fraction. (Value between 0 and 1)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_lunar_disk(double JD);

        /// <summary>
        /// Calculates the distance between the centre of the Earth and the
        /// centre of the Moon in km.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <returns>The distance between the Earth and Moon in km.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_lunar_earth_dist(double JD);

        /// <summary>
        /// Calculates the position angle of the midpoint of the illuminated limb of the 
        /// moon, reckoned eastward from the north point of the disk.
        /// *
        /// The angle is near 270 deg for first quarter and near 90 deg after a full moon.
        /// The position angle of the cusps are +90 deg and -90 deg.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <returns>The position angle in degrees.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_lunar_bright_limb(double JD);

        /// <summary>
        /// Calculate the mean longitude of the Moons ascening node
        /// for the given Julian day.
        /// </summary>
        /// <param name="JD">Julian Day.</param>
        /// <returns>Longitude of ascending node in degrees.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_lunar_long_asc_node(double JD);

        /// <summary>
        /// Calculate the longitude of the Moon's mean perigee.
        /// </summary>
        /// <param name="JD">Julian Day</param>
        /// <returns>Longitude of Moons mean perigee in degrees.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_lunar_long_perigee(double JD);
        #endregion

        #region Elliptic Motion
        /// <summary>
        /// Calculate the eccentric anomaly. 
        /// This method was devised by Roger Sinnott. (Sky and Telescope, Vol 70, pg 159)
        /// </summary>
        /// <param name="E">Orbital eccentricity</param>
        /// <param name="M">Mean anomaly</param>
        /// <returns>Eccentric anomaly</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_solve_kepler(double E, double M);

        /// <summary>
        /// Calculate the mean anomaly.
        /// </summary>
        /// <param name="n">Mean motion (degrees/day)</param>
        /// <param name="delta_JD">Time since perihelion</param>
        /// <returns>Mean anomaly (degrees)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_ell_mean_anomaly(double n, double delta_JD);

        /// <summary>
        /// Calculate the true anomaly.
        /// </summary>
        /// <param name="e">Orbital eccentricity</param>
        /// <param name="E">Eccentric anomaly</param>
        /// <returns>True anomaly (degrees)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_ell_true_anomaly(double e, double E);

        /// <summary>
        /// Calculate the radius vector.
        /// </summary>
        /// <param name="a">Semi-Major axis in AU</param>
        /// <param name="e">Orbital eccentricity</param>
        /// <param name="E">Eccentric anomaly</param>
        /// <returns>Radius vector AU</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_ell_radius_vector(double a, double e, double E);

        /// <summary>
        /// Calculate the semi major diameter.
        /// </summary>
        /// <param name="e">Eccentricity</param>
        /// <param name="q">Perihelion distance in AU</param>
        /// <returns>Semi-major diameter in AU</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_ell_smajor_diam(double e, double q);

        /// <summary>
        /// Calculate the semi minor diameter.
        /// </summary>
        /// <param name="e">Eccentricity</param>
        /// <param name="a">Semi-Major diameter in AU</param>
        /// <returns>Semi-minor diameter in AU</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_ell_sminor_diam(double e, double a);

        /// <summary>
        /// Calculate the mean daily motion (degrees/day).
        /// </summary>
        /// <param name="a">Semi major diameter in AU</param>
        /// <returns>Mean daily motion (degrees/day)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_ell_mean_motion(double a);

        /// <summary>
        /// Calculate the objects rectangular geocentric position given it's orbital
        /// elements for the given julian day.
        /// </summary>
        /// <param name="orbit">Orbital parameters of object.</param>
        /// <param name="JD">Julian day</param>
        /// <param name="posn">Position pointer to store objects position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_ell_geo_rect_posn(ref ln_ell_orbit orbit, double JD, ref ln_rect_posn posn);

        /// <summary>
        /// Calculate the objects rectangular heliocentric position given it's orbital
        /// elements for the given julian day.
        /// </summary>
        /// <param name="orbit">Orbital parameters of object.</param>
        /// <param name="JD">Julian day</param>
        /// <param name="posn">Position pointer to store objects position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_ell_helio_rect_posn(ref ln_ell_orbit orbit, double JD, ref ln_rect_posn posn);

        /// <summary>
        /// Calculate the orbital length in AU. 
        /// 
        /// Accuracy: 
        /// - 0.001% for e &lt; 0.88
        /// - 0.01% for e &lt; 0.95
        /// - 1% for e = 0.9997
        /// - 3% for e = 1
        /// </summary>
        /// <param name="orbit">Orbital parameters</param>
        /// <returns>Orbital length in AU</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_ell_orbit_len(ref ln_ell_orbit orbit);

        /// <summary>
        /// Calculate orbital velocity in km/s for the given julian day.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <returns>Orbital velocity in km/s.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_ell_orbit_vel(double JD, ref ln_ell_orbit orbit);

        /// <summary>
        /// Calculate orbital velocity at perihelion in km/s.
        /// </summary>
        /// <param name="orbit">Orbital parameters</param>
        /// <returns>Orbital velocity in km/s.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_ell_orbit_pvel(ref ln_ell_orbit orbit);

        /// <summary>
        /// Calculate the orbital velocity at aphelion in km/s.
        /// </summary>
        /// <param name="orbit">Orbital parameters</param>
        /// <returns>Orbital velocity in km/s.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_ell_orbit_avel(ref ln_ell_orbit orbit);

        /// <summary>
        /// Calculate the phase angle of the body. The angle Sun - body - Earth.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <returns>Phase angle.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_ell_body_phase_angle(double JD, ref ln_ell_orbit orbit);

        /// <summary>
        /// Calculate the bodies elongation to the Sun..
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <returns>Elongation to the Sun.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_ell_body_elong(double JD, ref ln_ell_orbit orbit);

        /// <summary>
        /// Calculate the distance between a body and the Sun.
        /// </summary>
        /// <param name="JD">Julian Day.</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <returns>The distance in AU between the Sun and the body.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_ell_body_solar_dist(double JD, ref ln_ell_orbit orbit);

        /// <summary>
        /// Calculate the distance between an body and the Earth
        /// for the given julian day.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <param name="orbit">Orbital parameters</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_ell_body_earth_dist(double JD, ref ln_ell_orbit orbit);

        /// <summary>
        /// Calculate a bodies equatorial coordinates for the given julian day.
        /// </summary>
        /// <param name="JD">Julian Day.</param>
        /// <param name="orbit">Orbital parameters.</param>
        /// <param name="posn">Pointer to hold asteroid position.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_ell_body_equ_coords(double JD, ref ln_ell_orbit orbit, ref ln_equ_posn posn);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at upper culmination)
        /// time of a body with an elliptic orbit for the given Julian day.
        /// *
        /// Note: this functions returns 1 if the body is circumpolar, that is it remains the whole
        /// day above the horizon. Returns -1 when it remains the whole day below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_ell_body_rst(double JD, ref ln_lnlat_posn observer, ref ln_ell_orbit orbit, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at upper culmination)
        /// time of a body with an elliptic orbit for the given Julian day.
        /// *
        /// Note: this functions returns 1 if the body is circumpolar, that is it remains the whole
        /// day above the horizon. Returns -1 when it remains the whole day below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <param name="horizon">Horizon height</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_ell_body_rst_horizon(double JD, ref ln_lnlat_posn observer, ref ln_ell_orbit orbit, double horizon, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time of next rise, set and transit (crosses the local meridian at upper culmination)
        /// time of a body with an elliptic orbit for the given Julian day.
        /// *
        /// This function guarantee, that rise, set and transit will be in &lt;JD, JD+1&gt; range.
        /// *
        /// Note: this functions returns 1 if the body is circumpolar, that is it remains the whole
        /// day above the horizon. Returns -1 when it remains the whole day below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_ell_body_next_rst(double JD, ref ln_lnlat_posn observer, ref ln_ell_orbit orbit, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time of next rise, set and transit (crosses the local meridian at upper culmination)
        /// time of a body with an elliptic orbit for the given Julian day.
        /// *
        /// This function guarantee, that rise, set and transit will be in &lt;JD, JD+1&gt; range.
        /// *
        /// Note: this functions returns 1 if the body is circumpolar, that is it remains the whole
        /// day above the horizon. Returns -1 when it remains the whole day below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <param name="horizon">Horizon height</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_ell_body_next_rst_horizon(double JD, ref ln_lnlat_posn observer, ref ln_ell_orbit orbit, double horizon, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time of next rise, set and transit (crosses the local meridian at upper culmination)
		/// time of a body with an elliptic orbit for the given Julian day.
		/// This function guarantee, that rise, set and transit will be in &lt;JD, JD + day_limit&gt; range.
		/// Note: this functions returns 1 if the body is circumpolar, that is it remains the whole
		/// day above the horizon. Returns -1 when it remains the whole day below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <param name="horizon">Horizon height</param>
        /// <param name="day_limit">Maximal number of days that will be searched for next rise and set</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_ell_body_next_rst_horizon_future(double JD, ref ln_lnlat_posn observer, ref ln_ell_orbit orbit, double horizon, int day_limit, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the julian day of the last perihelion.
        /// </summary>
        /// <param name="epoch_JD">Julian day of epoch</param>
        /// <param name="M">Mean anomaly</param>
        /// <param name="n">daily motion in degrees</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_ell_last_perihelion(double epoch_JD, double M, double n);
        #endregion

        #region Asteroids
        /// <summary>
        /// Calculate the visual magnitude of an asteroid.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <param name="H">Mean absolute visual magnitude</param>
        /// <param name="G">Slope parameter</param>
        /// <returns>The visual magnitude.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_asteroid_mag(double JD, ref ln_ell_orbit orbit, double H, double G);

        /// <summary>
        /// Calculate the semidiameter of an asteroid in km.
        /// *
        /// Note: Many asteroids have an irregular shape and therefore this function returns
        /// an approximate value of the diameter.
        /// </summary>
        /// <param name="H">Absolute magnitude of asteroid</param>
        /// <param name="A">Albedo of asteroid</param>
        /// <returns>Semidiameter in km</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_asteroid_sdiam_km(double H, double A);

        /// <summary>
        /// Calculate the semidiameter of an asteroid in arc seconds.
        /// *
        /// Note: Many asteroids have an irregular shape and therefore this function returns
        /// an approximate value of the diameter.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <param name="H">Absolute magnitude of asteroid</param>
        /// <param name="A">Albedo of asteroid</param>
        /// <returns>Semidiameter in seconds of arc</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_asteroid_sdiam_arc(double JD, ref ln_ell_orbit orbit, double H, double A);
        #endregion

        #region Comets
        /// <summary>
        /// Calculate the visual magnitude of a comet in an elliptical orbit.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <param name="g">Absolute magnitude</param>
        /// <param name="k">Comet constant</param>
        /// <returns>The visual magnitude.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_ell_comet_mag(double JD, ref ln_ell_orbit orbit, double g, double k);

        /// <summary>
        /// Calculate the visual magnitude of a comet in a parabolic orbit.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <param name="g">Absolute magnitude</param>
        /// <param name="k">Comet constant</param>
        /// <returns>The visual magnitude.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_par_comet_mag(double JD, ref ln_par_orbit orbit, double g, double k);
        #endregion

        #region Parabolic Motion
        /// <summary>
        /// Solve Barkers equation. LIAM add more
        /// </summary>
        /// <param name="q">Perihelion distance in AU</param>
        /// <param name="t">Time since perihelion in days</param>
        /// <returns>Solution of Barkers equation</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_solve_barker(double q, double t);

        /// <summary>
        /// Calculate the true anomaly.
        /// </summary>
        /// <param name="q">Perihelion distance in AU</param>
        /// <param name="t">Time since perihelion</param>
        /// <returns>True anomaly (degrees)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_par_true_anomaly(double q, double t);

        /// <summary>
        /// Calculate the radius vector.
        /// </summary>
        /// <param name="q">Perihelion distance in AU</param>
        /// <param name="t">Time since perihelion in days</param>
        /// <returns>Radius vector AU</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_par_radius_vector(double q, double t);

        /// <summary>
        /// Calculate the objects rectangular geocentric position given it's orbital
        /// elements for the given julian day.
        /// </summary>
        /// <param name="orbit">Orbital parameters of object.</param>
        /// <param name="JD">Julian day</param>
        /// <param name="posn">Position pointer to store objects position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_par_geo_rect_posn(ref ln_par_orbit orbit, double JD, ref ln_rect_posn posn);

        /// <summary>
        /// Calculate the objects rectangular heliocentric position given it's orbital
        /// elements for the given julian day.
        /// </summary>
        /// <param name="orbit">Orbital parameters of object.</param>
        /// <param name="JD">Julian day</param>
        /// <param name="posn">Position pointer to store objects position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_par_helio_rect_posn(ref ln_par_orbit orbit, double JD, ref ln_rect_posn posn);

        /// <summary>
        /// Calculate a bodies equatorial coordinates for the given julian day.
        /// </summary>
        /// <param name="JD">Julian Day.</param>
        /// <param name="orbit">Orbital parameters.</param>
        /// <param name="posn">Pointer to hold asteroid position.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_par_body_equ_coords(double JD, ref ln_par_orbit orbit, ref ln_equ_posn posn);

        /// <summary>
        /// Calculate the distance between a body and the Earth
        /// for the given julian day.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <param name="orbit">Orbital parameters</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_par_body_earth_dist(double JD, ref ln_par_orbit orbit);

        /// <summary>
        /// Calculate the distance between a body and the Sun.
        /// </summary>
        /// <param name="JD">Julian Day.</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <returns>The distance in AU between the Sun and the body.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_par_body_solar_dist(double JD, ref ln_par_orbit orbit);

        /// <summary>
        /// Calculate the phase angle of the body. The angle Sun - body - Earth.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <returns>Phase angle.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_par_body_phase_angle(double JD, ref ln_par_orbit orbit);

        /// <summary>
        /// Calculate the bodies elongation to the Sun..
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <returns>Elongation to the Sun.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_par_body_elong(double JD, ref ln_par_orbit orbit);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at upper culmination)
        /// time of a body with a parabolic orbit for the given Julian day.
        /// *
        /// Note: this functions returns 1 if the body is circumpolar, that is it remains the whole
        /// day either above the horizon. Returns -1 when it remains whole day below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_par_body_rst(double JD, ref ln_lnlat_posn observer, ref ln_par_orbit orbit, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at upper culmination)
        /// time of a body with a parabolic orbit for the given Julian day.
        /// *
        /// Note: this functions returns 1 if the body is circumpolar, that is it remains the whole
        /// day either above the horizon. Returns -1 when it remains whole day below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <param name="horizon">Horizon height</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_par_body_rst_horizon(double JD, ref ln_lnlat_posn observer, ref ln_par_orbit orbit, double horizon, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time of next rise, set and transit (crosses the local meridian at upper culmination)
        /// time of a body with an parabolic orbit for the given Julian day.
        /// *
        /// This function guarantee, that rise, set and transit will be in &lt;JD, JD+1&gt; range.
        /// *
        /// Note: this functions returns 1 if the body is circumpolar, that is it remains the whole
        /// day above the horizon. Returns -1 when it remains the whole day below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_par_body_next_rst(double JD, ref ln_lnlat_posn observer, ref ln_par_orbit orbit, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time of next rise, set and transit (crosses the local meridian at upper culmination)
        /// time of a body with an parabolic orbit for the given Julian day.
        /// *
        /// This function guarantee, that rise, set and transit will be in &lt;JD, JD+1&gt; range.
        /// *
        /// Note: this functions returns 1 if the body is circumpolar, that is it remains the whole
        /// day above the horizon. Returns -1 when it remains the whole day below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <param name="horizon">Horizon height</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_par_body_next_rst_horizon(double JD, ref ln_lnlat_posn observer, ref ln_par_orbit orbit, double horizon, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time of next rise, set and transit (crosses the local meridian at upper culmination)
        /// time of a body with an parabolic orbit for the given Julian day.
        /// *
        /// This function guarantee, that rise, set and transit will be in &lt;JD, JD + day_limit&gt; range.
        /// *
        /// Note: this functions returns 1 if the body is circumpolar, that is it remains the whole
        /// day above the horizon. Returns -1 when it remains the whole day below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <param name="horizon">Horizon height</param>
        /// <param name="day_limit">Maximal number of days that will be searched for next rise and set</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_par_body_next_rst_horizon_future(double JD, ref ln_lnlat_posn observer, ref ln_par_orbit orbit, double horizon, int day_limit, ref ln_rst_time rst);
        #endregion

        #region Atmospheric Refraction
        /// <summary>
        /// Calculate the adjustment in altitude of a body due to atmosphric 
        /// refraction. This value varies over altitude, pressure and temperature.
        /// 
        /// Note: Default values for pressure and teperature are 1010 mBar and 10C 
        /// respectively.
        /// </summary>
        /// <param name="altitude">The altitude of the object above the horizon in degrees</param>
        /// <param name="atm_pres">Atmospheric pressure in milibars</param>
        /// <param name="temp">Temperature in degrees C.</param>
        /// <returns>Adjustment in objects altitude in degrees.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_refraction_adj(double altitude, double atm_pres, double temp);
        #endregion

        #region Rise, Set, Transit
        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at upper culmination)
        /// time of the object for the given Julian day.
        /// *
        /// Note: this functions returns 1 if the object is circumpolar, that is it remains the whole
        /// day above the horizon. Returns -1 when it remains the whole day bellow the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="_object">Object position</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_object_rst(double JD, ref ln_lnlat_posn observer, ref ln_equ_posn _object, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at upper culmination)
        /// time of the object for the given Julian day and horizon.
        /// *
        /// Note: this functions returns 1 if the object is circumpolar, that is it remains the whole
        /// day above the horizon. Returns -1 when it remains whole day bellow the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="_object">Object position</param>
        /// <param name="horizon">Horizon height</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_object_rst_horizon(double JD, ref ln_lnlat_posn observer, ref ln_equ_posn _object, double horizon, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at upper culmination)
		/// time of the object for the given Julian day and horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="_object">Object position</param>
        /// <param name="horizon">Horizon height</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <param name="ut_offset">The "ln_get_object_rst_horizon( )" calls this function with offset = 0.5</param>
        /// <returns>0 for success, 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_object_rst_horizon_offset(double JD, ref ln_lnlat_posn observer, ref ln_equ_posn _object, double horizon, ref ln_rst_time rst, double ut_offset);

        /// <summary>
        /// Calculate the time of next rise, set and transit (crosses the local meridian at upper culmination)
        /// time of the object for the given Julian day and horizon.
        /// *
        /// This function guarantee, that rise, set and transit will be in &lt;JD, JD+1&gt; range.
        /// *
        /// Note: this functions returns 1 if the object is circumpolar, that is it remains the whole
        /// day above the horizon. Returns -1 when it remains whole day bellow the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="_object">Object position</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_object_next_rst(double JD, ref ln_lnlat_posn observer, ref ln_equ_posn _object, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time of next rise, set and transit (crosses the local meridian at upper culmination)
        /// time of the object for the given Julian day and horizon.
        /// *
        /// This function guarantee, that rise, set and transit will be in &lt;JD, JD+1&gt; range.
        /// *
        /// Note: this functions returns 1 if the object is circumpolar, that is it remains the whole
        /// day above the horizon. Returns -1 when it remains whole day bellow the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="_object">Object position</param>
        /// <param name="horizon">Horizon height</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_object_next_rst_horizon(double JD, ref ln_lnlat_posn observer, ref ln_equ_posn _object, double horizon, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at
        /// upper culmination) time of the body for the given Julian day and given
        /// horizon.
        /// *
        /// *
        /// Note 1: this functions returns 1 if the object is circumpolar, that is it remains the whole
        /// day above the horizon. Returns -1 when it remains whole day bellow the horizon.
        /// *
        /// Note 2: this function will not work for body, which ra changes more
        /// then 180 deg in one day (get_equ_body_coords changes so much). But
        /// you should't use that function for any body which moves to fast..use
        /// some special function for such things.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="get_equ_body_coords">Pointer to get_equ_body_coords() function</param>
        /// <param name="horizon">Horizon, see LN_XXX_HORIZON constants</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_body_rst_horizon(double JD, ref ln_lnlat_posn observer, get_equ_body_coords_fn get_equ_body_coords, double horizon, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at
		/// upper culmination) time of the body for the given Julian day and given horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="get_equ_body_coords">Pointer to get_equ_body_coords() function</param>
        /// <param name="horizon">Horizon, see LN_XXX_HORIZON constants</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <param name="ut_offset">The "ln_get_body_rst_horizon( )" calls this function with offset = 0.5</param>
        /// <returns>0 for success, 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_body_rst_horizon_offset(double JD, ref ln_lnlat_posn observer, get_equ_body_coords_fn get_equ_body_coords, double horizon, ref ln_rst_time rst, double ut_offset);

        /// <summary>
        /// Calculate the time of next rise, set and transit (crosses the local meridian at
        /// upper culmination) time of the body for the given Julian day and given
        /// horizon.
        /// *
        /// This function guarantee, that rise, set and transit will be in &lt;JD, JD+1&gt; range.
        /// *
        /// Note 1: this functions returns 1 if the body is circumpolar, that is it remains
        /// the whole day either above or below the horizon.
        /// *
        /// Note 2: This function will not work for body, which ra changes more
        /// then 180 deg in one day (get_equ_body_coords changes so much). But
        /// you should't use that function for any body which moves to fast..use
        /// some special function for such things.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="get_equ_body_coords">Pointer to get_equ_body_coords() function</param>
        /// <param name="horizon">Horizon, see LN_XXX_HORIZON constants</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_body_next_rst_horizon(double JD, ref ln_lnlat_posn observer, get_equ_body_coords_fn get_equ_body_coords, double horizon, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time of next rise, set and transit (crosses the local meridian at
        /// upper culmination) time of the body for the given Julian day and given
        /// horizon.
        /// *
        /// This function guarantee, that rise, set and transit will be in &lt;JD, JD + day_limit&gt; range.
        /// *
        /// Note 1: this functions returns 1 if the body is circumpolar, that is it remains
        /// the whole day either above or below the horizon.
        /// *
        /// Note 2: This function will not work for body, which ra changes more
        /// than 180 deg in one day (get_equ_body_coords changes so much). But
        /// you should't use that function for any body which moves to fast..use
        /// some special function for such things.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="get_equ_body_coords">Pointer to get_equ_body_coords() function</param>
        /// <param name="horizon">Horizon, see LN_XXX_HORIZON constants</param>
        /// <param name="day_limit">Maximal number of days that will be searched for next rise and set</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_body_next_rst_horizon_future(double JD, ref ln_lnlat_posn observer, get_equ_body_coords_fn get_equ_body_coords, double horizon, int day_limit, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at
		/// upper culmination) time of the body for the given Julian day and given horizon.
		/// Note 1: this functions returns 1 if the body is circumpolar, that is it remains
		/// the whole day either above or below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="get_motion_body_coords">Pointer to ln_get_ell_body_equ_coords. ln_get_para_body_equ_coords or ln_get_hyp_body_equ_coords function</param>
        /// <param name="orbit">Orbital parameters (ln_ell_orbit or ln_par_orbit or ln_hyp_orbit)</param>
        /// <param name="horizon">Horizon, see LN_XXX_HORIZON constants</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_motion_body_rst_horizon(double JD, ref ln_lnlat_posn observer, get_motion_body_coords_t get_motion_body_coords, IntPtr orbit, double horizon, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at
        /// upper culmination) time of the body for the given Julian day and given horizon.
        /// Note 1: this functions returns 1 if the body is circumpolar, that is it remains
        /// the whole day either above or below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="get_motion_body_coords">Pointer to ln_get_ell_body_equ_coords. ln_get_para_body_equ_coords or ln_get_hyp_body_equ_coords function</param>
        /// <param name="orbit">Orbital parameters (ln_ell_orbit or ln_par_orbit or ln_hyp_orbit)</param>
        /// <param name="horizon">Horizon, see LN_XXX_HORIZON constants</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <param name="offset">The "ln_get_motion_body_rst_horizon( )" calls this function with offset = 0.5</param>
        /// <returns>0 for success, 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_motion_body_rst_horizon_offset(double JD, ref ln_lnlat_posn observer, get_motion_body_coords_t get_motion_body_coords, IntPtr orbit, double horizon, ref ln_rst_time rst, double offset);

        /// <summary>
        /// Calculate the time of next rise, set and transit (crosses the local meridian at
		/// upper culmination) time of the body for the given Julian day and given horizon.
		/// This function guarantee, that rise, set and transit will be in &lt;JD, JD+1&gt; range.
		/// Note 1: this functions returns 1 if the body is circumpolar, that is it remains
		/// the whole day either above or below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="get_motion_body_coords">Pointer to ln_get_ell_body_equ_coords. ln_get_para_body_equ_coords or ln_get_hyp_body_equ_coords function</param>
        /// <param name="orbit">Orbital parameters (ln_ell_orbit or ln_par_orbit or ln_hyp_orbit)</param>
        /// <param name="horizon">Horizon, see LN_XXX_HORIZON constants</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_motion_body_next_rst_horizon(double JD, ref ln_lnlat_posn observer, get_motion_body_coords_t get_motion_body_coords, IntPtr orbit, double horizon, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time of next rise, set and transit (crosses the local meridian at
        /// upper culmination) time of the body for the given Julian day and given
        /// horizon.
        /// *
        /// This function guarantee, that rise, set and transit will be in &lt;JD, JD + day_limit&gt; range.
        /// *
        /// Note 1: this functions returns 1 if the body is circumpolar, that is it remains
        /// the whole day either above or below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="get_motion_body_coords">Pointer to ln_get_ell_body_equ_coords. ln_get_para_body_equ_coords or ln_get_hyp_body_equ_coords function</param>
        /// <param name="orbit">Orbital parameters (ln_ell_orbit or ln_par_orbit or ln_hyp_orbit)</param>
        /// <param name="horizon">Horizon, see LN_XXX_HORIZON constants</param>
        /// <param name="day_limit">Maximal number of days that will be searched for next rise and set</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_motion_body_next_rst_horizon_future(double JD, ref ln_lnlat_posn observer, get_motion_body_coords_t get_motion_body_coords, IntPtr orbit, double horizon, int day_limit, ref ln_rst_time rst);
        #endregion

        #region Angular Separation
        /// <summary>
        /// Calculates the angular separation of 2 bodies.
        /// This method was devised by Mr Thierry Pauwels of the
        /// Royal Observatory Belgium.
        /// </summary>
        /// <param name="posn1">Equatorial position of body 1</param>
        /// <param name="posn2">Equatorial position of body 2</param>
        /// <returns>Angular separation in degrees</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_angular_separation(ref ln_equ_posn posn1, ref ln_equ_posn posn2);

        /// <summary>
        /// Calculates the position angle of a body with respect to another body.
        /// </summary>
        /// <param name="posn1">Equatorial position of body 1</param>
        /// <param name="posn2">Equatorial position of body 2</param>
        /// <returns>Position angle in degrees</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_rel_posn_angle(ref ln_equ_posn posn1, ref ln_equ_posn posn2);
        #endregion

        #region LibNova library version information
        /// <summary>
        /// Return the libnova library version number string
        /// e.g. "0.4.0"
        /// </summary>
        /// <returns>Null terminated version string.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaller))]
        public static extern string ln_get_version();
        #endregion

        #region Miscellaneous Functions
        /// <summary>
        /// Obtains Latitude, Longitude, RA or Declination from a string.
        /// *
        ///  If the last char is N/S doesn't accept more than 90 degrees.            
        ///  If it is E/W doesn't accept more than 180 degrees.                      
        ///  If they are hours don't accept more than 24:00                          
        ///                                                                          
        ///  Any position can be expressed as follows:                               
        ///  (please use a 8 bits charset if you want                                
        ///  to view the degrees separator char '0xba')                              
        /// *
        ///  42.30.35,53                                                             
        ///  90°0'0,01 W                                                             
        ///  42°30'35.53 N                                                           
        ///  42°30'35.53S                                                            
        ///  42°30'N                                                                 
        ///  - 42.30.35.53                                                           
        ///   42:30:35.53 S                                                          
        ///  + 42.30.35.53                                                           
        ///  +42°30 35,53                                                            
        ///   23h36'45,0                                                             
        ///                                                                          
        ///                                                                          
        ///  42:30:35.53 S = -42°30'35.53"                                           
        ///  + 42 30.35.53 S the same previous position, the plus (+) sign is        
        ///  considered like an error, the last 'S' has precedence over the sign     
        ///                                                                          
        ///  90°0'0,01 N ERROR: +- 90°0'00.00" latitude limit                        
        /// *
        /// </summary>
        /// <param name="s">Location string</param>
        /// <returns>angle in degrees</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_dec_location(string s);

        /// <summary>
        /// Obtains a human readable location in the form: dd°mm'ss.ss"
        /// </summary>
        /// <param name="location">Location angle in degress</param>
        /// <returns>Angle string</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaller))]
        public static extern string ln_get_humanr_location(double location);

        /// <summary>
        /// Calculate an intermediate value of the 3 arguments for the given interpolation
        /// factor.
        /// </summary>
        /// <param name="n">Interpolation factor</param>
        /// <param name="y1">Argument 1</param>
        /// <param name="y2">Argument 2</param>
        /// <param name="y3">Argument 3</param>
        /// <returns>interpolation value</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_interpolate3(double n, double y1, double y2, double y3);

        /// <summary>
        /// Calculate an intermediate value of the 5 arguments for the given interpolation
        /// factor.
        /// </summary>
        /// <param name="n">Interpolation factor</param>
        /// <param name="y1">Argument 1</param>
        /// <param name="y2">Argument 2</param>
        /// <param name="y3">Argument 3</param>
        /// <param name="y4">Argument 4</param>
        /// <param name="y5">Argument 5</param>
        /// <returns>interpolation value</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_interpolate5(double n, double y1, double y2, double y3, double y4, double y5);

        /// <summary>
        /// Calculate the distance between rectangular points a and b.
        /// </summary>
        /// <param name="a">First rectangular coordinate</param>
        /// <param name="b">Second rectangular coordinate</param>
        /// <returns>Distance between a and b.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_rect_distance(ref ln_rect_posn a, ref ln_rect_posn b);
        #endregion

        #region General Conversion Functions
        /// <summary>
        /// Convert radians to degrees
        /// </summary>
        /// <param name="radians">Angle in radians to convert from</param>
        /// <returns>Angle in degrees</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_rad_to_deg(double radians);

        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        /// <param name="degrees">Angle in degrees to convert from</param>
        /// <returns>Angle in radians</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_deg_to_rad(double degrees);

        /// <summary>
        /// Convert hours:mins:secs to degrees
        /// </summary>
        /// <param name="hms">Hours:minutes:seconds to convert from</param>
        /// <returns>Angle in degrees</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_hms_to_deg(ref ln_hms hms);

        /// <summary>
        /// Convert degrees to hh:mm:ss
        /// </summary>
        /// <param name="degrees">Angle in degrees to convert from</param>
        /// <param name="hms">The time equvivalent (in H:M:S) of the input angle</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_deg_to_hms(double degrees, ref ln_hms hms);

        /// <summary>
        /// Convert hours:mins:secs to radians
        /// </summary>
        /// <param name="hms">Hours:minutes:seconds to convert from</param>
        /// <returns>Angle in radians</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_hms_to_rad(ref ln_hms hms);

        /// <summary>
        /// Convert radians to hh:mm:ss
        /// </summary>
        /// <param name="radians">Angle in radians to convert from</param>
        /// <param name="hms">The time equvivalent (in H:M:S) of the input angle</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_rad_to_hms(double radians, ref ln_hms hms);

        /// <summary>
        /// Convert dms to degrees
        /// </summary>
        /// <param name="dms">Angle in D°M'S" to convert from</param>
        /// <returns>Angle in decimal degrees</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_dms_to_deg(ref ln_dms dms);

        /// <summary>
        /// Convert degrees to dms
        /// </summary>
        /// <param name="degrees">Angle in decimal degrees to convert from</param>
        /// <param name="dms">Angle in D°M'S" format</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_deg_to_dms(double degrees, ref ln_dms dms);

        /// <summary>
        /// Convert dms to radians
        /// </summary>
        /// <param name="dms">Angle in D°M'S" to convert from</param>
        /// <returns>Angle in radians</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_dms_to_rad(ref ln_dms dms);

        /// <summary>
        /// Convert radians to dms
        /// </summary>
        /// <param name="radians">Angle in radians to convert from</param>
        /// <param name="dms">Angle in D°M'S" format</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_rad_to_dms(double radians, ref ln_dms dms);

        /// <summary>
        /// Human readable equatorial position to double equatorial position.
        /// </summary>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_hequ_to_equ(ref lnh_equ_posn hpos, ref ln_equ_posn pos);

        /// <summary>
        /// Human double equatorial position to human readable equatorial position.
        /// </summary>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_equ_to_hequ(ref ln_equ_posn pos, ref lnh_equ_posn hpos);

        /// <summary>
        /// Human readable horizontal position to double horizontal position.
        /// </summary>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_hhrz_to_hrz(ref lnh_hrz_posn hpos, ref ln_hrz_posn pos);

        /// <summary>
        /// Double horizontal position to human readable horizontal position.
        /// </summary>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_hrz_to_hhrz(ref ln_hrz_posn pos, ref lnh_hrz_posn hpos);

        /// <summary>
        /// Returns direction of given azimuth - like N,S,W,E,NSW,...
        /// </summary>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaller))]
        public static extern string ln_hrz_to_nswe(ref ln_hrz_posn pos);

        /// <summary>
        /// Human readable long/lat position to double long/lat position.
        /// </summary>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_hlnlat_to_lnlat(ref lnh_lnlat_posn hpos, ref ln_lnlat_posn pos);

        /// <summary>
        /// Double long/lat position to human readable long/lat position.
        /// </summary>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_lnlat_to_hlnlat(ref ln_lnlat_posn pos, ref lnh_lnlat_posn hpos);

        /// <summary>
        /// Add seconds to hms
        /// </summary>
        /// <param name="hms">Time in H:M:S format as input</param>
        /// <param name="seconds">Number of seconds to add</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_add_secs_hms(ref ln_hms hms, double seconds);

        /// <summary>
        /// Add hms to hms
        /// </summary>
        /// <param name="source">Time in H:M:S format as input</param>
        /// <param name="dest">Time in H:M:S format as the return object to add to input</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_add_hms(ref ln_hms source, ref ln_hms dest);

        /// <summary>
        /// Puts a large angle in the correct range 0 - 360 degrees
        /// </summary>
        /// <param name="angle">Any angle (even outside the range 0 - 360) in degrees</param>
        /// <returns>Angle in degreees</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_range_degrees(double angle);

        /// <summary>
        /// Puts a large angle in the correct range 0 - 2PI radians
        /// </summary>
        /// <param name="angle">Any angle (even outside the range 0 - 2PI) in radians</param>
        /// <returns>Angle in radians</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_range_radians(double angle);

        /// <summary>
        /// Puts a large angle in the correct range -2PI - +2PI radians
		/// preserve sign
        /// </summary>
        /// <param name="angle">Any angle (even outside the range -2PI - +2PI) in radians</param>
        /// <returns>Angle in radians</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_range_radians2(double angle);

        /// <summary>
        /// Convert units of AU into light days.
        /// </summary>
        /// <param name="dist">Distance in AU</param>
        /// <returns>Distance in light days.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_light_time(double dist);
        #endregion

        #region Hyperbolic Motion
        /// <summary>
        /// Solve Barkers equation. LIAM add more
        /// </summary>
        /// <param name="Q1">See 35.0</param>
        /// <param name="G">See 35.0</param>
        /// <param name="t">Time since perihelion in days</param>
        /// <returns>Solution of Barkers equation</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_solve_hyp_barker(double Q1, double G, double t);

        /// <summary>
        /// Calculate the true anomaly.
        /// </summary>
        /// <param name="q">Perihelion distance in AU</param>
        /// <param name="e">Orbit eccentricity</param>
        /// <param name="t">Time since perihelion</param>
        /// <returns>True anomaly (degrees)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_hyp_true_anomaly(double q, double e, double t);

        /// <summary>
        /// Calculate the radius vector.
        /// </summary>
        /// <param name="q">Perihelion distance in AU</param>
        /// <param name="e">Orbit eccentricity</param>
        /// <param name="t">Time since perihelion in days</param>
        /// <returns>Radius vector AU</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_hyp_radius_vector(double q, double e, double t);

        /// <summary>
        /// Calculate the objects rectangular geocentric position given it's orbital
        /// elements for the given julian day.
        /// </summary>
        /// <param name="orbit">Orbital parameters of object.</param>
        /// <param name="JD">Julian day</param>
        /// <param name="posn">Position pointer to store objects position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_hyp_geo_rect_posn(ref ln_hyp_orbit orbit, double JD, ref ln_rect_posn posn);

        /// <summary>
        /// Calculate the objects rectangular heliocentric position given it's orbital
        /// elements for the given julian day.
        /// </summary>
        /// <param name="orbit">Orbital parameters of object.</param>
        /// <param name="JD">Julian day</param>
        /// <param name="posn">Position pointer to store objects position</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_hyp_helio_rect_posn(ref ln_hyp_orbit orbit, double JD, ref ln_rect_posn posn);

        /// <summary>
        /// Calculate a bodies equatorial coordinates for the given julian day.
        /// </summary>
        /// <param name="JD">Julian Day.</param>
        /// <param name="orbit">Orbital parameters.</param>
        /// <param name="posn">Pointer to hold asteroid position.</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_hyp_body_equ_coords(double JD, ref ln_hyp_orbit orbit, ref ln_equ_posn posn);

        /// <summary>
        /// Calculate the distance between a body and the Earth
        /// for the given julian day.
        /// </summary>
        /// <param name="JD">Julian day.</param>
        /// <param name="orbit">Orbital parameters</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_hyp_body_earth_dist(double JD, ref ln_hyp_orbit orbit);

        /// <summary>
        /// Calculate the distance between a body and the Sun.
        /// </summary>
        /// <param name="JD">Julian Day.</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <returns>The distance in AU between the Sun and the body.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_hyp_body_solar_dist(double JD, ref ln_hyp_orbit orbit);

        /// <summary>
        /// Calculate the phase angle of the body. The angle Sun - body - Earth.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <returns>Phase angle.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_hyp_body_phase_angle(double JD, ref ln_hyp_orbit orbit);

        /// <summary>
        /// Calculate the bodies elongation to the Sun..
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <returns>Elongation to the Sun.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_hyp_body_elong(double JD, ref ln_hyp_orbit orbit);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at upper culmination)
        /// time of a body with a parabolic orbit for the given Julian day.
        /// *
        /// Note: this functions returns 1 if the body is circumpolar, that is it remains the whole
        /// day above or below the horizon. Returns -1 when it remains whole day below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_hyp_body_rst(double JD, ref ln_lnlat_posn observer, ref ln_hyp_orbit orbit, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time the rise, set and transit (crosses the local meridian at upper culmination)
        /// time of a body with a parabolic orbit for the given Julian day.
        /// *
        /// Note: this functions returns 1 if the body is circumpolar, that is it remains the whole
        /// day above or below the horizon. Returns -1 when it remains whole day below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <param name="horizon">Horizon height</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_hyp_body_rst_horizon(double JD, ref ln_lnlat_posn observer, ref ln_hyp_orbit orbit, double horizon, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time of next rise, set and transit (crosses the local meridian at upper culmination)
        /// time of a body with an hyperbolic orbit for the given Julian day.
        /// *
        /// This function guarantee, that rise, set and transit will be in &lt;JD, JD+1&gt; range.
        /// *
        /// Note: this functions returns 1 if the body is circumpolar, that is it remains the whole
        /// day above the horizon. Returns -1 when it remains the whole day below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_hyp_body_next_rst(double JD, ref ln_lnlat_posn observer, ref ln_hyp_orbit orbit, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time of next rise, set and transit (crosses the local meridian at upper culmination)
        /// time of a body with an hyperbolic orbit for the given Julian day.
        /// *
        /// This function guarantee, that rise, set and transit will be in &lt;JD, JD+1&gt; range.
        /// *
        /// Note: this functions returns 1 if the body is circumpolar, that is it remains the whole
        /// day above the horizon. Returns -1 when it remains the whole day below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <param name="horizon">Horizon height</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_hyp_body_next_rst_horizon(double JD, ref ln_lnlat_posn observer, ref ln_hyp_orbit orbit, double horizon, ref ln_rst_time rst);

        /// <summary>
        /// Calculate the time of next rise, set and transit (crosses the local meridian at upper culmination)
        /// time of a body with an hyperbolic orbit for the given Julian day.
        /// *
        /// This function guarantee, that rise, set and transit will be in &lt;JD, JD + day_limit&gt; range.
        /// *
        /// Note: this functions returns 1 if the body is circumpolar, that is it remains the whole
        /// day above the horizon. Returns -1 when it remains the whole day below the horizon.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="observer">Observers position</param>
        /// <param name="orbit">Orbital parameters</param>
        /// <param name="horizon">Horizon height</param>
        /// <param name="day_limit">Maximal number of days that will be searched for next rise and set</param>
        /// <param name="rst">Pointer to store Rise, Set and Transit time in JD</param>
        /// <returns>0 for success, else 1 for circumpolar (above the horizon), -1 for circumpolar (bellow the horizon)</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern int ln_get_hyp_body_next_rst_horizon_future(double JD, ref ln_lnlat_posn observer, ref ln_hyp_orbit orbit, double horizon, int day_limit, ref ln_rst_time rst);
        #endregion

        #region Parallax
        /// <summary>
        /// Calculate body parallax, which is need to calculate topocentric position of the body.
        /// </summary>
        /// <param name="_object">Object geocentric coordinates</param>
        /// <param name="au_distance">Distance of object from Earth in AU</param>
        /// <param name="observer">Geographics observer positions</param>
        /// <param name="height">Observer height in m</param>
        /// <param name="JD">Julian day of observation</param>
        /// <param name="parallax">RA and DEC parallax</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_parallax(ref ln_equ_posn _object, double au_distance, ref ln_lnlat_posn observer, double height, double JD, ref ln_equ_posn parallax);

        /// <summary>
        /// Calculate body parallax, which is need to calculate topocentric position of the body.
        /// Uses hour angle as time reference (handy in case we already compute it).
        /// </summary>
        /// <param name="_object">Object geocentric coordinates</param>
        /// <param name="au_distance">Distance of object from Earth in AU</param>
        /// <param name="observer">Geographics observer positions</param>
        /// <param name="height">Observer height in m</param>
        /// <param name="H">Hour angle of object in hours</param>
        /// <param name="parallax">RA and DEC parallax</param>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern void ln_get_parallax_ha(ref ln_equ_posn _object, double au_distance, ref ln_lnlat_posn observer, double height, double H, ref ln_equ_posn parallax);
        #endregion

        #region Airmass
        /// <summary>
        /// Calculate air mass in given altitude.
        /// </summary>
        /// <param name="alt">Altitude in degrees</param>
        /// <param name="airmass_scale">Airmass scale - usually 750.</param>
        /// <returns>Airmass for give altitude.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_airmass(double alt, double airmass_scale);

        /// <summary>
        /// Calculate altitude for given air mass.
        /// </summary>
        /// <param name="X">Airmass</param>
        /// <param name="airmass_scale">Airmass scale - usually 750.</param>
        /// <returns>Altitude for give airmass.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_alt_from_airmass(double X, double airmass_scale);
        #endregion

        #region Heliocentric time
        /// <summary>
        /// Calculate heliocentric correction for object at given coordinates and on given date.
        /// </summary>
        /// <param name="JD">Julian day</param>
        /// <param name="_object">Pointer to object (RA, DEC) for which heliocentric correction will be caculated</param>
        /// <returns>Heliocentric correction in fraction of day</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        public static extern double ln_get_heliocentric_time_diff(double JD, ref ln_equ_posn _object);
        #endregion

        #region IAU constellations
        /// <summary>
        /// Returns name of the constellation based on boundaries found at:
        /// <see href="http://vizier.cds.unistra.fr/viz-bin/VizieR?-source=6042" />
        /// </summary>
        /// <param name="position">Equitorial position</param>
        /// <returns>Name of the the constellation at the given position, or "---" if constellation cannot be found.</returns>
        [DllImport(LibName, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaller))]
        public static extern string ln_get_constellation(ref ln_equ_posn position);
        #endregion

        #region Extension methods
        /// <summary>
        /// Converts LibNova ln_date to .NET DateTime
        /// </summary>
        public static DateTime ToDateTime(this ln_date date, DateTimeKind kind = DateTimeKind.Utc) {
            var WholeSec = Math.Truncate(date.seconds);
            var Milis = (date.seconds - WholeSec) * 1000;
            return new DateTime(date.years, date.months, date.days, date.hours, date.minutes, (int)WholeSec, (int)Milis, kind);
        }

        /// <summary>
        /// Converts .NET DateTime to LibNova ln_date
        /// </summary>
        public static ln_date ToLibNovaDate(this DateTime dt) {
            Debug.Assert(dt.Kind == DateTimeKind.Utc, "Input date/time must be specified in UTC!");
            return new ln_date() {
                years = dt.Year,
                months = dt.Month,
                days = dt.Day,
                hours = dt.Hour,
                minutes = dt.Minute,
                seconds = dt.Second + dt.Millisecond / 1000.0
            };
        }

        /// <summary>
        /// Converts .NET DateTime to Julian Days
        /// </summary>
        public static double ToJD(this DateTime dt) {
            Debug.Assert(dt.Kind == DateTimeKind.Utc, "Input date/time must be specified in UTC!");
            var ln_dt = ToLibNovaDate(dt);
            return ln_get_julian_day(ref ln_dt);
        }

        /// <summary>
        /// Converts Julian Days to .NET DateTime
        /// </summary>
        /// <param name="JD"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this double JD, DateTimeKind kind = DateTimeKind.Utc) {
            var ln_dt = new ln_date();
            ln_get_date(JD, ref ln_dt);
            return ToDateTime(ln_dt, kind);
        }

        /// <summary>
        /// Converts LibNova ln_zonedate to .NET DateTime
        /// </summary>
        public static DateTime ToDateTime(this ln_zonedate date, DateTimeKind kind = DateTimeKind.Local) {
            var WholeSec = Math.Truncate(date.seconds);
            var Milis = (date.seconds - WholeSec) * 1000;
            var ret = new DateTime(date.years, date.months, date.days, date.hours, date.minutes, (int)WholeSec, (int)Milis, kind);
            return ret.AddSeconds(date.gmtoff);
        }
        #endregion
    }
}