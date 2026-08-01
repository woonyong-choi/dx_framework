# C++/CLI 래핑 예시 (재구성)

> 실제 사내 프록시(`CLIInterface`) 소스는 ㈜코드쓰리의 자산이라 공개하지 않는다.
> 아래는 같은 패턴을 설명하기 위해 **재구성한 예시**다. 이 저장소의 프레임워크가
> 어떤 경계 위에 서 있는지를 보여 주는 것이 목적이다.

## 1. 네이티브 엔진 — extern "C" 경계

엔진은 함수 단위 C API 를 내보낸다 (실제 엔진은 이런 함수가 655개였다).

```cpp
// engine_api.h (네이티브)
extern "C" {
    void* Engine_CreateActor(const char* name);
    void  Engine_DestroyActor(void* actor);
    void  Engine_SetPosition(void* actor, float x, float y, float z);
    int   Engine_Update(float delta_time);
}
```

## 2. C++/CLI 프록시 — 핸들을 관리 객체로

C++/CLI 는 네이티브 포인터를 들고 있으면서 .NET 클래스를 노출할 수 있다.
수명 관리(소멸자 · 파이널라이저)와 문자열 마샬링이 이 층의 몫이다.

```cpp
// ActorProxy.cpp (C++/CLI — 재구성 예시)
public ref class ActorProxy
{
    void* _handle;   // 네이티브 액터 핸들

public:
    ActorProxy(System::String^ name)
    {
        using namespace msclr::interop;
        marshal_context ctx;
        _handle = Engine_CreateActor(ctx.marshal_as<const char*>(name));
    }

    void SetPosition(float x, float y, float z)
    {
        Engine_SetPosition(_handle, x, y, z);
    }

    ~ActorProxy() { this->!ActorProxy(); }          // Dispose
    !ActorProxy() { if (_handle) Engine_DestroyActor(_handle); }
};
```

## 3. C# 프레임워크 — 콘텐츠가 만나는 유일한 얼굴

콘텐츠는 프록시를 직접 만지지 않는다. 프레임워크의 액터가 프록시를 감싸고,
Unity 와 같은 어휘(OnCreate / Update / GetComponent)를 노출한다.

```csharp
// JActor (이 저장소의 실제 계층이 하는 일)
public class JActor : Actor
{
    public override int OnCreate()
    {
        base.OnCreate();                 // 내부적으로 프록시 생성
        JEventHandler.ExecuteEvent(Evnet.OnCreateJActor, this);
        return 0;
    }
}
```

## 왜 이렇게 나눴나

- 엔진 함수 서명이 바뀌면 고칠 곳은 **프록시와 프레임워크 한 곳**이다.
  협업사 10곳의 콘텐츠는 모른다.
- `CLIInterface` 를 아는 것은 프레임워크뿐이라는 규칙이 경계를 지킨다.
  실제로 `using CLIInterface.*` 는 이 저장소의 프레임워크 내부 파일에만 나타난다.
