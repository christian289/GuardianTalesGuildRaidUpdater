namespace GuardianTalesGuildRaidUpdater.Models
{
    // .NET 6 Desktop Application을 만들 때 Session 기능을 하는 Token은 Settings 파일에 저장하는 것이 MSDN의 권고사항이다.
    // https://learn.microsoft.com/ko-kr/visualstudio/ide/managing-application-settings-dotnet?view=vs-2022
    // Settings 파일은 2가지 방법으로 만들 수 있는데 큰 차이는 없는 것 같다.
    // 1. 프로젝트 파일 컨텍스트 메뉴 -> 속성에서 좌측 네비게이션 메뉴의 '설정-일반'에 가면 어플리케이션 설정 만들기 메뉴가 있다.
    // 이렇게 했을 때 프로젝트 안에 폴더로 Properties가 생성되고 그 안에 Settings 클래스 파일이 생성된다. (.NET Framework와 유사)
    // 2. 프로젝트 파일 컨텍스트 메뉴 -> 어플리케이션 설정파일 추가
    // 이렇게하면 프로젝트 안에 폴더 없이 바로 정적 클래스로 엑세스 할 수 있는 Settings 정적 클래스 파일이 생성된다.
    // 중요한 것은 설정파일들의 속성들이 모두 '사용자' 형태로 지정되어 있어야하고 '어플리케이션'으로 지정되면 readonly가 된다.
    public class AuthInfo
    {
        private string refreshToken;
        private string accessToken;
        private int expiresIn;
        private string scope;
        private string tokenType;

        public string RefreshToken
        {
            get => refreshToken;
            set
            {
                refreshToken = value;
                Properties.Settings.Default.RefreshToken = refreshToken;
            }
        }

        public string AccessToken
        {
            get => accessToken;
            set
            {
                accessToken = value;
                Properties.Settings.Default.AccessToken = accessToken;
            }
        }

        public int ExpiresIn
        {
            get => expiresIn;
            set
            {
                expiresIn = value;
                Properties.Settings.Default.ExpiresIn = expiresIn;
            }
        }

        public string Scope
        { 
            get => scope;
            set
            {
                scope = value;
                Properties.Settings.Default.Scope = scope;
            }
        }

        public string TokenType
        {
            get => tokenType;
            set
            {
                tokenType = value;
                Properties.Settings.Default.TokenType = tokenType;
            }
        }

        public void EndUpdate()
        {
            Properties.Settings.Default.LastSaveTime = DateTime.Now;
            Properties.Settings.Default.Save();
        }
    }
}
