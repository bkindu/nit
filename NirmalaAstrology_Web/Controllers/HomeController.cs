using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NirmalaAstrology_Web.Filters;
using NirmalaAstrology_Web.Models;
using NirmalaAstrology_Web.Repositories;
using NirmalaAstrology_Web.Services;
using NirmalaAstrology_Web.ViewModels;

namespace NirmalaAstrology_Web.Controllers
{
    /// <summary>
    /// Home Controller - Handles the main homepage, Kundli generation, and Place management.
    /// </summary>
    [SessionState(SessionStateBehavior.Required)]
    public class HomeController : Controller
    {
        private readonly PlaceRepository _placeRepo = new PlaceRepository();
        private readonly CountryRepository _countryRepo = new CountryRepository();
        private readonly CustomerRepository _customerRepo = new CustomerRepository();
        private readonly ZodiacRepository _zodiacRepo = new ZodiacRepository();

        // GET: Kundli/Result (renders the chart page with calculated data)
        public ActionResult KundliResult()
        {
            try
            {
                // Try to get kundliBirthData from TempData (passed from client if needed)
                // Or we'll pass a dummy object to Result.cshtml which will handle reading from localStorage
                var sessionBirthData = BirthDataSessionStore.Get(Session);
                var birthDataJson = sessionBirthData == null ? (TempData["kundliBirthData"] as string) : JsonConvert.SerializeObject(sessionBirthData, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                // Create a model to pass to Result view
                dynamic resultModel = new System.Dynamic.ExpandoObject();
                resultModel.BirthDataJson = birthDataJson ?? "{}";

                return View("~/Views/Kundli/Result.cshtml", resultModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[KundliResult Error] " + ex.Message);
                // Return view even on error - Result.cshtml will handle the error display
                return View("~/Views/Kundli/Result.cshtml");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SetBirthDataSession(BirthDataSessionModel birthData)
        {
            try
            {
                if (birthData == null || birthData.Year < 1 || birthData.Month < 1 || birthData.Day < 1)
                    return Json(new { success = false, message = "Birth data is incomplete." });

                if (Session == null)
                    return Json(new { success = false, message = "ASP.NET session state is unavailable." });

                BirthDataSessionStore.Set(Session, birthData);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[SetBirthDataSession Error] " + ex);
                Response.StatusCode = 500;
                return Json(new { success = false, message = "Unable to preserve birth data." });
            }
        }

        [HttpGet]
        public JsonResult GetBirthDataSession()
        {
            return Json(BirthDataSessionStore.Get(Session), JsonRequestBehavior.AllowGet);
        }

        // POST: Home/CalculateKundli (AJAX endpoint - calculates Kundli from birth data)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CalculateKundli(int day, int month, int year, int hours, int minutes, double latitude, double longitude, double timezone = 5.30)
        {
            try
            {
                var existingBirthData = BirthDataSessionStore.Get(Session);
                var isAdmin = User != null && User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole(AuthService.RoleAdmin);
                var reportChoice = existingBirthData == null || String.IsNullOrWhiteSpace(existingBirthData.ReportChoice) ? "DETAILED" : existingBirthData.ReportChoice;
                // Public / guest: limited OPT_KUNDLI only — never DETAILED ganit chain over the wire.
                if (!isAdmin)
                    reportChoice = "OPT_KUNDLI";
                if (existingBirthData == null)
                    existingBirthData = new BirthDataSessionModel();
                existingBirthData.Day = day; existingBirthData.Month = month; existingBirthData.Year = year;
                existingBirthData.Hours = hours; existingBirthData.Minutes = minutes;
                existingBirthData.Latitude = latitude; existingBirthData.Longitude = longitude; existingBirthData.Timezone = timezone;
                if (!isAdmin) existingBirthData.ReportChoice = "OPT_KUNDLI";
                else if (string.IsNullOrWhiteSpace(existingBirthData.ReportChoice)) existingBirthData.ReportChoice = "DETAILED";
                BirthDataSessionStore.Set(Session, existingBirthData);

                // Pure C# KundliCalculator (legacy Indu / a2Functions port) — no JavaScript.
                var workflow = new KundliWorkflowService();
                var workflowResult = workflow.Generate(reportChoice, new KundliWorkflowRequest
                {
                    Day = day,
                    Month = month,
                    Year = year,
                    Hour = hours,
                    Minute = minutes,
                    Longitude = longitude,
                    Latitude = latitude,
                    TimezoneOffset = timezone
                });
                var result = workflowResult.Calculation;
                KundliWorkflowCache.Store(Session, workflowResult);

                if (result == null)
                {
                    return Json(new { success = false, message = "Calculation failed. Please check your birth details." });
                }

                // Convert houses to objects
                dynamic houses = null;
                if (result.Houses != null)
                {
                    houses = result.Houses.Select(h => new
                    {
                        houseNumber = h.HouseNumber,
                        rashi = h.Rashi,
                        rashiName = h.RashiName,
                        planets = h.Planets ?? new List<string>()
                    }).ToList();
                }

                // Convert planets to objects
                dynamic planetPositions = null;
                if (result.PlanetPositions != null)
                {
                    planetPositions = result.PlanetPositions.Select(p => new
                    {
                        name = p.Name,
                        rashi = p.Rashi,
                        rashiName = p.RashiName,
                        degrees = p.Degrees,
                        minutes = p.Minutes,
                        degreesInRashi = p.DegreesInRashi,
                        totalDegrees = p.TotalDegrees,
                        house = p.House,
                        isRetrograde = p.IsRetrograde,
                        retrograde = p.Retrograde
                    }).ToList();
                }

                // Build lagna object from Ascendant (PlanetPosition) with safe fallbacks
                var asc = result.Ascendant;
                var lagnaObj = new
                {
                    degree = asc != null ? asc.TotalDegrees : 0,
                    rashi = asc != null ? asc.Rashi : 1,
                    rashiName = asc != null ? asc.RashiName : "Aries",
                    degreesInRashi = asc != null ? asc.DegreesInRashi : 0
                };

                // Details are produced by the same KUNDALI workflow after calculation.
                // Map explicitly because MVC 5's default JSON serializer preserves PascalCase.
                var d = workflowResult.Details;
                var dbContent = d == null ? null : d.DatabaseContent;
                var kundliDetails = new
                {
                    birthRashiName = d != null ? d.BirthRashiName : "",
                    birthRashi = d != null ? d.BirthRashi : 0,
                    sunSignName = d != null ? d.SunSignName : "",
                    nakshatraName = d != null ? d.NakshatraName : "",
                    nakshatraNumber = d != null ? d.NakshatraNumber : 0,
                    nakshatraPada = d != null ? d.NakshatraPada : 0,
                    isGandmool = d != null && d.IsGandmool,
                    payaRashiWise = d != null ? d.PayaRashiWise : "",
                    payaNakshatraWise = d != null ? d.PayaNakshatraWise : "",
                    isManglik = d != null && d.IsManglik,
                    combustPlanets = d != null ? d.CombustPlanets : new List<string>(),
                    retrogradePlanets = d != null ? d.RetrogradePlanets : new List<string>(),
                    rashiSandhiPlanets = d != null ? d.RashiSandhiPlanets : new List<string>(),
                    planets = d == null ? new List<object>() : d.Planets.Select(p => (object)new
                    {
                        name = p.Name, code = p.Code, fullName = p.FullName,
                        rashi = p.Rashi, rashiName = p.RashiName, house = p.House,
                        totalDegrees = p.TotalDegrees, degreesInRashi = p.DegreesInRashi,
                        isRetrograde = p.IsRetrograde, motion = p.Motion,
                        udayAsta = p.UdayAsta, nakshatraNumber = p.NakshatraNumber,
                        nakshatraPada = p.NakshatraPada, nakshatraName = p.NakshatraName,
                        nakshatraLord = p.NakshatraLord, nakshatraDevta = p.NakshatraDevta, dignity = p.Dignity,
                        gati = p.Gati, dailyMotion = p.DailyMotion, strengthPercent = p.StrengthPercent,
                        positionRakv = p.PositionRakv, rashi0 = p.Rashi0
                    }).ToList(),
                    housePercentages = d == null ? new List<object>() : d.HousePercentages.Select(h => (object)new
                    {
                        house = h.House, rashi = h.Rashi, rashiName = h.RashiName,
                        occupancyPercent = h.OccupancyPercent, planetCount = h.PlanetCount
                    }).ToList(),
                    percentageNote = d != null ? d.PercentageNote : "",
                    panchang = d == null ? null : new
                    {
                        samvatsar = d.Panchang.Samvatsar, tithi = d.Panchang.Tithi, nakshatra = d.Panchang.Nakshatra, yoga = d.Panchang.Yoga, karan = d.Panchang.Karan,
                        paksha = d.Panchang.Paksha, vaarEnglish = d.Panchang.VaarEnglish, vaarHindu = d.Panchang.VaarHindu,
                        nakshatraPada = d.Panchang.NakshatraPada, rashi = d.Panchang.Rashi, sunSign = d.Panchang.SunSign, paya = d.Panchang.Paya,
                        saurMaas = d.Panchang.SaurMaas, ayan = d.Panchang.Ayan, gola = d.Panchang.Gola, ritu = d.Panchang.Ritu,
                        vikramiSamvat = d.Panchang.VikramiSamvat, sakaSamvat = d.Panchang.SakaSamvat, sunrise = d.Panchang.Sunrise, sunset = d.Panchang.Sunset,
                        ishtakal = d.Panchang.Ishtakal, dinman = d.Panchang.Dinman, siderealTime = d.Panchang.SiderealTime, ayanamsa = d.Panchang.Ayanamsa,
                        gmt = d.Panchang.Gmt, apparentNoon = d.Panchang.ApparentNoon, localMeanTime = d.Panchang.LocalMeanTime,
                        declinationOfSun = d.Panchang.DeclinationOfSun, ghatLagan = d.Panchang.GhatLagan,
                        tithiBhukat = d.Panchang.TithiBhukat,
                        tithiBhogya = d.Panchang.TithiBhogya, yogaBhukat = d.Panchang.YogaBhukat, yogaBhogya = d.Panchang.YogaBhogya,
                        karanBhukat = d.Panchang.KaranBhukat, karanBhogya = d.Panchang.KaranBhogya, nakshatraBhukat = d.Panchang.NakshatraBhukat,
                        nakshatraBhogya = d.Panchang.NakshatraBhogya, bhabhog = d.Panchang.Bhabhog,
                        sunriseTithi = d.Panchang.SunriseTithi, sunriseNakshatra = d.Panchang.SunriseNakshatra,
                        sunriseYoga = d.Panchang.SunriseYoga, sunriseKaran = d.Panchang.SunriseKaran,
                        tithiEnding = d.Panchang.TithiEnding, vaarEnding = d.Panchang.VaarEnding,
                        karanEnding = d.Panchang.KaranEnding, yogaEnding = d.Panchang.YogaEnding, nakshatraEnding = d.Panchang.NakshatraEnding,
                        nakshatraBhukatVikala = d.Panchang.NakshatraBhukatVikala, nakshatraBhogyaVikala = d.Panchang.NakshatraBhogyaVikala,
                        horaRashi = d.Panchang.HoraRashi, horaRashiLord = d.Panchang.HoraRashiLord, horaVaran = d.Panchang.HoraVaran,
                        horaVashya = d.Panchang.HoraVashya, horaHansak = d.Panchang.HoraHansak, horaRashiName = d.Panchang.HoraRashiName,
                        horaYoni = d.Panchang.HoraYoni, horaGana = d.Panchang.HoraGana, horaNadi = d.Panchang.HoraNadi,
                        horaYunja = d.Panchang.HoraYunja, horaNameLetter = d.Panchang.HoraNameLetter, horaVarag = d.Panchang.HoraVarag,
                        horaPaya = d.Panchang.HoraPaya, horaSunSign = d.Panchang.HoraSunSign,
                        ghatMaas = d.Panchang.GhatMaas, ghatTithies = d.Panchang.GhatTithies, ghatVaar = d.Panchang.GhatVaar,
                        ghatNakshatra = d.Panchang.GhatNakshatra, ghatYog = d.Panchang.GhatYog, ghatKaran = d.Panchang.GhatKaran,
                        ghatPrahar = d.Panchang.GhatPrahar, ghatPlanetPositions = d.Panchang.GhatPlanetPositions, ghatGrah = d.Panchang.GhatGrah,
                        ghatDhanya = d.Panchang.GhatDhanya, ghatJati = d.Panchang.GhatJati, ghatGender = d.Panchang.GhatGender,
                        ghatKaruradi = d.Panchang.GhatKaruradi, ghatCharadi = d.Panchang.GhatCharadi, ghatVarna = d.Panchang.GhatVarna,
                        velantar = d.Panchang.Velantar, charaPal = d.Panchang.CharaPal, palbha = d.Panchang.Palbha,
                        meanSun = d.Panchang.MeanSun, warTimeCorrection = d.Panchang.WarTimeCorrection, trueRahu = d.Panchang.TrueRahu
                    },
                    databaseContent = (!isAdmin || dbContent == null) ? null : new
                    {
                        nakshatraReading = dbContent.NakshatraReading,
                        nakshatraCharanReading = dbContent.NakshatraCharanReading,
                        signReading = dbContent.SignReading,
                        houseReading = dbContent.HouseReading,
                        udayaAsthaRule = dbContent.UdayaAsthaRule,
                        planetHouseReadings = dbContent.PlanetHouseReadings.Select(x => (object)new
                        {
                            planetCode = x.PlanetCode, planetName = x.PlanetName, house = x.House,
                            birthReading = x.BirthReading, gocharReading = x.GocharReading,
                            lalKitabReading = x.LalKitabReading, sourceReading = x.SourceReading
                        }).ToList(),
                        planetSignReadings = dbContent.PlanetSignReadings.Select(x => (object)new
                        {
                            planetCode = x.PlanetCode, planetName = x.PlanetName, sign = x.Sign,
                            sourceReading = x.SourceReading
                        }).ToList()
                    }
                };

                // Convert result to JSON-serializable format
                // Sun, Moon, Mars, etc. are plain doubles — use directly (no ?.)
                // Public users receive chart + basic details only (no DETAILED ganit payloads).
                var bhava = isAdmin ? workflowResult.Bhava : null;
                var dasha = isAdmin ? workflowResult.Dasha : null;
                var vargas = isAdmin ? workflowResult.Vargas : null;
                var ashtakavarga = isAdmin ? workflowResult.Ashtakavarga : null;
                var ganit = isAdmin ? workflowResult.Ganit : null;
                var resultData = new
                {
                    choice = workflowResult.Choice,
                    limitedPublic = !isAdmin,
                    lagna = lagnaObj,
                    lagnaDegrees = result.LagnaDegrees,
                    ascendantRashiName = result.AscendantRashiName ?? "",
                    midheaven = result.Midheaven,
                    sun = result.Sun,
                    moon = result.Moon,
                    mars = result.Mars,
                    mercury = result.Mercury,
                    jupiter = result.Jupiter,
                    venus = result.Venus,
                    saturn = result.Saturn,
                    rahu = result.Rahu,
                    ketu = result.Ketu,
                    ayanamsa = result.Ayanamsa,
                    ayanamsaRak = NirmalaAstrology_Web.Services.AstroMath.FormatAyanamsa(result.Ayanamsa),
                    siderealTime = result.SiderealTime,
                    siderealTimeRak = NirmalaAstrology_Web.Services.AstroMath.FormatSiderealTimeHours(result.SiderealTime),
                    lagnaRakv = NirmalaAstrology_Web.Services.AstroMath.FormatRakv(result.LagnaDegrees),
                    sunRakv = NirmalaAstrology_Web.Services.AstroMath.FormatRakv(result.Sun),
                    moonRakv = NirmalaAstrology_Web.Services.AstroMath.FormatRakv(result.Moon),
                    marsRakv = NirmalaAstrology_Web.Services.AstroMath.FormatRakv(result.Mars),
                    mercuryRakv = NirmalaAstrology_Web.Services.AstroMath.FormatRakv(result.Mercury),
                    jupiterRakv = NirmalaAstrology_Web.Services.AstroMath.FormatRakv(result.Jupiter),
                    venusRakv = NirmalaAstrology_Web.Services.AstroMath.FormatRakv(result.Venus),
                    saturnRakv = NirmalaAstrology_Web.Services.AstroMath.FormatRakv(result.Saturn),
                    rahuRakv = NirmalaAstrology_Web.Services.AstroMath.FormatRakv(result.Rahu),
                    ketuRakv = NirmalaAstrology_Web.Services.AstroMath.FormatRakv(result.Ketu),
                    houses = houses,
                    planetPositions = planetPositions,
                    details = kundliDetails,
                    bhava = bhava == null ? null : new
                    {
                        lagna = bhava.Lagna,
                        midheaven = bhava.Midheaven,
                        houses = bhava.Houses.Select(h => new
                        {
                            houseNumber = h.HouseNumber,
                            houseName = h.HouseName,
                            madhyaDegrees = h.MadhyaDegrees,
                            sandhiDegrees = h.SandhiDegrees,
                            madhyaRakv = h.MadhyaRakv,
                            sandhiRakv = h.SandhiRakv
                        }).ToList(),
                        chalitPlanets = bhava.ChalitPlanets.Select(p => new
                        {
                            planetName = p.PlanetName,
                            planetIndex = p.PlanetIndex,
                            longitude = p.Longitude,
                            chalitHouse = p.ChalitHouse,
                            rashiHouse = p.RashiHouse
                        }).ToList()
                    },
                    dasha = dasha == null ? null : new
                    {
                        birthLord = dasha.BirthLord,
                        balanceText = dasha.BalanceText,
                        balanceYears = dasha.BalanceYears,
                        balanceMonths = dasha.BalanceMonths,
                        balanceDays = dasha.BalanceDays,
                        birthDateLabel = dasha.BirthDateLabel,
                        mahaDashas = dasha.MahaDashas.Select(m => new
                        {
                            sequence = m.Sequence,
                            lord = m.Lord,
                            years = m.Years,
                            fromDate = m.FromDate,
                            toDate = m.ToDate,
                            ageLabel = m.AgeLabel
                        }).ToList(),
                        antarDashas = dasha.AntarDashas.Select(a => new
                        {
                            mahaSequence = a.MahaSequence,
                            mahaLord = a.MahaLord,
                            antarLord = a.AntarLord,
                            fromDate = a.FromDate,
                            toDate = a.ToDate,
                            label = a.Label
                        }).ToList()
                    },
                    vargas = vargas == null ? null : new
                    {
                        tableRows = vargas.TableRows.Select(r => new
                        {
                            index = r.Index,
                            name = r.Name,
                            code = r.Code,
                            lagna = r.PlanetSigns[1],
                            sun = r.PlanetSigns[2],
                            moon = r.PlanetSigns[3],
                            mars = r.PlanetSigns[4],
                            mercury = r.PlanetSigns[5],
                            jupiter = r.PlanetSigns[6],
                            venus = r.PlanetSigns[7],
                            saturn = r.PlanetSigns[8],
                            rahu = r.PlanetSigns[9],
                            ketu = r.PlanetSigns[10]
                        }).ToList(),
                        charts = vargas.Charts.Select(c => new
                        {
                            index = c.Index,
                            code = c.Code,
                            name = c.Name,
                            purpose = c.Purpose,
                            lagnaSign = c.LagnaSign,
                            lagnaSignName = c.LagnaSignName,
                            planets = c.Planets.Select(p => new
                            {
                                planetName = p.PlanetName,
                                sign = p.Sign,
                                signName = p.SignName,
                                houseFromLagna = p.HouseFromLagna
                            }).ToList()
                        }).ToList(),
                        deities = (vargas.Deities ?? new List<VargaDeityRow>()).Select(de => new
                        {
                            vargaCode = de.VargaCode,
                            vargaName = de.VargaName,
                            planetName = de.PlanetName,
                            deity = de.Deity
                        }).ToList()
                    },
                    ashtakavarga = ashtakavarga == null ? null : new
                    {
                        school = ashtakavarga.School,
                        sarvaBySign = Enumerable.Range(1, 12).Select(i => ashtakavarga.SarvaBySign[i]).ToList(),
                        trikonaSarvaBySign = Enumerable.Range(1, 12).Select(i => ashtakavarga.TrikonaSarvaBySign[i]).ToList(),
                        ekadhipatyaSarvaBySign = Enumerable.Range(1, 12).Select(i => ashtakavarga.EkadhipatyaSarvaBySign[i]).ToList(),
                        charts = ashtakavarga.Charts.Select(c => new
                        {
                            planetName = c.PlanetName,
                            totalBindus = c.TotalBindus,
                            bindusBySign = Enumerable.Range(1, 12).Select(i => c.BindusBySign[i]).ToList(),
                            trikonaBySign = Enumerable.Range(1, 12).Select(i => c.TrikonaBySign[i]).ToList(),
                            ekadhipatyaBySign = Enumerable.Range(1, 12).Select(i => c.EkadhipatyaBySign[i]).ToList()
                        }).ToList(),
                        pindas = ashtakavarga.Pindas.Select(p => new
                        {
                            planetName = p.PlanetName,
                            rasiPinda = p.RasiPinda,
                            grahaPinda = p.GrahaPinda,
                            totalPinda = p.TotalPinda
                        }).ToList()
                    },
                    ganit = ganit == null ? null : new
                    {
                        panchadha = ganit.Panchadha == null ? null : ganit.Panchadha.Rows.Select(r => new
                        {
                            planet = r.Planet,
                            relations = r.Relations
                        }).ToList(),
                        grahaBala = ganit.GrahaBala == null ? null : new
                        {
                            planets = ganit.GrahaBala.Planets.Select(p => new
                            {
                                planet = p.Planet,
                                uchha = p.Ucha,
                                saptaVarga = p.SaptaVarga,
                                sthana = p.Sthana,
                                dig = p.Dig,
                                kala = p.Kala,
                                cheshta = p.Cheshta,
                                naisarga = p.Naisarga,
                                drishti = p.Drishti,
                                shadbala = p.Shadbala,
                                rupas = p.Rupas,
                                requiredRupas = p.RequiredRupas,
                                percent = p.Percent
                            }).ToList(),
                            bhavas = ganit.GrahaBala.Bhavas.Select(b => new
                            {
                                house = b.House,
                                dig = b.Dig,
                                lordShadbala = b.LordShadbala,
                                total = b.Total
                            }).ToList()
                        },
                        specialLagnas = ganit.SpecialLagnas == null ? null : ganit.SpecialLagnas.Lagnas.Select(l => new
                        {
                            name = l.Name, sign = l.Sign, signName = l.SignName, longitude = l.Longitude
                        }).ToList(),
                        upGrahas = ganit.UpGrahas == null ? null : new
                        {
                            points = ganit.UpGrahas.Points.Select(MapNamedSign).ToList(),
                            dhoomadi = ganit.UpGrahas.Dhoomadi.Select(MapNamedSign).ToList(),
                            parasara = ganit.UpGrahas.Parasara.Select(MapNamedSign).ToList(),
                            mantreshwar = ganit.UpGrahas.Mantreshwar.Select(MapNamedSign).ToList()
                        },
                        avasthas = ganit.Avasthas == null ? null : ganit.Avasthas.Rows.Select(a => new
                        {
                            planet = a.Planet,
                            baladi = a.Baladi,
                            jagradadi = a.Jagradadi,
                            deeptadi = a.Deeptadi,
                            charadiKarak = a.CharadiKarak,
                            arohi = a.Arohi,
                            pravas = a.Pravas
                        }).ToList(),
                        kinshukadi = ganit.Kinshukadi == null ? null : ganit.Kinshukadi.Rows.Select(k => new
                        {
                            planet = k.Planet, shodasha = k.Shodasha, dasaVarga = k.DasaVarga, saptaVarga = k.SaptaVarga,
                            shodashaCount = k.ShodashaCount, dasaCount = k.DasaCount, saptaCount = k.SaptaCount
                        }).ToList(),
                        yogas = ganit.Yogas == null ? null : ganit.Yogas.Yogas.Select(y => new
                        {
                            name = y.Name, category = y.Category, note = y.Note
                        }).ToList(),
                        extraDashas = ganit.ExtraDashas == null ? null : new
                        {
                            ashtottari = MapAltDasha(ganit.ExtraDashas.Ashtottari),
                            ashtottariArdradi = MapAltDasha(ganit.ExtraDashas.AshtottariArdradi),
                            yogini = MapAltDasha(ganit.ExtraDashas.Yogini),
                            shodshottari = MapAltDasha(ganit.ExtraDashas.Shodshottari),
                            panchottari = MapAltDasha(ganit.ExtraDashas.Panchottari),
                            shatabadika = MapAltDasha(ganit.ExtraDashas.Shatabadika),
                            chaturshityabadika = MapAltDasha(ganit.ExtraDashas.Chaturshityabadika),
                            davisapatika = MapAltDasha(ganit.ExtraDashas.Davisapatika),
                            sashathihayani = MapAltDasha(ganit.ExtraDashas.Sashathihayani),
                            sandhya = MapAltDasha(ganit.ExtraDashas.Sandhya),
                            pachak = MapAltDasha(ganit.ExtraDashas.Pachak),
                            systems = (ganit.ExtraDashas.Systems ?? new List<AlternateDashaResult>())
                                .Select(MapAltDasha).ToList()
                        }
                    }
                };

                return Json(new { success = true, result = resultData });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[CalculateKundli Error] " + ex.Message);
                return Json(new { success = false, message = "An error occurred while calculating Kundli." });
            }
        }

        // GET: Home/Index
        public ActionResult Index()
        {
            // Admin must stay in Admin workspace — do not show public marketing modules.
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated
                && User.IsInRole(AuthService.RoleAdmin))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            HomeViewModel model = new HomeViewModel();
            try
            {
                model.ZodiacList = _zodiacRepo.GetAllZodiacs();
            }
            catch (Exception)
            {
                model.ZodiacList = GetStaticZodiacList();
            }
            if (model.ZodiacList == null || model.ZodiacList.Count == 0)
                model.ZodiacList = GetStaticZodiacList();

            model.TodayPanchang = GetTodayPanchangCached();
            model.TodayPanchangDateLabel = DateTime.Now.ToString("dddd, dd MMM yyyy");
            model.Testimonials = GetStaticTestimonials();
            model.FAQs = GetStaticFAQs();
            model.LatestArticles = GetLatestArticles();
            return View(model);
        }

        // POST: Home/GenerateKundli
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateKundli(CustomerModel model)
        {
            if (!ModelState.IsValid)
            {
                HomeViewModel vm = new HomeViewModel { Customer = model };
                try { vm.ZodiacList = _zodiacRepo.GetAllZodiacs(); } catch { vm.ZodiacList = GetStaticZodiacList(); }
                if (vm.ZodiacList == null || vm.ZodiacList.Count == 0) vm.ZodiacList = GetStaticZodiacList();
                vm.TodayPanchang = GetTodayPanchangCached();
                vm.TodayPanchangDateLabel = DateTime.Now.ToString("dddd, dd MMM yyyy");
                vm.Testimonials = GetStaticTestimonials();
                vm.FAQs = GetStaticFAQs();
                vm.LatestArticles = GetLatestArticles();
                return View("Index", vm);
            }
            try
            {
                BirthDataSessionStore.Set(Session, CreateBirthDataSession(model));
                int customerId = _customerRepo.SaveCustomer(model);
                TempData["SuccessMessage"] = "Kundli generated successfully! Customer ID: " + customerId;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while generating Kundli. Please try again.";
                System.Diagnostics.Trace.WriteLine("[GenerateKundli Error] " + ex.Message);
            }
            HomeViewModel vmError = new HomeViewModel { Customer = model };
            try { vmError.ZodiacList = _zodiacRepo.GetAllZodiacs(); } catch { vmError.ZodiacList = GetStaticZodiacList(); }
            if (vmError.ZodiacList == null || vmError.ZodiacList.Count == 0) vmError.ZodiacList = GetStaticZodiacList();
            vmError.TodayPanchang = GetTodayPanchangCached();
            vmError.TodayPanchangDateLabel = DateTime.Now.ToString("dddd, dd MMM yyyy");
            vmError.Testimonials = GetStaticTestimonials();
            vmError.FAQs = GetStaticFAQs();
            vmError.LatestArticles = GetLatestArticles();
            return View("Index", vmError);
        }

        private static BirthDataSessionModel CreateBirthDataSession(CustomerModel model)
        {
            double timezone;
            Double.TryParse(model.Timezone, out timezone);
            return new BirthDataSessionModel
            {
                Name = model.Name,
                Gender = String.IsNullOrWhiteSpace(model.Gender) ? "Male" : model.Gender,
                Day = model.DOB.Day,
                Month = model.DOB.Month,
                Year = model.DOB.Year,
                Hours = model.BirthTime.Hours,
                Minutes = model.BirthTime.Minutes,
                Latitude = Convert.ToDouble(model.Latitude),
                Longitude = Convert.ToDouble(model.Longitude),
                Timezone = timezone == 0 ? 5.30 : timezone,
                PlaceId = model.PlaceId,
                Place = model.PlaceName,
                ReportChoice = "DETAILED"
            };
        }

        // POST: Home/SaveCustomer (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize]
        public JsonResult SaveCustomer(CustomerModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = new List<string>();
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            errors.Add(error.ErrorMessage);
                        }
                    }
                    return Json(new { success = false, message = "Validation failed.", errors = errors });
                }
                int customerId = _customerRepo.SaveCustomer(model);
                return Json(new { success = true, message = "Kundli saved successfully!", customerId = customerId });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[SaveKundli Error] " + ex.Message);
                return Json(new { success = false, message = "Error saving Kundli." });
            }
        }

