using System;

namespace FullInspector.Samples.Other.Limits {
    public class LimitsAttribute : Attribute {
        public float Min;
        public float Max;

        public LimitsAttribute(float min, float max) {
            Min = min;
            Max = max;
        }
    }
}