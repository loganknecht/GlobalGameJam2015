using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullInspector.Samples.Other.Limits {
    [AddComponentMenu("Full Inspector Samples/Other/Limits")]
    public class LimitsBehavior : BaseBehavior<JsonNetSerializer> {
        [Limits(1f, 3.5f)]
        public float Range;

        [Limits(1f, 3.5f)]
        public int Range2;
    }
}