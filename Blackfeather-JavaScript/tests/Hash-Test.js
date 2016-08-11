var Stopwatch = require("node-stopwatch").Stopwatch;
var stopwatch = Stopwatch.create();

stopwatch.start();
var Blackfeather = require('../blackfeather-1.0.0');
var text = "caw caw caw!"; // any text should work Utf8, Utf16 have been tested.
var rounds = 1; // more rounds = more cpu, more cpu = more security, must match when re-computed.

var hash = new Blackfeather.Security.Cryptology.Hash().Compute(text, rounds);
stopwatch.stop();

console.log("ticks: " + stopwatch.elapsedTicks);
console.log("milliseconds: " + stopwatch.elapsedMilliseconds);
console.log("seconds: " + stopwatch.elapsed.seconds);
console.log("minutes: " + stopwatch.elapsed.minutes);
console.log("\n");

console.log("example hash: " + hash.toJSON());