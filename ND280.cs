
namespace Bev.Instruments.Heidenhain
{
    public class ND280 : HeidenhainBase, IHeidenhain
    {
        public ND280(string portName) : base(portName) { }
        public override string InstrumentType => "ND280";
    }
}
