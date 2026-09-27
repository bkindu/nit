using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// MVP port of VB6 m1Marriage_Muhurata / VivahMuhurtaPuja Jantri Saahe lookup.
    /// Date-range search of lagna slots + remarks (English) from Access export JSON.
    /// Data coverage in Indu.mdb seed: 2016-04-16 .. 2018-03-12.
    /// </summary>
    public sealed class VivahMuhurtaService
    {
        private static readonly string[] LagnaNames =
        {
            "", "Mesh (1)", "Vrishabh (2)", "Mithun (3)", "Kark (4)", "Simha (5)", "Kanya (6)",
            "Tula (7)", "Vrischik (8)", "Dhanu (9)", "Makar (10)", "Kumbh (11)", "Meen (12)", "Godhuli (13)"
        };

        private readonly string _jsonPath;

        public VivahMuhurtaService(string jsonPath)
        {
            _jsonPath = jsonPath;
        }

        public VivahMuhurtaResult Search(DateTime from, DateTime to)
        {
            if (to < from)
            {
                var tmp = from;
                from = to;
                to = tmp;
            }

            var result = new VivahMuhurtaResult
            {
                From = from.Date,
                To = to.Date,
                CoverageNote = "Jantri Saahe from Access VivahMuhurtaPuja (2016-04-16 to 2018-03-12). Not a live muhurta engine."
            };

            var rows = LoadRows();
            foreach (var row in rows)
            {
                DateTime dt;
                if (!DateTime.TryParseExact(row.date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    continue;
                if (dt.Date < from.Date || dt.Date > to.Date) continue;
                if (row.slots == null || row.slots.Count == 0) continue;

                var day = new VivahMuhurtaDay
                {
                    Date = dt.Date,
                    DateLabel = dt.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                    Weekday = dt.ToString("dddd", CultureInfo.InvariantCulture)
                };
                foreach (var slot in row.slots)
                {
                    if (slot == null || slot.lagna < 1 || slot.lagna > 13) continue;
                    day.Slots.Add(new VivahMuhurtaSlot
                    {
                        Lagna = slot.lagna,
                        LagnaName = slot.lagna <= 13 ? LagnaNames[slot.lagna] : ("Lagna " + slot.lagna),
                        Remark = (slot.remark ?? "").Trim()
                    });
                }
                if (day.Slots.Count > 0)
                    result.Days.Add(day);
            }
            return result;
        }

        private List<PujaRow> LoadRows()
        {
            if (string.IsNullOrEmpty(_jsonPath) || !File.Exists(_jsonPath))
                return new List<PujaRow>();
            try
            {
                var rows = JsonConvert.DeserializeObject<List<PujaRow>>(File.ReadAllText(_jsonPath));
                return rows ?? new List<PujaRow>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[VivahMuhurta JSON] " + ex.Message);
                return new List<PujaRow>();
            }
        }

        private sealed class PujaRow
        {
            public string date { get; set; }
            public List<PujaSlot> slots { get; set; }
        }

        private sealed class PujaSlot
        {
            public int lagna { get; set; }
            public string remark { get; set; }
        }
    }
}
