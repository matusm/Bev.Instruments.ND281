
namespace Bev.Instruments.Heidenhain
{
    public class ND281 : HeidenhainBase
    {
        public ND281(string portName) : base(portName) { }
        public override string InstrumentType => "ND281B";
    }
}
