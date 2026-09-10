# 🧱 JFramework · dx_framework

다누리 XR 엔진 위에서 콘텐츠가 사용하는 C# 프레임워크입니다. 생명주기·코루틴·이벤트 API를 제공하고 C++ 엔진 변경을 프록시 경계에서 흡수하도록 구성했습니다.

**프레임워크 소스 공개용 저장소입니다.** 비공개 `CLIInterface.dll`이 필요해 전체 엔진을 독립 빌드할 수 없습니다. 엔진 DLL이 필요 없는 코루틴 계층은 별도로 실행할 수 있습니다.

[프록시 재구성 예제](docs/examples/CliProxyExample.md) · [GitHub 프로필](https://github.com/woonyong-kr)

## 실행 가능한 부분

[.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)를 설치한 뒤 실행합니다.

```bash
dotnet run --project tests/JCoroutine.ContractTests
```

이 프로그램은 실제 `JCoroutine.cs`를 포함해 지연·중첩·중단·전체 중단을 검사합니다. 전체 XR 엔진이나 렌더링의 실행 검증을 대신하지는 않습니다.

## 구조와 설계

C++ 엔진 → C++/CLI 프록시 → C# 프레임워크 → 콘텐츠로 연결됩니다. 콘텐츠가 네이티브 엔진을 직접 호출하지 않도록 관리 API에 경계를 모았습니다. Unity 경험이 있는 콘텐츠 개발자가 익숙한 생명주기와 API를 사용할 수 있도록 설계했습니다.

| 코드 | 역할 |
| --- | --- |
| [JCoroutine.cs](Sources/1.%20JEngine/2.%20EngineCore/2.%20Main/JCoroutine.cs) | IEnumerator 기반 중첩·지연 코루틴 |
| [EventSystem](Sources/1.%20JEngine/2.%20EngineCore/3.%20EventSystem/) | 문자열 키 이벤트와 예약 실행 |
| [Math](Sources/1.%20JEngine/2.%20EngineCore/1.%20Math/) | Vector, Quaternion, Easing |
| [Actor](Sources/1.%20JEngine/6.%20DanuriEngine/3.%20Actor/) | OnCreate부터 OnDestroy까지의 생명주기·GetComponent |
| [MainCore](Sources/1.%20JEngine/6.%20DanuriEngine/2.%20MainCore/) | 씬 전환·입력·ray picking |
| [HighPrecisionTimer](HighPrecisionTimer/) | 별도 타이머 프로젝트 |

`JFbx`, `JWidget`, `JPanel`, `JUICamera`는 프레임워크 안에서 Actor 생명주기와 `GetComponent<T>`를 사용하는 실제 소비자입니다.

## 기여와 공개 범위

최우녕이 C# 프레임워크와 HighPrecisionTimer를 구현했습니다. C++ 엔진·렌더러·셰이더와 C++/CLI 프록시는 ㈜코드쓰리 자산으로 공개하지 않습니다. 프록시 문서는 원본 코드가 아닌 재구성 예제입니다.

외부 코드인 Microsoft의 `RecyclableMemoryStream`은 저작권과 MIT 라이선스 표시를 유지합니다. Git 이력은 퇴사 후 일괄 업로드한 것이므로 커밋 수를 기여 근거로 사용하지 않습니다.
