using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using ConsoleExtensions;
using System.IO;
using System.Security;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Auth_System
{
    internal static class Program
    {
        const string CONSOLE_TITLE = "Auth System";
        private static readonly SecureString LOGIN_URL = new SecureString();
        private static readonly SecureString SSL_PUBLICKEY = new SecureString();

        //[STAThread]
        private static void Main(string[] args)
        {
            Console.Title = "Auth System";
            Console.SetWindowSize(60, 15);
            Console.SetBufferSize(60, 15);

            //webrequest 최적화
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;

            //SecureString 만들기
#if DEBUG
            ("http://localhost/project1/loginsimple.php").ToCharArray().ToList().ForEach(x => LOGIN_URL.AppendChar(x));
#endif
#if !DEBUG
            ("https://test.hypnotik.xyz/project1/loginsimple.php").ToCharArray().ToList().ForEach(x => LOGIN_URL.AppendChar(x));
#endif
            ("3082010A0282010100E080C0692B62C7D8B6ADCA4A88FED83C" +
                "16C3CDBDC51F6FD0B57C19E6BCBE3E498CE18E4DF862404" +
                "65938ED4F223091106146BAA1851F8FF5837FAA56F6C9B9" +
                "69B944CDDDAB37D344F2FC58CBA881387BF48E3CFB8A2DA" +
                "F0F375E3CF386AD4485047501B2F765C0637D3AD62883C3" +
                "60A0131D607047A25941781C6AE1886E42711B28410148C" +
                "014885DCFB4F07937E35627DF2732392429F9E23B0148A8" +
                "24E8D0C0AC156DEF62E8B2432624E63B3E8303936EDC434" +
                "65B943F6C86CE33BFEEB6E1EC0FC7A794450361A062718D" +
                "47D824E42C4EEFA46D54A4FB7261EF1509FBA68835EA353" +
                "86237ACDB2C36B08E87B057CC27D63AD0371C6C2ECFCAB2" +
                "6C8CB4A0EB0203010001").ToCharArray().ToList().ForEach(x => SSL_PUBLICKEY.AppendChar(x));

            while (true)
            {
                Console.Clear();

                //사용자 계정 아이디
                var theID = string.Empty;
                //사용자 계정 비밀번호
                var thePW = string.Empty;

                ConsoleOutput.WriteLine("로그인 해주세요.", ConsoleColor.Cyan);

                //아이디, 비밀번호 입력받기
                ConsoleOutput.Write("ID: ", ConsoleColor.Cyan);
                theID = ConsoleInput.ReadLine();

                ConsoleOutput.Write("PW: ", ConsoleColor.Cyan);
                thePW = ConsoleInput.ReadLinePassword();

                if (theID == string.Empty || thePW == string.Empty)
                {
                    ConsoleOutput.WriteLine("로그인 실패", ConsoleColor.Red);
                    Console.Beep();
                    Console.ReadLine();
                    continue;
                }

                var nowDateTimeString = DateTime.UtcNow.ToString("yyyyMMddHHmm");
                var uniqueContentForUser = $"{nowDateTimeString.Substring(0, nowDateTimeString.Length - 1)}{theID}";
                var theSecurityCode = Sha512($"{uniqueContentForUser}login");

                var thePostDataString = $"username={theID}&password={thePW}&securitycode={theSecurityCode}";
                var thePostDataBytes = Encoding.UTF8.GetBytes(thePostDataString);

#if DEBUG   //디버그모드 전용
                ConsoleOutput.WriteLine($"theID값:\n{theID}\n", ConsoleColor.Magenta);
                ConsoleOutput.WriteLine($"thePW값:\n{thePW}\n", ConsoleColor.Magenta);
                ConsoleOutput.WriteLine($"uniqueContentForUser값:\n{uniqueContentForUser}\n", ConsoleColor.Magenta);
                ConsoleOutput.WriteLine($"theSecurityCode값:\n{theSecurityCode}\n", ConsoleColor.Magenta);
                ConsoleOutput.WriteLine($"thePostDataString값:\n{thePostDataString}\n", ConsoleColor.Magenta);
#endif

                //웹 요청 준비
                var theWebRequest = (HttpWebRequest)WebRequest.Create(SecureStringToString(LOGIN_URL));
                theWebRequest.Proxy = null;
                theWebRequest.Method = WebRequestMethods.Http.Post;
                theWebRequest.ContentType = "application/x-www-form-urlencoded";
                theWebRequest.ContentLength = thePostDataBytes.Length;
#if DEBUG   //디버그모드 전용
                theWebRequest.UserAgent = "HypnotikAuthSystem-DEBUG";//웹페이지에 디버그 메세지 요청
#endif
#if !DEBUG  //릴리즈모드 전용
                theWebRequest.UserAgent = "HypnotikAuthSystem";
#endif

                //PostData 입력
                using (var theRequestStream = theWebRequest.GetRequestStream())
                {
                    theRequestStream.Write(thePostDataBytes, 0, thePostDataBytes.Length);
                    theRequestStream.Close();
                }

                HttpWebResponse theWebResponse;

                try
                {
                    //웹 요청 실행
                    theWebResponse = (HttpWebResponse)theWebRequest.GetResponse();
                }
                catch (WebException e)
                {
                    ConsoleOutput.WriteLine($"오류 발생:{e.Message}", ConsoleColor.Red);
                    Console.Beep();
                    Console.ReadLine();
                    continue;
                }

#if DEBUG   //디버그모드 전용
                ConsoleOutput.WriteLine($"theWebResponse.StatusDescription값:\n{theWebResponse.StatusDescription}\n", ConsoleColor.Magenta);
#endif
                //웹 응답 값
                var theResponseString = string.Empty;

                //웹 응답 데이터 가져오기
                using (var theResponseStream = theWebResponse.GetResponseStream())
                {
                    using (var theStreamReader = new StreamReader(theResponseStream))
                    {
                        theResponseString = theStreamReader.ReadToEnd();
                    }
                }

#if DEBUG   //디버그모드 전용
                ConsoleOutput.WriteLine($"theResponseString값:\n{theResponseString}\n", ConsoleColor.Magenta);
#endif

                var isGetCertificateFailed = false;
                var theCertificatePublicKey = string.Empty;

                try
                {
                    //인증서 공개키 가져오기
                    theCertificatePublicKey = theWebRequest.ServicePoint.Certificate.GetPublicKeyString();
                    isGetCertificateFailed = false;                    
                }
                catch
                {
                    isGetCertificateFailed = true;
                }

                if (isGetCertificateFailed)
                {//인증서 가져오기 실패시
#if !DEBUG   //릴리즈모드 전용
                    ConsoleOutput.WriteLine("인증서를 가져올 수 없습니다.", ConsoleColor.Red);
                    Console.Beep();
                    Console.ReadLine();
                    continue;
#endif
                }
                else if (theCertificatePublicKey != SecureStringToString(SSL_PUBLICKEY))
                {//인증서 변조 검사
                    ConsoleOutput.WriteLine("인증서가 변조되었습니다.", ConsoleColor.Red);
                    //ConsoleOutput.WriteLine("인증서 공개키: " + theCertificatePublicKey, ConsoleColor.Magenta);
                    Console.Beep();
                    Console.ReadLine();
                    continue;
                }                

#if DEBUG   //디버그모드 전용
                ConsoleOutput.WriteLine($"theCertificatePublicKey값:\n{theCertificatePublicKey}\n", ConsoleColor.Magenta);
#endif

                var theUniqueCodeToLoginSuccess = Sha512($"{uniqueContentForUser}alright");

#if DEBUG   //디버그모드 전용
                ConsoleOutput.WriteLine($"theUniqueCodeToLoginSuccess값:\n{theUniqueCodeToLoginSuccess}\n", ConsoleColor.Magenta);
#endif

                if (!theResponseString.Contains(theUniqueCodeToLoginSuccess))
                {//로그인 여부 확인
                    ConsoleOutput.WriteLine("로그인 실패", ConsoleColor.Red);
                    Console.Beep();
                    Console.ReadLine();
                    continue;
                }

                //로그인 성공 후 실행될 코드 ↓
                ConsoleOutput.WriteLine("로그인 성공", ConsoleColor.Green);
                Console.Beep();

                while (true)
                {
                    Console.Clear();

                    ConsoleOutput.WriteLine($"환영합니다 {theID}님.", ConsoleColor.Cyan);
                    ConsoleOutput.WriteLine("1. 로그아웃", ConsoleColor.White);
                    ConsoleOutput.WriteLine("2. 종료", ConsoleColor.White);

                    var shouldLogout = false;

                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.D1://로그아웃 (로그인화면으로 복귀)
                            shouldLogout = true;
                            Console.Beep();
                            break;
                        case ConsoleKey.D2://종료
                            Process.GetCurrentProcess().Kill();
                            break;
                        default:
                            break;
                    }

                    if (shouldLogout)
                    {
                        break;
                    }
                }

            }
        }

        /// <summary>
        /// sha512 문자열 해쉬화
        /// </summary>
        /// <param name="plainText">해쉬화 할 문자열</param>
        /// <returns>해쉬값 반환</returns>
        private static string Sha512(string plainText)
        {
            var result = string.Empty;

            foreach (var item in new SHA512Managed().ComputeHash(Encoding.UTF8.GetBytes(plainText)))
            {
                result += item.ToString("x2");
            }

            return result;
        }

        /// <summary>
        /// SecureString 복호화
        /// </summary>
        /// <param name="secureString">SecrueString 객체</param>
        /// <returns>평문 반환</returns>
        private static string SecureStringToString(SecureString secureString)
        {
            return Marshal.PtrToStringUni(Marshal.SecureStringToGlobalAllocUnicode(secureString));
        }
    }
}
