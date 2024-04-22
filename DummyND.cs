using System;

namespace Bev.Instruments.Heidenhain
{
    public class DummyND : IHeidenhain
    {
        public string InstrumentManufacturer => "Heidenhain";

        public string InstrumentType => "Dummy";

        public string InstrumentID => $"{InstrumentManufacturer} {InstrumentType}";

        public double GetValue() => Math.PI;
    }
}