        // GET: Home/SearchPlace (AJAX)
        [HttpGet]
        public JsonResult SearchPlace(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword) || keyword.Length < 2)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
            try
            {
                var places = _placeRepo.SearchPlaces(keyword);
                var result = new List<object>();
                foreach (var p in places)
                {
                    result.Add(new
                    {
                        placeId = p.PlaceID,
                        placeName = p.PlaceName,
                        countryName = p.CountryName,
                        latitude = p.Latitude,
                        longitude = p.Longitude,
                        timezone = p.TimeZone,
                        smtCorr = p.SMTCorr ?? "",
                        displayText = p.DisplayText
                    });
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[SearchPlace Error] " + ex.Message);
                return Json(new { success = false, message = "Error searching places." }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Home/GetCountries (AJAX)
        [HttpGet]
        public JsonResult GetCountries()
        {
            try
            {
                var countries = _countryRepo.GetAllCountries();
                var result = new List<object>();
                foreach (var c in countries)
                {
                    result.Add(new
                    {
                        countryId = c.CountryID,
                        countryName = c.CountryName
                    });
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[GetCountries Error] " + ex.Message);
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Home/AddPlace (AJAX) — Admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize]
        public JsonResult AddPlace(PlaceModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = new List<string>();
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            errors.Add(error.ErrorMessage);
                        }
                    }
                    return Json(new { success = false, message = "Validation failed.", errors = errors });
                }
                int placeId = _placeRepo.InsertPlace(model);
                return Json(new { success = true, message = "Place added successfully!", placeId = placeId });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[AddPlace Error] " + ex.Message);
                return Json(new { success = false, message = "Error adding place." });
            }
        }

        #region Static Data Fallback

        private static object MapNamedSign(NamedSign l)
        {
            if (l == null) return null;
            return new { name = l.Name, sign = l.Sign, signName = l.SignName, longitude = l.Longitude };
        }

        private static object MapAltDasha(AlternateDashaResult d)
        {
            if (d == null) return null;
            return new
            {
                systemName = d.SystemName,
                birthLord = d.BirthLord,
                balanceText = d.BalanceText,
                periods = (d.Periods ?? new List<AlternateDashaPeriod>()).Select(p => new
                {
                    mahaLord = p.MahaLord,
                    antarLord = p.AntarLord,
                    fromDate = p.FromDate,
                    toDate = p.ToDate,
                    label = p.Label
                }).ToList()
            };
        }

        private static readonly object PanchangCacheLock = new object();
        private static LegacyPanchangModel _cachedTodayPanchang;
        private static DateTime _cachedTodayPanchangDay = DateTime.MinValue;

        private LegacyPanchangModel GetTodayPanchangCached()
        {
            lock (PanchangCacheLock)
            {
                if (_cachedTodayPanchang != null && _cachedTodayPanchangDay.Date == DateTime.Today)
                    return _cachedTodayPanchang;
            }

            try
            {
                var now = DateTime.Now;
                // Patiala reference for public "today" panchang card
                const double lat = 30.19;
                const double lon = 76.25;
                const double tz = 5.30;
                var calc = new KundliCalculator();
                var result = calc.CalculateKundli(now.Day, now.Month, now.Year, 12, 0, lon, lat, tz);
                var panchang = new LegacyPanchangCalculator().Calculate(result, new KundliWorkflowRequest
                {
                    Day = now.Day,
                    Month = now.Month,
                    Year = now.Year,
                    Hour = 12,
                    Minute = 0,
                    Latitude = lat,
                    Longitude = lon,
                    TimezoneOffset = tz
                });
                lock (PanchangCacheLock)
                {
                    _cachedTodayPanchang = panchang;
                    _cachedTodayPanchangDay = DateTime.Today;
                }
                return panchang;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[Home.TodayPanchang] " + ex.Message);
                return null;
            }
        }

        private List<ZodiacModel> GetStaticZodiacList()
        {
            return new List<ZodiacModel>
            {
                new ZodiacModel { ZodiacId = 1, Name = "Aries", HindiName = "Mesha", Icon = "fas fa-fire", Image = "", Description = "Aries is the first sign of the zodiac, known for courage, confidence, and enthusiasm. Ruled by Mars, Aries natives are natural-born leaders who thrive on challenges and adventure.", LuckyNumber = "1, 8, 17", LuckyColor = "Red", LuckyDay = "Tuesday", Planet = "Mars", Element = "Fire", Nature = "Cardinal", Strength = "Courageous, confident, enthusiastic, optimistic", Weakness = "Impatient, moody, short-tempered, impulsive", Career = "Leadership roles, entrepreneurship, sports, military", Love = "Passionate and adventurous in love", Health = "Prone to headaches and stress-related issues", Finance = "Bold financial decisions, good earners", Personality = "Dynamic and energetic, Aries individuals are always ready for action. They possess a natural magnetism that draws people toward them.", Characteristics = "Bold, ambitious, and energetic", CompatibleSigns = "Leo, Sagittarius, Gemini, Aquarius" },
                new ZodiacModel { ZodiacId = 2, Name = "Taurus", HindiName = "Vrishabha", Icon = "fas fa-leaf", Image = "", Description = "Taurus is an earth sign known for reliability, patience, and practicality. Ruled by Venus, Taurus natives appreciate beauty, comfort, and the finer things in life.", LuckyNumber = "2, 6, 9", LuckyColor = "Green, Pink", LuckyDay = "Friday", Planet = "Venus", Element = "Earth", Nature = "Fixed", Strength = "Reliable, patient, practical, devoted", Weakness = "Stubborn, possessive, uncompromising", Career = "Finance, banking, art, agriculture, architecture", Love = "Loyal and devoted partners", Health = "Throat and neck issues", Finance = "Financially stable, good savers", Personality = "Taurus individuals are grounded and practical. They value stability and are known for their unwavering determination.", Characteristics = "Dependable, patient, and sensual", CompatibleSigns = "Virgo, Capricorn, Cancer, Pisces" },
                new ZodiacModel { ZodiacId = 3, Name = "Gemini", HindiName = "Mithuna", Icon = "fas fa-wind", Image = "", Description = "Gemini is an air sign known for versatility, communication skills, and intellectual curiosity. Ruled by Mercury, Gemini natives are adaptable and love social interactions.", LuckyNumber = "3, 5, 7", LuckyColor = "Yellow, Light Green", LuckyDay = "Wednesday", Planet = "Mercury", Element = "Air", Nature = "Mutable", Strength = "Gentle, curious, adaptable, quick learner", Weakness = "Nervous, inconsistent, indecisive", Career = "Journalism, teaching, writing, sales, communication", Love = "Playful and communicative in relationships", Health = "Respiratory issues and anxiety", Finance = "Multiple income sources, versatile earners", Personality = "Gemini individuals are witty and charming. They have a dual nature that makes them fascinating conversationalists.", Characteristics = "Versatile, curious, and expressive", CompatibleSigns = "Libra, Aquarius, Aries, Leo" },
                new ZodiacModel { ZodiacId = 4, Name = "Cancer", HindiName = "Karka", Icon = "fas fa-water", Image = "", Description = "Cancer is a water sign known for emotional depth, intuition, and nurturing qualities. Ruled by the Moon, Cancer natives are deeply connected to family and home.", LuckyNumber = "2, 3, 15", LuckyColor = "White, Silver", LuckyDay = "Monday", Planet = "Moon", Element = "Water", Nature = "Cardinal", Strength = "Tenacious, highly imaginative, loyal, emotional", Weakness = "Moody, pessimistic, suspicious, manipulative", Career = "Healthcare, education, hospitality, real estate", Love = "Deeply caring and protective partners", Health = "Digestive system and stomach issues", Finance = "Cautious with money, good at saving", Personality = "Cancer individuals are deeply intuitive and sentimental. They value emotional security and family bonds above all else.", Characteristics = "Nurturing, intuitive, and protective", CompatibleSigns = "Scorpio, Pisces, Taurus, Virgo" },
                new ZodiacModel { ZodiacId = 5, Name = "Leo", HindiName = "Simha", Icon = "fas fa-sun", Image = "", Description = "Leo is a fire sign known for charisma, creativity, and natural leadership. Ruled by the Sun, Leo natives shine brightly and love being the center of attention.", LuckyNumber = "1, 3, 10", LuckyColor = "Gold, Orange", LuckyDay = "Sunday", Planet = "Sun", Element = "Fire", Nature = "Fixed", Strength = "Creative, passionate, generous, warm-hearted", Weakness = "Arrogant, stubborn, self-centered, lazy", Career = "Entertainment, management, politics, fashion", Love = "Generous and warm in love", Health = "Heart and spine issues", Finance = "Generous spenders, attract wealth", Personality = "Leo individuals are natural stars who command attention wherever they go. Their warmth and generosity make them beloved by many.", Characteristics = "Bold, charismatic, and confident", CompatibleSigns = "Aries, Sagittarius, Gemini, Libra" },
                new ZodiacModel { ZodiacId = 6, Name = "Virgo", HindiName = "Kanya", Icon = "fas fa-seedling", Image = "", Description = "Virgo is an earth sign known for analytical thinking, attention to detail, and practical service. Ruled by Mercury, Virgo natives are perfectionists with a strong sense of duty.", LuckyNumber = "5, 14, 23", LuckyColor = "Green, Brown", LuckyDay = "Wednesday", Planet = "Mercury", Element = "Earth", Nature = "Mutable", Strength = "Loyal, analytical, kind, hardworking", Weakness = "Shyness, overly critical, all work no play", Career = "Healthcare, research, editing, accounting", Love = "Supportive and detail-oriented partners", Health = "Digestive issues and anxiety", Finance = "Practical and careful with money", Personality = "Virgo individuals are meticulous and reliable. They have an innate ability to improve everything they touch.", Characteristics = "Analytical, practical, and humble", CompatibleSigns = "Taurus, Capricorn, Cancer, Scorpio" },
                new ZodiacModel { ZodiacId = 7, Name = "Libra", HindiName = "Tula", Icon = "fas fa-balance-scale", Image = "", Description = "Libra is an air sign known for balance, harmony, and aesthetic sense. Ruled by Venus, Libra natives seek justice, beauty, and peaceful relationships.", LuckyNumber = "4, 6, 13", LuckyColor = "Pink, Light Blue", LuckyDay = "Friday", Planet = "Venus", Element = "Air", Nature = "Cardinal", Strength = "Cooperative, diplomatic, gracious, fair-minded", Weakness = "Indecisive, avoids confrontation, self-pitying", Career = "Law, diplomacy, design, counseling", Love = "Romantic and harmonious in love", Health = "Kidney and lower back issues", Finance = "Balanced approach to finances", Personality = "Libra individuals are natural peacemakers who value harmony and beauty in all aspects of life.", Characteristics = "Diplomatic, graceful, and fair", CompatibleSigns = "Gemini, Aquarius, Leo, Sagittarius" },
                new ZodiacModel { ZodiacId = 8, Name = "Scorpio", HindiName = "Vrischika", Icon = "fas fa-spider", Image = "", Description = "Scorpio is a water sign known for intensity, passion, and transformative power. Ruled by Pluto, Scorpio natives are deeply emotional and fiercely loyal.", LuckyNumber = "8, 11, 18", LuckyColor = "Dark Red, Black", LuckyDay = "Tuesday", Planet = "Pluto", Element = "Water", Nature = "Fixed", Strength = "Resourceful, brave, passionate, loyal", Weakness = "Distrusting, jealous, secretive, violent", Career = "Research, investigation, surgery, psychology", Love = "Intensely passionate and loyal partners", Health = "Reproductive system and intestines", Finance = "Strategic with investments", Personality = "Scorpio individuals are powerful and magnetic. They possess a depth of emotion that few can match.", Characteristics = "Intense, mysterious, and determined", CompatibleSigns = "Cancer, Pisces, Virgo, Capricorn" },
                new ZodiacModel { ZodiacId = 9, Name = "Sagittarius", HindiName = "Dhanu", Icon = "fas fa-horse", Image = "", Description = "Sagittarius is a fire sign known for optimism, adventure, and philosophical thinking. Ruled by Jupiter, Sagittarius natives love freedom and exploration.", LuckyNumber = "3, 7, 9", LuckyColor = "Purple, Blue", LuckyDay = "Thursday", Planet = "Jupiter", Element = "Fire", Nature = "Mutable", Strength = "Generous, idealistic, great sense of humor", Weakness = "Promises more than can deliver, impatient, tactless", Career = "Travel, education, publishing, philosophy", Love = "Adventurous and honest in love", Health = "Hip and thigh issues", Finance = "Optimistic about money, lucky with investments", Personality = "Sagittarius individuals are free spirits who love adventure. Their optimism is contagious and their humor is legendary.", Characteristics = "Adventurous, optimistic, and philosophical", CompatibleSigns = "Aries, Leo, Libra, Aquarius" },
                new ZodiacModel { ZodiacId = 10, Name = "Capricorn", HindiName = "Makara", Icon = "fas fa-mountain", Image = "", Description = "Capricorn is an earth sign known for discipline, ambition, and practical wisdom. Ruled by Saturn, Capricorn natives are determined achievers who value structure.", LuckyNumber = "4, 8, 13", LuckyColor = "Brown, Black, Dark Green", LuckyDay = "Saturday", Planet = "Saturn", Element = "Earth", Nature = "Cardinal", Strength = "Responsible, disciplined, self-control, good managers", Weakness = "Know-it-all, unforgiving, condescending, expecting the worst", Career = "Business, management, engineering, government", Love = "Loyal and committed partners", Health = "Bones, joints, and teeth issues", Finance = "Excellent financial planners, long-term thinkers", Personality = "Capricorn individuals are the achievers of the zodiac. Their discipline and determination help them reach the highest peaks.", Characteristics = "Ambitious, disciplined, and wise", CompatibleSigns = "Taurus, Virgo, Scorpio, Pisces" },
                new ZodiacModel { ZodiacId = 11, Name = "Aquarius", HindiName = "Kumbha", Icon = "fas fa-tint", Image = "", Description = "Aquarius is an air sign known for innovation, humanitarianism, and independent thinking. Ruled by Uranus, Aquarius natives are visionaries ahead of their time.", LuckyNumber = "4, 7, 11", LuckyColor = "Turquoise, Blue", LuckyDay = "Saturday", Planet = "Uranus", Element = "Air", Nature = "Fixed", Strength = "Progressive, original, independent, humanitarian", Weakness = "Runs from emotional expression, temperamental, uncompromising", Career = "Technology, science, social work, activism", Love = "Intellectual and unconventional in love", Health = "Circulatory system and ankles", Finance = "Unconventional but successful with money", Personality = "Aquarius individuals are the innovators of the zodiac. They think differently and are driven by a desire to make the world better.", Characteristics = "Innovative, independent, and humanitarian", CompatibleSigns = "Gemini, Libra, Aries, Sagittarius" },
                new ZodiacModel { ZodiacId = 12, Name = "Pisces", HindiName = "Meena", Icon = "fas fa-fish", Image = "", Description = "Pisces is a water sign known for compassion, artistic talent, and spiritual depth. Ruled by Neptune, Pisces natives are dreamers with profound emotional intelligence.", LuckyNumber = "3, 9, 12", LuckyColor = "Sea Green, Lavender", LuckyDay = "Thursday", Planet = "Neptune", Element = "Water", Nature = "Mutable", Strength = "Compassionate, artistic, intuitive, gentle, wise", Weakness = "Fearful, overly trusting, sad, desire to escape reality", Career = "Arts, music, healing, spirituality, film", Love = "Romantic and selfless in love", Health = "Feet and immune system issues", Finance = "Generous but needs to be careful with spending", Personality = "Pisces individuals are the most empathetic of the zodiac. Their artistic sensibility and spiritual depth make them truly unique.", Characteristics = "Compassionate, artistic, and intuitive", CompatibleSigns = "Cancer, Scorpio, Taurus, Capricorn" }
            };
        }

        private List<Testimonial> GetStaticTestimonials()
        {
            return new List<Testimonial>
            {
                new Testimonial { Name = "Rajesh Kumar", Location = "Delhi", Text = "Nirmala Astrology provided incredibly accurate predictions about my career. Their guidance helped me make the right decisions at the right time.", Rating = "5" },
                new Testimonial { Name = "Priya Sharma", Location = "Mumbai", Text = "The Kundli analysis was detailed and insightful. I was amazed at how precisely they described my personality and life events.", Rating = "5" },
                new Testimonial { Name = "Amit Patel", Location = "Ahmedabad", Text = "I consulted them for match making and the compatibility analysis was spot on. Highly recommended for anyone seeking genuine astrological guidance.", Rating = "5" },
                new Testimonial { Name = "Sneha Gupta", Location = "Bangalore", Text = "Their numerology consultation changed my perspective on life. The remedies suggested were practical and effective.", Rating = "4" }
            };
        }

        private List<FAQItem> GetStaticFAQs()
        {
            return new List<FAQItem>
            {
                new FAQItem { Question = "What is Kundli?", Answer = "Kundli (also known as Janam Kundli or Birth Chart) is a detailed astrological chart prepared based on the exact date, time, and place of birth. It maps the positions of planets and celestial bodies at the time of your birth and is used to analyze various aspects of your life including personality, career, health, and relationships." },
                new FAQItem { Question = "How accurate is online Kundli generation?", Answer = "Our Kundli generation system uses precise astronomical calculations based on Swiss Ephemeris data. When you provide accurate birth details including the exact time and place, the generated Kundli is highly accurate and matches manually prepared charts by expert astrologers." },
                new FAQItem { Question = "What information do I need to generate my Kundli?", Answer = "To generate an accurate Kundli, you need three key pieces of information: your exact date of birth, your exact time of birth (preferably from a birth certificate), and your place of birth. The more precise your birth time, the more accurate your Kundli will be." },
                new FAQItem { Question = "Is match making (Kundli Milan) necessary for marriage?", Answer = "In Vedic astrology, Kundli Milan (also called Guna Milan) is a traditional practice to assess compatibility between prospective partners. It analyzes 36 Gunas (qualities) and checks for Mangal Dosha. While it is a valuable tool for understanding compatibility, it should be considered as guidance rather than the sole deciding factor." },
                new FAQItem { Question = "How can I consult with an astrologer?", Answer = "You can consult with our experienced astrologers by booking a session through our website. We offer both online (video call) and in-person consultations. Our team of scholars, established since 1974, provides authentic and detailed astrological guidance." },
                new FAQItem { Question = "What services does Nirmala Astrology offer?", Answer = "Nirmala Astrology offers a comprehensive range of services including Kundli generation and analysis, Match Making (Kundli Milan), Numerology, Palm Reading, Daily and Yearly Horoscopes, Vastu consultation, Gemstone recommendations, and personalized remedies for various life concerns." }
            };
        }

        private List<ArticlePreview> GetLatestArticles()
        {
            try
            {
                var svc = new ArticleService();
                var published = svc.GetPublished(6);
                var list = new List<ArticlePreview>();
                foreach (var a in published)
                {
                    list.Add(new ArticlePreview
                    {
                        ArticleId = a.ArticleId,
                        Title = a.Title,
                        Description = a.Summary,
                        ImageUrl = a.ImageUrl,
                        Slug = a.Slug
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[Home.Articles] " + ex.Message);
                return new List<ArticlePreview>();
            }
        }

        #endregion
    }
}
