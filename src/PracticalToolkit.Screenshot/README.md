## Usage

> Even after using `Using`, there will still be a certain amount of time waiting for `GC` to release the memory
> usage, `GC` can be manually called according to the actual situation

1. Capture Area

```cs
using var runner = new ScreenshotRunner();
using var bitmap = runner.Screenshot();
```

> The following is an example of setting the capture area
```cs
var isDrawBorder = true;	// Draw the border of the capture area
var isDrawMagnifier = true;	// Draw the magnifier
var opacity = 0.5f;	        // The opacity of the capture area
var borderColor = Color.Red;// The color of the border of the capture area
var options = new RunnerOptions(isDrawBorder, isDrawMagnifier, opacity, borderColor);
using var runner = new ScreenshotRunner(options);
using var bitmap = runner.Screenshot();

```

2. Capture All Desktop

```cs
using var runner = new ScreenshotRunner();
using var bitmap = runner.ScreenshotAll();
```