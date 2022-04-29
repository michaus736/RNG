using RNG;
using NAudio;
using NAudio.Wave;


var extractor= new Extractor();
await extractor.GetSamples();
extractor.Parser();
Console.WriteLine(123);

//testing


