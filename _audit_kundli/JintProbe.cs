using System;
using System.IO;
using System.Reflection;
using Jint;
class Program {
  static int Main() {
    try {
      var dir = @"E:\NirmalaAstrology\NirmalaAstrology_Web\Scripts\Custom";
      var engine = new Engine();
      engine.Execute("var globalThis = this; var window = this;");
      var vsop = Path.Combine(dir, "vsop87-earth-sun.js");
      Console.WriteLine("VSOP exists=" + File.Exists(vsop));
      if (File.Exists(vsop)) {
        try { engine.Execute(File.ReadAllText(vsop)); Console.WriteLine("VSOP OK"); }
        catch (Exception ex) { Console.WriteLine("VSOP_FAIL: " + ex.Message); }
      }
      engine.Execute(File.ReadAllText(Path.Combine(dir, "kundli-reference-engine.js")));
      Console.WriteLine("ENGINE OK");
      engine.Invoke("Kepler", 14, 9, 2026, 22, 8, 76.24, 30.19, 5.30);
      var plt = engine.GetValue("resultPlt");
      Console.WriteLine("OK lagna=" + plt.Get("1") + " sun=" + plt.Get("2") + " moon=" + plt.Get("3"));
      Console.WriteLine("LBR typeof=" + engine.Evaluate("typeof LBR_For_Earth_or_Sun").AsString());

      // Full calculator path
      var asm = Assembly.LoadFrom(@"E:\NirmalaAstrology\_audit_kundli\NirmalaAstrology_Web.dll");
      var calcType = asm.GetType("NirmalaAstrology_Web.Services.WorkingJavascriptKundliCalculator");
      var calc = Activator.CreateInstance(calcType, Path.Combine(dir, "kundli-reference-engine.js"));
      var method = calcType.GetMethod("CalculateKundli");
      var result = method.Invoke(calc, new object[] { 14, 9, 2026, 22, 8, 76.24, 30.19, 5.30 });
      Console.WriteLine("CALC OK type=" + result.GetType().FullName);
      var lagnaProp = result.GetType().GetProperty("Lagna");
      Console.WriteLine("CALC Lagna=" + lagnaProp.GetValue(result));

      // PDF path
      var wfType = asm.GetType("NirmalaAstrology_Web.Services.KundliWorkflowService");
      var wf = Activator.CreateInstance(wfType, Path.Combine(dir, "kundli-reference-engine.js"));
      var reqType = asm.GetType("NirmalaAstrology_Web.Services.KundliWorkflowRequest");
      var req = Activator.CreateInstance(reqType);
      reqType.GetProperty("Day").SetValue(req, 14);
      reqType.GetProperty("Month").SetValue(req, 9);
      reqType.GetProperty("Year").SetValue(req, 2026);
      reqType.GetProperty("Hour").SetValue(req, 22);
      reqType.GetProperty("Minute").SetValue(req, 8);
      reqType.GetProperty("Latitude").SetValue(req, 30.19);
      reqType.GetProperty("Longitude").SetValue(req, 76.24);
      reqType.GetProperty("TimezoneOffset").SetValue(req, 5.30);
      var gen = wfType.GetMethod("Generate");
      var workflow = gen.Invoke(wf, new object[] { "OPT_KUNDLI", req });
      Console.WriteLine("WORKFLOW OK");

      var birthType = asm.GetType("NirmalaAstrology_Web.Models.BirthDataSessionModel");
      var birth = Activator.CreateInstance(birthType);
      birthType.GetProperty("Name").SetValue(birth, "Test");
      birthType.GetProperty("Day").SetValue(birth, 14);
      birthType.GetProperty("Month").SetValue(birth, 9);
      birthType.GetProperty("Year").SetValue(birth, 2026);
      birthType.GetProperty("Hours").SetValue(birth, 22);
      birthType.GetProperty("Minutes").SetValue(birth, 8);
      birthType.GetProperty("Place").SetValue(birth, "Patiala");
      birthType.GetProperty("Latitude").SetValue(birth, 30.19);
      birthType.GetProperty("Longitude").SetValue(birth, 76.24);
      birthType.GetProperty("Timezone").SetValue(birth, 5.30);

      var builderType = asm.GetType("NirmalaAstrology_Web.Services.Reports.PublicLimitedReportBuilder");
      var builder = Activator.CreateInstance(builderType);
      var doc = builderType.GetMethod("BuildKundliTwoPage").Invoke(builder, new object[] { birth, workflow });
      Console.WriteLine("DOC OK");

      var letterhead = @"E:\NirmalaAstrology\NirmalaAstrology_Web\Images\Indu_Visiting_LetterHead.png";
      var rendererType = asm.GetType("NirmalaAstrology_Web.Services.Reports.LetterheadPdfReportRenderer");
      var renderer = Activator.CreateInstance(rendererType, letterhead);
      var bytes = (byte[])rendererType.GetMethod("Render").Invoke(renderer, new object[] { doc });
      Console.WriteLine("PDF OK bytes=" + bytes.Length);
      File.WriteAllBytes(Path.Combine(dir, "basic_horoscope_test.pdf"), bytes);
      return 0;
    } catch (Exception ex) {
      Console.WriteLine("FAIL: " + ex);
      if (ex.InnerException != null) Console.WriteLine("INNER: " + ex.InnerException);
      return 1;
    }
  }
}
