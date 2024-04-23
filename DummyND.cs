using System;

namespace Bev.Instruments.Heidenhain
{
    public class DummyND : IHeidenhain
    {
        private readonly Random r;

        public DummyND() => r = new Random();

        public string InstrumentManufacturer => "Heidenhain";

        public string InstrumentType => "Dummy";

        public string InstrumentID => $"{InstrumentManufacturer} {InstrumentType}";

        public double GetValue() => r.NextDouble() * 100;
    }
}
