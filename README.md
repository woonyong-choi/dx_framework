# JFramework — 다누리 XR 엔진의 C# 프레임워크 계층

## 한눈에

| 구분 | 내용 |
|---|---|
| 무엇 | 다누리 XR 엔진 위의 C# 프레임워크. 엔진이 바뀌어도 콘텐츠가 깨지지 않게 하는 한 겹 |
| 왜 | 협업사 10곳의 콘텐츠를 개발 중인 자체 엔진의 변경에서 지키기 위해 |
| 내 몫 | 이 저장소의 C# 코드 전부. 엔진 본체와 C++/CLI 프록시는 ㈜코드쓰리 자산이라 제외 |
| 스택 | C# · C++/CLI 경계 · DirectX 11 자체 엔진 위 |
| 검증된 사실 | 콘텐츠 쪽 코드([dx_content_interface](https://github.com/woonyong-kr/dx_content_interface))에 네이티브 호출(DllImport)이 0건이다. 엔진과는 관리되는 C# 표면으로만 만난다 |
| 한계 | 프록시 DLL 이 비공개라 단독 빌드는 불가. 구조 열람용 공개 |

**같은 사람의 다른 저장소** · 이력서 허브: <https://woonyong-kr.github.io>
[Kyro(k8s-ops)](https://github.com/woonyong-kr/k8s-ops) · [MiniDB](https://github.com/woonyong-kr/minidb) · [PintOS](https://github.com/woonyong-kr/pintos) · [dx_framework](https://github.com/woonyong-kr/dx_framework) · [dx_content_interface](https://github.com/woonyong-kr/dx_content_interface)


3D · XR 콘텐츠 제작용 자체 엔진(다누리) 위에 올린 C# 프레임워크입니다.
협업사 10곳이 콘텐츠를 만들던 개발 중인 엔진에서, **엔진이 바뀌어도
콘텐츠가 깨지지 않는 한 겹**을 만드는 것이 목적이었습니다.

```
C++ 엔진 (extern "C" 655개 함수)      ← ㈜코드쓰리, 비공개
   ↓
C++/CLI 프록시 (CLIInterface)         ← ㈜코드쓰리, 비공개 (패턴 예시는 docs/examples)
   ↓
C# 프레임워크 (이 저장소)             ← 콘텐츠가 만나는 유일한 API
   ↓
콘텐츠 (협업사 10곳)
```

핵심 규칙은 하나입니다. **C++ 엔진을 직접 부르는 코드는 콘텐츠에 없다.**
콘텐츠와 프레임워크는 관리되는 C# 표면(CLIInterface · JFramework)만 쓰고,
네이티브 함수 서명이 바뀌면 프록시와 이 저장소가 흡수합니다. 열 곳의
콘텐츠는 바뀐 것을 모릅니다.

## 코드 지도

| 영역 | 위치 | 내용 |
|---|---|---|
| 코루틴 | `Sources/1. JEngine/2. EngineCore/2. Main/JCoroutine.cs` | `IEnumerator` 기반 실행기 — 중첩 코루틴, `yield return 1.5f` 지연 |
| 이벤트 | `Sources/1. JEngine/2. EngineCore/3. EventSystem/` | 문자열 키 델리게이트 허브(JEventHandler) · 예약 실행/배속(JScheduler) |
| 수학 | `Sources/1. JEngine/2. EngineCore/1. Math/` | Vector · Quaternion · Easing |
| 액터 | `Sources/1. JEngine/6. DanuriEngine/3. Actor/` | `OnCreate → OnEnable → Update → OnDisable → OnDestroy` 생명주기, `GetComponent<T>` |
| 씬 · 입력 | `Sources/1. JEngine/6. DanuriEngine/2. MainCore/` | 코루틴 기반 씬 전환, 포인터 상태, 레이 피킹 |
| 고정밀 타이머 | `HighPrecisionTimer/` | 별도 프로젝트 |

설계 어휘를 Unity 와 같게 맞췄습니다. 콘텐츠 개발자 대부분이 Unity 경험자라,
새 API 를 배우는 비용을 없애는 것이 가장 싼 온보딩이었기 때문입니다.

이 프레임워크를 사용하는 콘텐츠 쪽 코드는
[dx_content_interface](https://github.com/woonyong-kr/dx_content_interface) 에 있습니다.

## 빌드에 대하여

이 저장소만으로는 빌드되지 않습니다. `JFramework.csproj` 가 참조하는
`CLIInterface.dll`(C++/CLI 프록시)은 ㈜코드쓰리의 자산이라 포함하지 않았습니다.
프록시가 어떤 모양인지는 [docs/examples/CliProxyExample.md](docs/examples/CliProxyExample.md)
에 재구성 예시로 정리했습니다.

## 내 것과 내 것이 아닌 것

- **내 것** — 이 저장소의 C# 코드 전부 (JEngine · DanuriEngine 계층 · HighPrecisionTimer)
- **내 것이 아닌 것** — C++ 엔진 본체 · 렌더러(DirectX 11 · OpenGL ES) · PBR 셰이더 · C++/CLI 프록시 소스 (㈜코드쓰리),
  `RecyclableMemoryStream` (Microsoft, MIT — 원 라이선스 헤더 유지)

git 이력은 퇴사 후 통째로 올린 것이라 커밋 수는 근거가 되지 않습니다.
코드가 근거입니다.

## 더 읽기

- [엔진 위에 올린 한 겹 — 생명주기 · 코루틴 · 이벤트](https://woonyong-kr.github.io/#/posts/jengine-layer)
- [655개 함수와 열 개 회사 사이 — 프록시 경계](https://woonyong-kr.github.io/#/posts/cli-proxy)
- [열 개 회사의 소스를 한곳으로](https://woonyong-kr.github.io/#/posts/source-integration)

---

## 만든 사람

**최우녕** — AI 애플리케이션을 만듭니다. LLM 의 판단 범위를 계약과
테스트로 고정하고, 만든 것은 골든셋 · 실측 벤치마크로 검증합니다.
게임사 총괄 PD 로 프로젝트 7건을 리딩한 뒤, 기술 결정의 근거를
바닥부터 다시 확인하기 위해 크래프톤 정글에서 OS · DB · 웹 서버를
직접 구현했습니다.

woonyong.kr@gmail.com
