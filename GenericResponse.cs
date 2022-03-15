using System;
namespace Bev.Instruments.ND281
{
    public class GenericResponse
    {
        public GenericResponse()
        {
            Reset();
        }

        public double Value { get; private set; }
        public string ResponseLine { get; private set; }
        public Unit MeasurementUnit { get; private set; }
        public Sign Sign { get; private set; }
        public Sorting SortingStatus { get; private set; }
        public MeasurementSeries SeriesStatus { get; private set; }

        public void ParseInstrumentResponse(string response)
        {
            Reset();
            ResponseLine = RemoveCrLfFromString(response);

        }

        private string RemoveCrLfFromString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            return str.Replace("\n", "").Replace("\r", "");
        }

        private void Reset()
        {
            Value = double.NaN;
            ResponseLine = string.Empty;
            MeasurementUnit = Unit.None;
            Sign = Sign.None;
            SortingStatus = Sorting.None;
            SeriesStatus = MeasurementSeries.None;
        }

    }

    public enum Unit
    {
        None,
        MM,
        Inch,
        Degree,
        DMS,
        Rad,
        Fault
    }

    public enum Sign
    {
        None,
        Positive,
        Negative,
        Fault
    }

    public enum Sorting // not for ND280 ?
    {
        None,
        Greater,
        Less,
        Equal,
        Fault
    }

    public enum MeasurementSeries // not for ND280 ?
    {
        None,
        Min,
        Actual,
        Max,
        Diff,
        Fault
    }
}
