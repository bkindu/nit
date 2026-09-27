const fs = require("fs");
const vm = require("vm");
const code = fs.readFileSync("E:/NirmalaAstrology/NirmalaAstrology_Web/Scripts/Custom/kundli-reference-engine.js", "utf8");
const ctx = { console };
vm.createContext(ctx);
vm.runInContext(code, ctx);
ctx.Kepler(8, 2, 1981, 7, 47, 76.25, 30.19, 5.30);
const p = ctx.resultPlt;
const names = ["","Lagna","Sun","Moon","Mars","Mercury","Jupiter","Venus","Saturn","Rahu","Ketu"];
const csharp = {
  Lagna: 306.22945345736,
  Sun: 295.65536604309438,
  Moon: 337.15742985697057,
  Mars: 307.30985535094339,
  Mercury: 311.30924640533743,
  Jupiter: 166.46963076728258,
  Venus: 281.22079197244807,
  Saturn: 165.83228675155664,
  Rahu: 106.90975602641593,
  Ayan: 23.592103
};
const map = {1:"Lagna",2:"Sun",3:"Moon",4:"Mars",5:"Mercury",6:"Jupiter",7:"Venus",8:"Saturn",9:"Rahu"};
for (const i of [1,2,3,4,5,6,7,8,9]) {
  const name = map[i];
  const js = p[i];
  const cs = csharp[name];
  const d = Math.abs(cs - js);
  console.log(`${name.padEnd(8)} JS=${js.toFixed(9)} C#=${cs.toFixed(9)} Δ=${d.toExponential(3)}`);
}
console.log(`Ayan     JS=${ctx.dAyanamsa} C#=${csharp.Ayan} Δ=${Math.abs(csharp.Ayan - ctx.dAyanamsa)}`);
