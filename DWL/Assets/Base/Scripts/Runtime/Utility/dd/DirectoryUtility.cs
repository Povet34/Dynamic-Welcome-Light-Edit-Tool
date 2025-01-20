using System.IO;

namespace Neofect.BodyChecker.Utility
{
    public class DirectoryUtility
    {
        public static void DeleteDirectory(string _folderPath)
        {
            File.SetAttributes(_folderPath, FileAttributes.Normal); //폴더 읽기 전용 해제

            foreach (string _folder in Directory.GetDirectories(_folderPath)) //폴더 탐색
            {
                DeleteDirectory(_folder); //재귀 호출
            }

            foreach (string _file in Directory.GetFiles(_folderPath)) //파일 탐색
            {
                File.SetAttributes(_file, FileAttributes.Normal); //파일 읽기 전용 해제
                File.Delete(_file); //파일 삭제
            }

            Directory.Delete(_folderPath); //폴더 삭제
        }
    }

}
