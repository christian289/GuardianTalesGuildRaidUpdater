# 메모

## Image 처리

WPF의 Image에는 3가지 방식이 있다.
https://learn.microsoft.com/ko-kr/dotnet/desktop/wpf/app-development/wpf-application-resource-content-and-data-files?view=netframeworkdesktop-4.8
이 앱에서는 Resource & Content 혼합 형태로 빌드한다.
Image를 모두 Resource로 해버리면 빌드시간이 굉장히 길어진다.
따라서 이 앱이 Image를 인식할 수 있으면서 직접적으로 빌드 프로세스에 포함되지 않는 느슨한 방식인 Content를 사용한다.
Content로 설정한 것은 ```Application.GetContentStream(Uri)```로 가져올 수 있다.
https://icodebroker.tistory.com/11062

## Google Drive & Google Spread Sheets
구글 스프레드 시트에서 파일 목록을 가져오려면, 구글 드라이브 API를 사용해야 한다.
구글 드라이브 API 중에서 Files: list 를 사용한다. https://developers.google.com/drive/api/v3/reference/files/list
이 페이지의 Test API로 내 구글 드라이브의 파일들을 조회할 수 있는데, 그냥 아무 파라미터 없이 조회하면 온갖파일이 다나온다.
따라서 q 파라미터를 통해서 필터링을 해야한다.
q 파라미터의 대략적인 사용 방법은 [이 문서](https://developers.google.com/drive/api/guides/search-files)에 있다.
또한 내가 하려는 것처럼 Spread Sheets만 필터링하려고 할 때 MIME 유형인지를 확인해야하고 그것은 [이 문서](https://developers.google.com/drive/api/guides/mime-types)에서 확인할 수 있다.
보면 **Google 드라이브의 파일 중에** 내가 찾는 Spread Sheets 타입의 MIME 유형은 application/vnd.google-apps.spreadsheet 라고 나와있다.
따라서 구글 드라이브 Files: list API를 사용할 때 q 파라미터에 **mimeType = 'application/vnd.google-apps.spreadsheet'** 라고 입력한다.
그러면 응답으로 google spread sheets 파일만 결과로 온다.
문제는 API Key로는 안되고, OAuth 2.0으로만 되는 것 같아서 API Key로 사용하는 방법을 연구해야 한다.

---

# 가디언 테일즈 길드레이드 딜량 업데이트 프로그램

## 동작에 필요한 것

### Google Cloud Platform (Google 회원가입이 되어있어야 함)

#### 프로젝트 생성

1. GCP 로그인

2. IAM 및 관리자

3. 프로젝트 만들기

4. 프로젝트 이름 정하고 '만들기'

#### API 활성화 및 API Key 생성

1. GCP 대시보드 이동

2. 상단 검색창에 '라이브러리' 입력

3. 검색결과 가장 위에 있는 '라이브러리' 클릭

4. Google Drive API, Google Sheets API 클릭

5. 파란색 '사용'버튼 클릭

6. 좌측에 '사용자 인증 정보' 클릭

7. 상단에 '사용자 인증 정보 만들기' 클릭 - API 키 클릭

8. 생성된 API 키 (알 수 없는 문자열)을 복사해서 메모장에 잘 저장해 둠 (추후 GCP에서 다시 확인은 가능함)

#### Google OAuth

1. 'API 및 서비스'에서 **OAuth 동의 화면** 으로 이동.

2. 불특정 다수가 본인의 Google ID를 이용해 인증할 것이므로, 외부/내부와 상관없이 아무거나 선택.

3. 앱 이름과 이메일은 필수 항목이므로 기재.

4. (중요)앱 도메인은 정식으로 Google OAuth Client가 게시될 경우 모두 필수적으로 채워야하지만, 현재 이 앱은 혼자 이용할 것이기 때문에 테스트 상태로 게시하지 않고 내버려 둔다.

5. 개발자 연락처로 본인의 Google 이메일 아이디를 입력

6. 범위를 추가하여, API 종류에 Google Drive API와 Google Sheets API의 카테고리에 해당하는 권한을 모두 선택한다. 아마도 민감하지 않은 범위, 민감한 범위 항목에만 채워질 것이다.

7. 테스트 사용자에 본인의 Google 이메일 아이디를 추가하면 완료다.

8. 이후 **앱 게시**를 하면 Google에서 이 OAuth 2 클라이언트에 대한 심사가 이뤄지는데 우리는 공개용 APP이 아니라 본인만 쓸 것이므로 따로 앱 게시를 할 필요는 없다.

9. 나머지는 소스코드대로 진행해주면 Sheets 파일목록을 잘 가져오는 것을 볼 수 있고 거기서 ID를 이용하여 API키로 Sheet를 핸들링하면 된다.

### DataWrapper API

1. Google 아이디로 회원 가입 (대체 인증)

2. [여기](https://app.datawrapper.de/account/api-tokens) 가서 토큰 생성

- Auth : read, write

- Chart : read, write

- Theme : read

- Visualization : read

3. 엑세스 토큰 (이상한 문자열 체크) 어디에 잘 적어두기 (추후 DataWrapper에서 다시 확인은 가능함)

### Notion API

1. Google 아이디로 회원 가입 (대체 인증)

## 동작 개요

- 프로그램 실행
	- 