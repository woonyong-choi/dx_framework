<h1 align="center">JFramework</h1>

<p align="center">
  다누리 XR 콘텐츠를 위한 C# 프레임워크<br>
  액터 생명주기 · 컴포넌트 조회 · 코루틴 · 이벤트
</p>

<p align="center">
  <a href="https://github.com/woonyong-choi/dx_framework/actions/workflows/ci.yml"><img src="https://github.com/woonyong-choi/dx_framework/actions/workflows/ci.yml/badge.svg" alt="코루틴 검사 CI"></a>
</p>

C++/CLI로 노출된 엔진 API 위에 콘텐츠 개발에서 반복해서 쓰는 기능을 모은 C# 계층입니다. 이 저장소는 구현한 프레임워크 소스를 공개하는 포트폴리오입니다.

## 문제와 제약

다누리 XR의 C++ 엔진 위에서 3D 콘텐츠를 작성하려면 액터 초기화, 컴포넌트 조회, 프레임 갱신을 C# 코드와 연결해야 합니다. JFramework는 Unity와 비슷한 생명주기와 API로 이 공통 처리를 제공합니다.

## 설계 결정

| 결정 | 구현 |
|---|---|
| 엔진 연동을 C# 액터 계층에 집약 | `JActor`가 엔진의 `Actor`를 상속하고 컴포넌트 준비·등록·파괴 이벤트 처리 |
| 액터 관리자가 프레임 처리 순서 관리 | `OnStart`, `Update`, 예약 실행, 코루틴, `LateUpdate` 순서로 호출 |

일부 공개 API는 `CLIInterface` 타입을 그대로 사용합니다. 프록시와 공개 C# 코드의 경계는 [C++/CLI 프록시 재구성 예제](docs/examples/CliProxyExample.md)에서 설명합니다.

## 구현

| 코드 | 역할 |
|---|---|
| [JCoroutine.cs](Sources/1.%20JEngine/2.%20EngineCore/2.%20Main/JCoroutine.cs) | `IEnumerator` 기반 중첩·지연 코루틴 |
| [EventSystem](Sources/1.%20JEngine/2.%20EngineCore/3.%20EventSystem/) | 문자열 키 이벤트와 예약 실행 |
| [Math](Sources/1.%20JEngine/2.%20EngineCore/1.%20Math/) | Vector, Quaternion, Easing |
| [Actor](Sources/1.%20JEngine/6.%20DanuriEngine/3.%20Actor/) | 액터 생명주기·관리자·`GetComponent<T>` |
| [MainCore](Sources/1.%20JEngine/6.%20DanuriEngine/2.%20MainCore/) | 씬 전환·입력·ray picking |
| [HighPrecisionTimer](HighPrecisionTimer/) | 별도 타이머 프로젝트 |

`JCamera`와 `JWidget`은 저장소 안에서 액터 생명주기와 `GetComponent<T>`를 사용하는 소비자입니다.

## 실행

전체 프레임워크는 .NET Framework 4.7.2와 비공개 `CLIInterface` DLL이 필요해 이 저장소만으로 빌드할 수 없습니다. [JFramework.csproj](JFramework.csproj)에 참조 설정이 있습니다.

엔진 없이 실행할 수 있는 부분은 독립 코루틴 검사입니다. .NET 9 SDK에서 실행합니다.

```sh
dotnet run --project tests/JCoroutine.ContractTests
```

```text
JCoroutine contracts: 4/4 passed
```

[JCoroutine.ContractTests](tests/JCoroutine.ContractTests/)는 실제 `JCoroutine.cs`를 포함해 지연·중첩·개별 중단·전체 중단 시나리오를 검사합니다. [CI](.github/workflows/ci.yml)도 같은 명령을 실행합니다. 전체 엔진·렌더링과 프록시 예제 실행은 검사 범위에 포함하지 않습니다.

## 문서와 변경 이력

- [C++/CLI 프록시](docs/examples/CliProxyExample.md)
- [커밋](https://github.com/woonyong-choi/dx_framework/commits/main/)
- [이슈](https://github.com/woonyong-choi/dx_framework/issues)
- [PR](https://github.com/woonyong-choi/dx_framework/pulls)

<details>
<summary>공개 저장소의 주요 변경</summary>

| 변경 | 근거 커밋 |
|---|---|
| 빌드 산출물·패키지와 엔진 바이너리를 현재 트리에서 제거 | [fdbf0e2](https://github.com/woonyong-choi/dx_framework/commit/fdbf0e2), [07001e7](https://github.com/woonyong-choi/dx_framework/commit/07001e7) |
| 백업·폐기 코드와 소스 인코딩 정리 | [4557df0](https://github.com/woonyong-choi/dx_framework/commit/4557df0), [09d0c6b](https://github.com/woonyong-choi/dx_framework/commit/09d0c6b) |
| 부모 비활성 판정·Enum 검색·코루틴 정리 수정 | [e622a61](https://github.com/woonyong-choi/dx_framework/commit/e622a61) |
| 독립 코루틴 검사와 CI 추가 | [78c917a](https://github.com/woonyong-choi/dx_framework/commit/78c917a) |

Git 이력은 퇴사 후 소스를 일괄 업로드한 시점부터의 기록입니다. 사내 개발 순서나 기여량을 나타내지 않으며, 초기 반입 바이너리는 과거 커밋에 남아 있습니다.

</details>

## 기여와 공개 범위

최우녕이 C# 프레임워크와 `HighPrecisionTimer`를 구현해 3D 콘텐츠 개발에 사용했습니다. C++ 엔진·렌더러·셰이더와 실제 C++/CLI 프록시 소스는 ㈜코드쓰리 자산으로 공개하지 않습니다. 문서의 프록시 코드는 재구성 예제입니다.

Microsoft의 `RecyclableMemoryStream`은 외부 코드이며 원본의 저작권과 MIT 라이선스 고지를 유지합니다. 저장소 전체에 적용하는 `LICENSE` 파일은 없습니다.
