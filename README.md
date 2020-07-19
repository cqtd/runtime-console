# Runtime Console Example

런타임 디버깅 툴에 관련된 에셋은 많습니다.  
하지만 다 자기가 필요한 것들을 중점으로 개발하기에 구매해 쓰기에는 애매합니다.  
몇몇 디버깅에 실용적인 기능들을 탑재하고 빌드하여 콘솔로 테스트해봅니다.  

![](https://github.com/cqtd/runtime-console/blob/master/Images/1.gif?raw=true)  

![](https://github.com/cqtd/runtime-console/blob/master/Images/2.gif?raw=true)  

## Example
1. `Scene/Console.unity`를 열고 플레이해보세요.  
2. ``` ` (Back Quote) ``` 키를 눌러 콘솔을 열어보세요.
3. 아래 테스트용 정적 함수를 호출해보세요

- SetMaxFPS integer
- ToggleVolumes
- ToggleMemory
- Quit, Exit

## API
```C# RuntimeConsole.Console.LoadConsole()``` 메서드로 콘솔 서브씬을 로드할 수도 있습니다.

### 구현
Reflection 사용

### 버전, 변경점
**v1.1.3**  
[CHANGELOG.md](https://github.com/cqtd/runtime-console/blob/master/CHANGELOG.md)

### To do list
- TextMeshPro Implementation
- New Input System Implementation
- For Class Instances

### Disclaimer
- Command가 포함된 클래스는 static 생성자와 instance 생성자가 호출됨을 유의할 것
- 프로덕션 빌드에 최적화되어있지 않음
- UNITY_EDITOR, DEVELOPMENT_BUILD 등의 전처리기를 이용해 적절히 수정해서 사용할 것
