# benchmark-dotnet-boxing
Benchmark of the real overhead (including GC) of the value types boxing compared to direct serialization into the byte array.

After the performance-critical synchronous procedure, there's basically two non-blocking ways of sharing the result of processing with other threads:
 - populate some kind of generic buffer, e.g. `object[]`
   - easy to implement (every value is an `object` already)
   - extra boxing cost in the main procedure for every value in the message
   - extra collection cost in the application for every value in the message
 - populate value-typed buffer, e.g. `byte[]`
   - need to implement serialization for every possible type (can be tricky for custom reference types)
   - extra serialization cost in the main procedure for every value in the message
   - no boxing, no collection cost
  
  The goal is to find out the optimum conditions for selecting one of the above strategies.
