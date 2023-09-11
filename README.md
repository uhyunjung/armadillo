<div align="center">
  <h1>아르마딜로 대시 게임</h1>
  <img src="https://user-images.githubusercontent.com/67186222/108508040-8a26f280-72fe-11eb-9e23-92912d199a11.PNG" alt="armadillo" width="500px" height="300px"/>
  <br />
</div>
<br />

## 목차
1. [**프로젝트 소개**](#1)
1. [**기술 스택**](#2)
1. [**주요 기능**](#3)
1. [**프로젝트 구성도**](#4)
1. [**발표 자료 및 데모 영상**](#5)
1. [**팀원 소개**](#6)
1. [**기간 및 일정**](#7)
1. [**실행 방법**](#8)
1. [**기술 블로그**](#9)
1. [**디렉토리 구조**](#10)

<!-- 버전기록 특이사항 SEO HeadingsMap 웹성능최적화 구글애널리틱스통계-->
<br />

<div id="1"></div>

## ❤️ 프로젝트 소개
> 아르마딜로 대시는 1명의 보스 플레이어와 4명의 아르마딜로 플레이어가 방송국 점거 및 생존을 위해 대결하는 멀티 리듬 탄막 슈팅 액션 게임입니다.<br/>유니티와 포톤의 Pun2를 이용해 개발했습니다.
<br />

<div id="2"></div>

## ⚙️ 기술 스택
![](https://img.shields.io/badge/-C%23-000000?logo=Csharp&style=flat)
![](https://img.shields.io/badge/Unity-2019.4.16f1-blue)
![](https://img.shields.io/badge/Pun2-2.27-lightgrey)
![](https://img.shields.io/badge/Photonlib-4.1.4.9-green)
<br /><br />

<div id="3"></div>

## 📲 주요 기능

| <div align="center"/>기능                      | <div align="center"/>내용                                                  |
| :------------------------ | :---------------------------------------------------------------------------------------------------------------------------------- |
| <div align="center"/>메인메뉴|- 게임시작, 환경설정, 게임종료 버튼 클릭시 해당 씬으로 이동한다|
| <div align="center"/>로비|- Join 버튼 클릭 시 방 접속하고, 방 생성 버튼 클릭 시 방 이름을 입력하고 생성할 수 있다|
| <div align="center"/>룸|- 플레이어 색깔 및 보스 플레이어 자동 지정<br/>- 3명 이상 Ready버튼 클릭 시 게임실행 화면으로 이동한다.|
| <div align="center"/>게임실행 |- 아르마딜로 플레이어는 방향키와 스페이스바로 이동 및 대시(구르기)할 수 있따<br/>- 보스 플레이어는 숫자키로 1-6번 탄막을 발사할 수 있다.<br/>&nbsp;&nbsp;&nbsp;&nbsp;- 1번 탄막 : 작은 방송국 소품들로 이루어진 원형 탄막<br/>&nbsp;&nbsp;&nbsp;&nbsp;- 2번 탄막 : 스탠드 조명에서 발사되는 빛 형태의 탄막<br/>&nbsp;&nbsp;&nbsp;&nbsp;- 3번 탄막 : 카메라 흔들기 및 화면상 탄막들 배속 감속<br/>&nbsp;&nbsp;&nbsp;&nbsp;- 4번 탄막 : 무대용 조명이 회전하는 탄막<br/>&nbsp;&nbsp;&nbsp;&nbsp;- 5번 탄막 : 확성기의 음파 탄막<br/>&nbsp;&nbsp;&nbsp;&nbsp;- 6번 탄막 : 스피커의 음표 탄막<br/>- 50초 안에 아르마딜로 플레이어 1명이라도 생존 시, 아르마딜로 플레이어의 승리<br>- 아르마딜로 플레이어가 전부 사망 시 보스 플레이어가 승리한다.|
| <div align="center"/>환경설정|- 배경음악 및 효과음을 슬라이드바로 조절한다|

<br />

<div id="4"></div>

## 🗄️ 프로젝트 구성도
| <div align="center"/>Architecture|                                                                                                                    
| :------------------------ |
| <div align="center"/><img src="https://user-images.githubusercontent.com/67186222/206713347-29f52f61-b7d2-4fe5-a0a7-880dba2d3a67.png" alt="architecture" width="900px" height="500px"/>
<br />

<div id="5"></div>

## 📽️ 발표 자료 및 데모 영상
| <div align="center"/>시작 발표| <div align="center">개발 기획|<div align="center">최종 발표|<div align="center">데모 영상|
| :------------------------ | :--------------------------- |:--------------------------- |:--------------------------- |
|🔗[**시작 발표**](https://drive.google.com/file/d/1uQV5LNFgAIRDjRnNyPYiePD6CSduXxhN/view?usp=share_link)|🔗[**개발 기획**](https://drive.google.com/file/d/13yBIzASY0htRY69fCoC9UsIBLlTZhsRf/view?usp=share_link)|🔗[**최종 발표**](https://drive.google.com/file/d/1hMR_WWvOSlnIcSozv6gDPZH3KE3tbG7P/view?usp=share_link)|🔗[**데모 영상**](https://drive.google.com/file/d/1-N2kKMqFFt01A3FWLS_9gTOyygCBWHEI/view?usp=share_link)|

<br /><br />

<div id="6"></div>

## 👨‍👩‍👦‍👦 팀원 소개
|<div align="center"/>이름|역할|개발업무|
|------|---|---|
|<div align="center"/>허선|<div align="center"/>PM, 기획, 그래픽, 코더|- 스프라이트 생성 및 애니메이션, 폰트, BGM 지정<br/>- 4번 탄막인 무대용 조명 탄막 생성 및 발사, 소멸<br/>- 테스트 및 밸런스 확인, QA|
|<div align="center"/>유현정|<div align="center"/>코더|- 보스의 탄막버튼 숫자키로 작동 및 UI<br/>- 1번 탄막인 원형 탄막 생성 및 발사, 소멸<br/>- 룸 프리팹 오브젝트로 생성 및 이동, 플레이어 색깔과 보스 지정<br/>- 플레이어, Ready 버튼, 룸, 탄막 상태의 서버 동기화 및 원격 프로시저 호출<br/>- 게임종료 및 플레이어 전부 나갈 시, 룸 오브젝트 삭제|
|<div align="center"/>임아현|<div align="center"/>코더|- 플레이어<br/>- 6번 탄막 스피커 탄막 생성 및 발사, 소멸<br/>- 플레이어의 서버 동기화 및 원격 프로시저 호출|
|<div align="center"/>조서원|<div align="center"/>그래픽|- 스프라이트 생성 및 애니메이션, 폰트, BGM 지정|
|<div align="center"/>차주영|<div align="center"/>코더|- 2번 탄막인 스탠드 조명 탄막 생성 및 발사, 소멸<br/>- 3번 탄막 카메라 흔들기 및 탄막 배속, 감속<br /> - 5번 탄막인 확성기 탄막 생성 및 발사, 소멸|
|<div align="center"/>최한비|<div align="center"/>코더|- 아르마딜로 플레이어의 이동 및 대시<br/>- 마스터 서버 접속 및 로비 이동<br/>- 방 생성 클릭 시, 방 이릅 입력 및 중복 이름 금지 및 방장 지정<br/>- Join 클릭 시 룸 접속, Close방일 경우 접속 불가능<br/>- 룸 다중 생성 및 인원 체크, 플레이어의 중복 접속 방지|
<br />

<div id="7"></div>

## 📅 기간 및 일정
**2021.1~2021.3**
<br /><br />

<div id="8"></div>

## 💡 실행 방법
### Client 환경
> 🔗[**빌드 파일**](https://drive.google.com/file/d/1Vqvty0aNQF2-GvyHOxkD5uo9hHC7ovw1/view?usp=sharing) 압축 해제 후, Armadillo develop.exe 실행

### 개발환경
**1. 원격 저장소 복제**

```bash
$ git clone https://github.com/yhyeonjg/armadillo.git
```
**2. appID 설정**
> Photon에서 어플리케이션 ID 생성 후, 유니티에서 Window > Pun2 Wizard > Setup Project > appID 등록
<br/>

<div id="9"></div>

## 🖥️ 기술 블로그
🔗[**기술 블로그**](https://whyou-story.tistory.com/search/%EC%95%84%EB%A5%B4%EB%A7%88%EB%94%9C%EB%A1%9C%20%EB%8C%80%EC%8B%9C)
<br/><br />

<div id="10"></div>

## 📂 디렉토리 구조

```
armadillo
├── Assets/
│   ├── Photon                                                                        - 포톤 서버
│   ├── Resources/Animation, Fonts, Prefabs, Sounds, Sprites                          - 애니메이션, 폰트, 프리팹, BGM, 스프라이트
│   ├── Scenes/Game Scene, Lobby Scene, Room Scene, Setting Scene, Start Scene        - 게임, 로비, 룸, 설정, 시작 씬
│   └── Scripts/
│       ├── Game/
│       │    ├── 1_Bullet/                                                             - 1번 탄막 원형 탄막
│       │    │   └── CircleFire, CircleMouse, CircleMove
│       │    ├── 2_Bullet/                                                             - 2번 탄막 스탠드 조명 탄막
│       │    │   └── ArmManager, LserOn, LaserRotation, SLRange
│       │    ├── 3_Bullet/                                                             - 3번 탄막 카메라 흔들기
│       │    │    └── CameraShake, TimeSpeed
│       │    ├── 4_Bullet/                                                             - 4번 탄막 무대용 조명 탄막
│       │    │    └── FlashRange, SLRotate, StageLight, TotalSLRange
│       │    ├── 5_Bullet/                                                             - 5번 탄막 확성기 탄막
│       │    │    ├── Left/
│       │    │    │    └── LeftSpeaker, SLRange_Left, WaveMaker
│       │    │    ├── Right/
│       │    │    │    └── RightSpeaker, SLRange_right, WaveMaker_R
│       │    │    └── LaserLotation_Left
│       │    ├── 6_Bullet/                                                             - 6번 탄막 스피커 탄막
│       │    │   └── left_move, right_move
│       │    └── BulletBtn, Controller_Time, DashMove, GameOverManager, TimeManager
│       ├── Lobby/                                                                    - 로비
│       │    └── LobbyManager RoomData
│       ├── Room/                                                                     - 룸
│       │    └── ReadyBtn, ReadyClick, Room
│       ├── Setting/                                                                  - 환경설정
│       │    └── SettingUI, SoundController
│       └── CursorManager, SoundManager, StartManager
├── Packages
├── ProjectSettings
├── .gitignore
└── README.md
```
<br/>
