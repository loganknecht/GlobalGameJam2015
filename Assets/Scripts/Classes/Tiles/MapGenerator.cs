// using UnityEngine;
// using System.Collections;

// public class MapGenerator : MonoBehaviour {
//   public const int StartX = 0;
//   public const int StartY = 0;

//   public struct Options {
//      // ~900 for 3-minute level at 5 tiles/sec
//     uint LengthMin;
//     uint LengthMax;

//     // ~10, then tweak
//     uint PlatformLengthMin;
//     uint PlatformLengthMax;

//     // horizontal gap size, 1-3 or something
//     uint GapMin;
//     uint GapMax;

//     // -1 to 5 ?
//     int GapStepMin;
//     int GapStepMax;
//   }


//   private List<Tile> Generate(Options options) {
//     var output = new LinkedList<Tile>();

//     uint targetLength = UniformRandom(options.LengthMin, options.LengthMax);

//     // TODO assume x/y in tilemap units, not space units. Correct?
//     int x = StartX;
//     int y = StartY;
//     int currentLength = 0;
//     uint currentPlatformLength = 0;

//     while (currentLength < targetLength) {

//       // generate next tile
//       Tile next = new Tile(tileX = x, tileY = y);
//       ++currentPlatformLength;
//       ++currentLength;

//       if (ShouldGap(options, currentPlatformLength)) {
//         uint gapSize = UniformRandom(options.GapMin, options.GapMax);
//         int gapStep = UniformRandom(options.GapStepMin, options.GapStepMax);

//         // TODO should probably sanity-check gap sizes somehow

//         x += gapSize;
//         y += gapStep;

//         // TODO insert a jump trigger

//         currentPlatformLength = 0;

//       } else {
//         ++x;
//       }

//       output.Add(next);
//     }

//     return output;

//   }

//   private bool ShouldGap(Options options, uint currentPlatformLength) {
//     if (currentPlatformLength) < options.PlatformLengthMin) return false;
//     if (currentPlatformLength) >= options.PlatformLengthMax) return true;

//     int rand = UniformRandom(options.PlatformLengthMin, options.PlatformLengthMax);
//     return currentPlatformLength > rand;
//   }

// }


