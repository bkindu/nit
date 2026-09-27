const fs = require("fs");
const vm = require("vm");
const code = fs.readFileSync("E:/NirmalaAstrology/NirmalaAstrology_Web/Scripts/Custom/kundli-reference-engine.js", "utf8");
const ctx = { console };
vm.createContext(ctx);
vm.runInContext(code, ctx);
ctx.Kepler(8, 2, 1981, 7, 47, 76.4, 30.33, 5.5);
const plt = ctx.resultPlt;
function FNR(dX, dCY) { const FNFR = x => x - Math.floor(x); return FNFR(FNFR(dX / dCY) + 2) * dCY; }
function FNTGR(x) { return parseInt(x + 0.0000005); }
function FNFR(x) { return x - Math.floor(x); }
const moon = plt[3], lagna = plt[1], mc = plt[11];
const dNak = 3 / 40 * FNR(moon, 120);
const lord = FNTGR(dNak);
const years = [7, 20, 6, 10, 7, 18, 16, 19, 17][lord];
const bhogya = (1 - FNFR(dNak)) * years;
const iY = Math.floor(bhogya);
const dX = FNFR(bhogya) * 12;
const iM = Math.floor(dX);
const iD = Math.floor(FNFR(dX) * 30 + 0.5);
const lords = ["Ketu", "Sukra", "Surya", "Chandra", "Mangla", "Rahu", "Guru", "Sani", "Budha"];
console.log(JSON.stringify({ lagna, moon, mc, birthLord: lords[lord], balance: iY + "y " + iM + "m " + iD + "d" }, null, 2));
