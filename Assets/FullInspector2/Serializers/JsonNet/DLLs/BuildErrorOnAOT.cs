#if UNITY_IOS || UNITY_WEBPLAYER
#error The default Json.NET serializer that Full Inspector includes cannot be used on this platform. Apologies! Full Inspector includes support for this platform with the Full Serializer serializer. It's even easier to use than Json.NET! Please open the serializer importer to select it ("Window/Full Inspector/Developer/Show Serializer Importer").
#endif