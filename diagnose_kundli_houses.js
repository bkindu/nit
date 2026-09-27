const fs = require('fs');
const vm = require('vm');
const code = fs.readFileSync('/home/ubuntu/nirmala_astrology_project/NirmalaAstrology_Web/Scripts/Custom/kundli-reference-engine.js','utf8');
const context = { console: console, Math: Math, Number: Number, parseInt: parseInt, parseFloat: parseFloat, Date: Date };
vm.createContext(context); vm.runInContext(code, context);
context.Kepler(8,2,1981,7,47,76.25,30.19,5.3);
const names=['Lagna','Sun','Moon','Mars','Mercury','Jupiter','Venus','Saturn','Rahu','Ketu'];
function rashi(x){ return ((Math.floor(x)%12)+12)%12+1; }
for(let i=1;i<=10;i++) console.log(names[i-1], context.resultPlt[i], context.resultPltS[i], rashi(context.resultPltS[i]));
