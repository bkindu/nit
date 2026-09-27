using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.Services
{
    /// <summary>
    /// Port of VB6 Palmistry.frm: Intro_Palm + selected PalmIndex IDs + optional Mount / Lucky / Remedy.
    /// English readings from App_Data JSON exports of Access Palmistry / Brg_Reading / Numerology_Reading / Upay.
    /// </summary>
    public sealed class PalmistryService
    {
        private static readonly string[] MountCodes =
        {
            "SR", "CN", "MG", "BD", "GR", "SK", "SN", "RH", "KT",
            "SRCN", "SRMG", "SRBD", "SRGR", "SRSK", "SKSN", "SRRH", "SRKT",
            "CNMG", "CNBD", "CNGR", "CNSK", "CNSN", "CNRH", "CNKT",
            "MGBD", "MGGR", "MGSK", "MGSN", "MGRH", "MGKT",
            "BDGR", "BDSK", "BDSN", "BDRH", "BDKT",
            "GRSK", "GRSN", "GRRH", "GRKT",
            "SKSN", "SKRH", "SKKT", "SNRH", "SNKT",
            "R1", "R2", "R3", "R4", "R5", "R6", "R7", "R8", "R9", "R10", "R11", "R12"
        };

        private static readonly string[] RemedyCodes =
        {
            "SRU", "CNU", "MGU", "BDU", "GRU", "SKU", "SNU", "RHU", "KTU"
        };

        private readonly string _indexPath;
        private readonly string _readingsPath;
        private readonly string _mountPath;
        private readonly string _luckyPath;
        private readonly string _remedyPath;

        public PalmistryService(
            string indexPath,
            string readingsPath,
            string mountPath,
            string luckyPath,
            string remedyPath)
        {
            _indexPath = indexPath;
            _readingsPath = readingsPath;
            _mountPath = mountPath;
            _luckyPath = luckyPath;
            _remedyPath = remedyPath;
        }

        public List<PalmistryOption> LoadOptions()
        {
            var options = new List<PalmistryOption>();
            if (string.IsNullOrEmpty(_indexPath) || !File.Exists(_indexPath))
                return options;

            string group = "General";
            foreach (var raw in File.ReadAllLines(_indexPath))
            {
                var line = (raw ?? "").Trim();
                if (line.Length == 0) continue;

                var open = line.LastIndexOf('(');
                var close = line.LastIndexOf(')');
                if (open < 0 || close <= open)
                {
                    group = line;
                    continue;
                }

                var id = line.Substring(open + 1, close - open - 1).Trim();
                if (id.Length == 0) continue;
                options.Add(new PalmistryOption
                {
                    Id = id,
                    Label = line,
                    Group = group
                });
            }
            return options;
        }

        public IEnumerable<SelectListItem> MountSelectList(string selected)
        {
            yield return new SelectListItem { Text = "(Mount)", Value = "", Selected = string.IsNullOrEmpty(selected) };
            foreach (var code in MountCodes)
                yield return new SelectListItem { Text = code, Value = code, Selected = string.Equals(code, selected, StringComparison.OrdinalIgnoreCase) };
        }

        public IEnumerable<SelectListItem> LuckySelectList(string selected)
        {
            yield return new SelectListItem { Text = "(Lucky)", Value = "", Selected = string.IsNullOrEmpty(selected) };
            for (var n = 1; n <= 9; n++)
            {
                var code = "N" + n;
                yield return new SelectListItem { Text = code, Value = code, Selected = string.Equals(code, selected, StringComparison.OrdinalIgnoreCase) };
            }
        }

        public IEnumerable<SelectListItem> RemedySelectList(string selected)
        {
            yield return new SelectListItem { Text = "(Remedy)", Value = "", Selected = string.IsNullOrEmpty(selected) };
            foreach (var code in RemedyCodes)
                yield return new SelectListItem { Text = code, Value = code, Selected = string.Equals(code, selected, StringComparison.OrdinalIgnoreCase) };
        }

        public PalmistryResult Build(
            string nativeName,
            IEnumerable<string> selectedIds,
            string mountCode,
            string luckyCode,
            string remedyCode)
        {
            var palm = LoadMap(_readingsPath);
            var mount = LoadMap(_mountPath);
            var lucky = LoadMap(_luckyPath);
            var remedy = LoadMap(_remedyPath);
            var options = LoadOptions().ToDictionary(o => o.Id, o => o.Label, StringComparer.OrdinalIgnoreCase);

            var result = new PalmistryResult
            {
                NativeName = string.IsNullOrWhiteSpace(nativeName) ? "Native" : nativeName.Trim()
            };

            Append(result, "Intro_Palm", "Introduction", palm);
            var ids = (selectedIds ?? Enumerable.Empty<string>())
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var id in ids)
            {
                string label;
                if (!options.TryGetValue(id, out label))
                    label = id;
                Append(result, id, label, palm);
            }

            if (!string.IsNullOrWhiteSpace(mountCode) &&
                !mountCode.Equals("Mount", StringComparison.OrdinalIgnoreCase))
            {
                Append(result, mountCode.Trim().ToUpperInvariant(), "Mount " + mountCode.Trim().ToUpperInvariant(), mount);
            }

            if (!string.IsNullOrWhiteSpace(luckyCode) &&
                !luckyCode.Equals("Lucky", StringComparison.OrdinalIgnoreCase))
            {
                var code = luckyCode.Trim().ToUpperInvariant();
                var numKey = code.StartsWith("N", StringComparison.OrdinalIgnoreCase) && code.Length > 1
                    ? code.Substring(1)
                    : code;
                string text;
                if (!lucky.TryGetValue(numKey, out text) || string.IsNullOrWhiteSpace(text))
                    lucky.TryGetValue(code, out text);
                result.Sections.Add(new PalmistrySection
                {
                    Code = code,
                    Label = "Lucky " + code,
                    Text = string.IsNullOrWhiteSpace(text) ? "(No reading for " + code + ")" : text.Trim()
                });
            }

            if (!string.IsNullOrWhiteSpace(remedyCode) &&
                !remedyCode.Equals("Remedy", StringComparison.OrdinalIgnoreCase))
            {
                Append(result, remedyCode.Trim().ToUpperInvariant(), "Remedy " + remedyCode.Trim().ToUpperInvariant(), remedy);
            }

            var sb = new StringBuilder();
            sb.AppendLine("PALM READING OF " + result.NativeName);
            sb.AppendLine();
            foreach (var section in result.Sections)
            {
                if (sb.Length > 0) sb.AppendLine();
                sb.AppendLine(section.Text);
            }
            result.FullReading = sb.ToString().TrimEnd();
            return result;
        }

        private static void Append(PalmistryResult result, string code, string label, Dictionary<string, string> map)
        {
            string text;
            if (!map.TryGetValue(code, out text) || string.IsNullOrWhiteSpace(text))
                text = "(No reading for " + code + ")";
            result.Sections.Add(new PalmistrySection
            {
                Code = code,
                Label = label,
                Text = text.Trim()
            });
        }

        private static Dictionary<string, string> LoadMap(string path)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return map;
            try
            {
                var rows = JsonConvert.DeserializeObject<List<ReadingRow>>(File.ReadAllText(path));
                if (rows == null) return map;
                foreach (var row in rows)
                {
                    if (row == null || string.IsNullOrWhiteSpace(row.id)) continue;
                    map[row.id.Trim()] = row.text ?? "";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[Palmistry JSON] " + path + ": " + ex.Message);
            }
            return map;
        }

        private sealed class ReadingRow
        {
            public string id { get; set; }
            public string text { get; set; }
        }
    }
}
