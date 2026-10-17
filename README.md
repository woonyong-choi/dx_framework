# JFramework · dx_framework

다누리 XR 엔진의 액터 생명주기·코루틴·이벤트를 제공하는 C# 프레임워크입니다.

[![CI](https://github.com/woonyong-choi/dx_framework/actions/workflows/ci.yml/badge.svg)](https://github.com/woonyong-choi/dx_framework/actions/workflows/ci.yml)

> [!NOTE]
> 프레임워크 소스를 공개하는 포트폴리오 저장소입니다. 전체 엔진 빌드에 필요한 비공개 의존성은 현재 트리에 없으며, 코루틴 검사는 엔진 없이 실행할 수 있습니다.

## 목차

- [문제와 제약](#문제와-제약)
- [설계 결정](#설계-결정)
- [구현](#구현)
- [결과](#결과)
- [실행](#실행)
- [문서와 변경 이력](#문서와-변경-이력)
- [기여와 공개 범위](#기여와-공개-범위)

## 문제와 제약

다누리 XR의 C++ 엔진 위에서 3D 콘텐츠를 작성할 때, 액터 초기화와 컴포넌트 조회를 C# 코드에서 다루도록 만든 계층입니다. Unity와 비슷한 생명주기와 API를 사용하도록 구성했습니다.

[JFramework.csproj](JFramework.csproj)는 .NET Framework 4.7.2 라이브러리이며 `CLIInterfaceD.dll` 또는 `CLIInterface.dll`을 참조합니다. 현재 저장소만으로 전체 프레임워크와 엔진을 독립 빌드할 수 없습니다. 코루틴 검사용 .NET 9 프로젝트는 이 빌드와 별개입니다.

## 설계 결정

| 결정 | 코드에서 확인할 수 있는 동작 | 제약 |
|---|---|---|
| C++/CLI 인터페이스 위에 C# 기능 집약 | `JActor`가 엔진의 `Actor`를 상속하고 컴포넌트 조회·생명주기 처리를 제공 | 일부 공개 API에도 `CLIInterface` 타입 노출 |
| 액터 관리자가 프레임 처리 순서 관리 | `OnStart`, `Update`, 예약 실행, 코루틴, `LateUpdate`를 순서대로 호출 | 실제 엔진 콜백과 연동하려면 비공개 의존성 필요 |
| 코루틴 소스를 별도 프로젝트에서 검사 | 실제 `JCoroutine.cs`를 링크하고 `Update(float)`로 열거자 진행 | 전체 프레임워크 빌드와 렌더링은 검사 범위 밖 |

프록시와 프레임워크에 엔진 조작을 모았지만, 엔진 타입을 완전히 숨기는 구조는 아닙니다. 실제 프록시 구현과 재구성 예제의 차이는 [C++/CLI 프록시 재구성 예제](docs/examples/CliProxyExample.md)에 적었습니다.

## 구현

1. 엔진 콜백을 받은 `JActor.OnCreate()`가 기반 콜백을 호출하고 컴포넌트와 Transform을 준비합니다.
2. 액터 관리자가 등록된 액터의 프레임 처리와 예약 실행·코루틴을 갱신합니다.
3. `OnDestroy()`가 파괴 이벤트를 보내고 관리자 등록과 컴포넌트 캐시를 정리합니다.

| 코드 | 역할 |
|---|---|
| [JCoroutine.cs](Sources/1.%20JEngine/2.%20EngineCore/2.%20Main/JCoroutine.cs) | `IEnumerator` 기반 중첩·지연 코루틴 |
| [EventSystem](Sources/1.%20JEngine/2.%20EngineCore/3.%20EventSystem/) | 문자열 키 이벤트와 예약 실행 |
| [Math](Sources/1.%20JEngine/2.%20EngineCore/1.%20Math/) | Vector, Quaternion, Easing |
| [Actor](Sources/1.%20JEngine/6.%20DanuriEngine/3.%20Actor/) | 액터 생명주기·관리자·`GetComponent<T>` |
| [MainCore](Sources/1.%20JEngine/6.%20DanuriEngine/2.%20MainCore/) | 씬 전환·입력·ray picking |
| [HighPrecisionTimer](HighPrecisionTimer/) | 별도 타이머 프로젝트 |

`JCamera`와 `JWidget`은 저장소 안에서 액터 생명주기와 `GetComponent<T>`를 사용하는 소비자입니다.

## 결과

독립 콘솔 검사는 실제 코루틴 소스의 네 가지 시나리오를 확인합니다. [CI](.github/workflows/ci.yml)도 같은 명령을 실행합니다.

| 시나리오 | 확인하는 조건 |
|---|---|
| 지연 | `float`를 반환한 코루틴이 갱신 시간을 누적한 뒤 재개 |
| 중첩 | 부모 시작·자식 시작·자식 종료·부모 종료 순서 |
| 개별 중단 | 두 루틴 중 하나를 중단한 뒤 개수 1개 유지 |
| 전체 중단 | 루틴 개수 0개와 실행 중 상태 해제 |

액터·이벤트·타이머·네이티브 연동과 전체 엔진 실행은 이 검사로 검증하지 않습니다. 성능과 콘텐츠 호환성의 측정 결과도 제시하지 않습니다.

## 실행

.NET 9 SDK가 필요합니다. 2026-10-17에 macOS의 SDK 9.0.305로 아래 명령과 출력을 확인했습니다.

```sh
dotnet run --project tests/JCoroutine.ContractTests
```

```text
JCoroutine contracts: 4/4 passed
```

`4/4`는 콘솔 프로그램에 정의된 시나리오 수입니다. 검사 코드와 참조 설정은 [JCoroutine.ContractTests](tests/JCoroutine.ContractTests/)에 있습니다.

## 문서와 변경 이력

- [C++/CLI 프록시 재구성 예제](docs/examples/CliProxyExample.md)는 네이티브 호출 예시와 공개 C# 코드의 경계를 설명합니다.
- [커밋 이력](https://github.com/woonyong-choi/dx_framework/commits/main/)에서 공개 저장소의 변경을 확인할 수 있습니다.
- [이슈](https://github.com/woonyong-choi/dx_framework/issues)에서 작업의 완료 조건과 근거를, [PR](https://github.com/woonyong-choi/dx_framework/pulls)에서 변경과 검증을 확인할 수 있습니다.

| 변경 | 근거 커밋 |
|---|---|
| 빌드 산출물·패키지와 엔진 바이너리를 현재 트리에서 제거 | [fdbf0e2](https://github.com/woonyong-choi/dx_framework/commit/fdbf0e2), [07001e7](https://github.com/woonyong-choi/dx_framework/commit/07001e7) |
| 백업·폐기 코드와 소스 인코딩 정리 | [4557df0](https://github.com/woonyong-choi/dx_framework/commit/4557df0), [09d0c6b](https://github.com/woonyong-choi/dx_framework/commit/09d0c6b) |
| 부모 비활성 판정·Enum 검색·코루틴 정리 수정 | [e622a61](https://github.com/woonyong-choi/dx_framework/commit/e622a61) |
| 독립 코루틴 검사와 CI 추가 | [78c917a](https://github.com/woonyong-choi/dx_framework/commit/78c917a) |

Git 이력은 퇴사 후 소스를 일괄 업로드한 시점부터의 기록입니다. 사내 개발 순서나 기여량을 나타내지 않으며, 초기 반입 바이너리는 과거 커밋에 남아 있습니다.

## 기여와 공개 범위

최우녕이 C# 프레임워크와 `HighPrecisionTimer`를 구현해 3D 콘텐츠 개발에 사용했습니다. C++ 엔진·렌더러·셰이더와 실제 C++/CLI 프록시 소스는 ㈜코드쓰리 자산으로 공개하지 않습니다. 프록시 문서는 원본 구현을 복사한 코드가 아닌 재구성 예제입니다.

Microsoft의 `RecyclableMemoryStream`은 외부 코드이며 원본의 저작권과 MIT 라이선스 고지를 유지합니다. 저장소 전체에 적용하는 `LICENSE` 파일은 없습니다.
