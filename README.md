# Runtime Console Example

[한국어(KOREAN)](README_KR.md)  

There are many debugging tools in asset store for runtime.
But, almost of them is developed for their purpose so it's not fit to everyone's needs.
So, this repository provides several features to run console in runtime.

![](https://github.com/19950222/runtime-console/blob/master/Images/1.gif?raw=true)  

![](https://github.com/19950222/runtime-console/blob/master/Images/2.gif?raw=true)  

## Example
1. Play after opening `Scene/Console.unity`.  
2. Push <kbd>`</kbd> to open runtime console.
3. Execute below static functions for testing.  

- SetMaxFPS integer
- ToggleVolumes
- ToggleMemory
- Quit, Exit

### Implementation
`System.Reflection`

### Changes
**v1.1.5**  
[CHANGELOG.md](https://github.com/19950222/runtime-console/blob/master/CHANGELOG.md)

### To do list
- TextMeshPro Implementation
- New Input System Implementation
- For Class Instances

### Disclaimer
- the class that implemented 'Command' will be execute static constructor and instance constructor.  
- this is not fully compatible to shippable production build. use in QA only.  
- implement some preprocessor such as `UNITY_EDITOR`, `DEVELOPMENT_BUILD` on your purpose.
