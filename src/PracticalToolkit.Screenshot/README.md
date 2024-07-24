## Usage

> Even after using `Using`, there will still be a certain amount of time waiting for `GC` to release the memory usage, `GC` can be manually called according to the actual situation

1. Capture Area

```cs
using var runner = new ScreenshotRunner();
using var bitmap = runner.Screenshot();
```

2. Capture All Desktop

```cs
using var runner = new ScreenshotRunner();
using var bitmap = runner.ScreenshotAll();
```