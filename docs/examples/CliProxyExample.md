# C++/CLI 프록시 재구성 예제

> [!NOTE]
> 네이티브 C API와 C++/CLI 코드는 호출 경계를 설명하기 위한 재구성 예제다. 실제 `CLIInterface` 소스나 검증된 엔진 연동 코드가 아니다.

공개 C# 프레임워크가 참조하는 실제 프록시 소스는 ㈜코드쓰리 자산으로 공개하지 않는다. 아래 함수 이름·서명·핸들 소유권은 설명을 위해 정한 예시이며, 실제 엔진의 구현을 나타내지 않는다.

## 네이티브 호출 예시

예제의 C API는 이미 생성된 액터 핸들과 좌표를 받아 위치를 바꾼다.

```cpp
extern "C" {
	void Engine_SetPosition(void* actor, float x, float y, float z);
}
```

## 관리 객체로 전달하는 예시

C++/CLI 클래스가 보관한 네이티브 핸들을 C API에 넘기는 형태다. 내부 생성자는 유효한 핸들을 빌려 받는다고 가정한다. 핸들 소유자는 이 객체를 쓰는 동안 핸들을 유지한다.

```cpp
public ref class ActorProxy
{
	void* _handle;

internal:
	ActorProxy(void* handle) : _handle(handle) {}

public:
	void SetPosition(float x, float y, float z)
	{
		Engine_SetPosition(_handle, x, y, z);
	}
};
```

예제는 핸들을 생성하거나 해제하지 않는다. 실제 연동에는 소유권·해제 시점·스레드 규약에 맞는 수명 관리가 필요하다. 이 저장소에는 예제를 빌드하거나 실행하는 프로젝트가 없다.

## 공개 C# 코드의 경계

실제 [JActor.cs](../../Sources/1.%20JEngine/6.%20DanuriEngine/3.%20Actor/1.%20Base/JActor.cs)는 `CLIInterface.Actor`를 상속한다. `OnCreate()`는 기반 콜백 호출 후 컴포넌트 수집과 Transform 확보, 관리자 등록, 생성 이벤트 실행을 처리한다. 기반 클래스 구현이 없으므로 `base.OnCreate()`가 프록시를 생성하는지는 확인할 수 없다.

`Container`, `TransformGroup`, `GetComponent<T>`의 제약 타입 `ContainerComponent`는 `CLIInterface` 타입이다. 프레임워크의 공개 API에도 이 타입이 드러나므로, 콘텐츠가 프록시 의존성을 전혀 모른다고 설명할 수 없다.

| 구분 | 확인 가능한 범위 |
|---|---|
| 이 문서의 C API·`ActorProxy` | 호출 전달 형태를 설명하는 재구성 예제 |
| 공개 C# 프레임워크 | 액터·컴포넌트·이벤트·코루틴을 관리 API에 모은 구현 |
| 실제 네이티브 엔진·프록시 | 소스 비공개로 내부 구현과 호출 규약 미확인 |

엔진 API 변경의 영향은 바뀐 서명과 이를 사용하는 프레임워크·콘텐츠 코드에 따라 달라진다. 공개 저장소의 독립 코루틴 검사는 이 프록시 경계를 검증하지 않는다. 실행 범위와 명령은 [README](../../README.md#실행)에 있다.
